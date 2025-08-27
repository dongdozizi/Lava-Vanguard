using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Async;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Unity.Collections.LowLevel.Unsafe;
using Cinemachine;

public class RankingPanel : UIPanel
{
    [Header("No Revive")]
    public GameObject noReviveSubPanel;
    public Button noReviveHeadButton;
    public Button noReviveSortByWaveButton;
    public Button noReviveSortByKilledButton;
    public RankingNoReviveRow[] noReviveRows;

    [Header("With Revive")]
    public GameObject withReviveSubPanel;
    public Button withReviveHeadButton;
    public Button withReviveSortByWaveButton;
    public Button withReviveSortByKilledButton;
    public Button withReviveSortByReviveButton;
    public RankingWithReviveRow[] withReviveRows;

    private int revive = 0;
    private string[] rankingTitle = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th" };

    private int noReviveWaveData = -1;
    private int noReviveKilledData = -1;
   
    //firestore
    private string projectId = "csci526teamsevenranking";
    private string apiKey = "";
    private string baseUrlFireStore = "https://firestore.googleapis.com/v1/projects/";
    private string dbPath = "/databases/(default)/documents:runQuery";
    private string aggregatePath = "/databases/(default)/documents:runAggregationQuery";
    private string writePath = "/databases/(default)/documents/";

    private string noReviveParts;
    private string withReviveParts;

    private string currentRankType;

    public override void Init()
    {
        base.Init();

        // Setup no revive
        noReviveHeadButton.onClick.AddListener(noReviveSetUp);
        noReviveSortByWaveButton.onClick.AddListener(noReviveSortByWave);
        noReviveSortByKilledButton.onClick.AddListener(noReviveSortByKilled);
        // Setup with revive
        withReviveHeadButton.onClick.AddListener(withReviveSetUp);
        withReviveSortByWaveButton.onClick.AddListener(withReviveSortByWave);
        withReviveSortByKilledButton.onClick.AddListener(withReviveSortByKilled);
        withReviveSortByReviveButton.onClick.AddListener(withReviveSortByRevive);
        backButton.onClick.AddListener(() => UIGameManager.Instance.Close<RankingPanel>());
        currentRankType = "noRevive";
    }

    public override void Open()
    {
        base.Open();
        DebugManager.Instance.typing = true;
        noReviveSetUp();
        UIGameManager.Instance.SetFocus(true);
    }
    public override void Close()
    {
        base.Close();
        DebugManager.Instance.typing = false;
        Tooltip.Instance.HideTooltip();
        UIGameManager.Instance.SetFocus(false);
    }

    private void noReviveSetUp()
    {
        currentRankType = "noRevive";
        noReviveSubPanel.SetActive(true);
        withReviveSubPanel.SetActive(false);
        noReviveHeadButton.image.color = ColorCenter.RankingPanelColors["HeadButtonActive"];
        withReviveHeadButton.image.color = ColorCenter.RankingPanelColors["HeadButtonInactive"];
        for(int i = 0; i < 10; i++)
        {
            noReviveRows[i].Set(rankingTitle[i], "--", "--", "--");
        }
        StartCoroutine(getScore("noRevive","wave",noReviveWaveData));
    }

    private void withReviveSetUp()
    {
        currentRankType = "withRevive";
        noReviveSubPanel.SetActive(false);
        withReviveSubPanel.SetActive(true);
        noReviveHeadButton.image.color = ColorCenter.RankingPanelColors["HeadButtonInactive"];
        withReviveHeadButton.image.color = ColorCenter.RankingPanelColors["HeadButtonActive"];
        for(int i = 0; i < 10; i++)
        {
            withReviveRows[i].Set(rankingTitle[i], "--", "--", "--", "--");
        }
        StartCoroutine(getScore("withRevive","wave",LevelManager.Instance.wave));
    }
    private void noReviveSortByWave()
    {
        StartCoroutine(getScore("noRevive", "wave", noReviveWaveData));
    }
    private void noReviveSortByKilled()
    {
        StartCoroutine(getScore("noRevive", "killed", noReviveKilledData));
    }
    private void withReviveSortByWave()
    {
        StartCoroutine(getScore("withRevive", "wave", LevelManager.Instance.wave));
    }
    private void withReviveSortByKilled()
    {
        StartCoroutine(getScore("withRevive", "killed", EnemyManager.Instance.enemyKilled));
    }
    private void withReviveSortByRevive()
    {
        StartCoroutine(getScore("withRevive", "revive", revive));
    }
    private IEnumerator getScore(string collectionId, string sortByField, int greaterThanValue)
    {
        string url = baseUrlFireStore + projectId + dbPath+apiKey;

        // Get descending by wave
        string queryJson = 
            "{ \"structuredQuery\": {"
            + "\"from\":[{\"collectionId\":\"" + collectionId + "\"}],"
            + "\"orderBy\":[{"
                + "\"field\":{\"fieldPath\":\"" + sortByField + "\"},"
                + "\"direction\":\"DESCENDING\""
            + "}],"
            + "\"limit\":10"
            + "} }";

        byte[] body = System.Text.Encoding.UTF8.GetBytes(queryJson);
        using var uwr = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Get Score Fail: " + uwr.error);
            yield break;
        }

        var parts = uwr.downloadHandler.text;
        //Debug.Log(parts);
        if (collectionId == "noRevive")
        {
            noReviveParts = parts;
            parseNoReviveParts(sortByField);
        }
        else 
        {
            withReviveParts = parts;
            parseWithReviveParts(sortByField);
        }
    }

    private void parseNoReviveParts(string sortByField)
    {
        string wrapped = "{\"entries\":" + noReviveParts + "}";
        var list = JsonUtility.FromJson<FirestoreList>(wrapped);
        int lastValue = -1,cnt=0;
        for (int i = 0; i < noReviveRows.Length; i++)
        {
            if (i < list.entries.Length)
            {
                var entry = list.entries[i];
                string name = entry.document.fields.name.stringValue;
                int wave = int.Parse(entry.document.fields.wave.integerValue);
                int killed = int.Parse(entry.document.fields.killed.integerValue);
                if (sortByField == "wave")
                {
                    if (lastValue != wave)
                        cnt=i;
                    lastValue = wave;
                }
                else if (sortByField == "killed")
                {
                    if (lastValue != killed)
                        cnt=i;
                    lastValue = killed;
                }
                noReviveRows[i].Set(rankingTitle[cnt], name, wave.ToString(), killed.ToString());
            }
            else
            {
                noReviveRows[i].Set(rankingTitle[i], "--", "--", "--");
            }
        }
    }

    private void parseWithReviveParts(string sortByField)
    {
        string wrapped = "{\"entries\":" + withReviveParts + "}";
        var list = JsonUtility.FromJson<FirestoreList>(wrapped);
        int lastValue = -1,cnt=0;
        for (int i = 0; i < withReviveRows.Length; i++)
        {
            if (i < list.entries.Length)
            {
                var entry = list.entries[i];
                string name = entry.document.fields.name.stringValue;
                int wave = int.Parse(entry.document.fields.wave.integerValue);
                int killed = int.Parse(entry.document.fields.killed.integerValue);
                int revive=int.Parse(entry.document.fields.revive.integerValue);
                if (sortByField == "wave")
                {
                    if (lastValue != wave)
                        cnt=i;
                    lastValue = wave;
                }
                else if (sortByField == "killed")
                {
                    if (lastValue != killed)
                        cnt=i;
                    lastValue = killed;
                }
                else if (sortByField == "revive")
                {
                    if (lastValue != revive)
                        cnt=i;
                    lastValue = revive;
                }
                withReviveRows[i].Set(rankingTitle[cnt], name, wave.ToString(), killed.ToString(),revive.ToString());
            }
            else
            {
                withReviveRows[i].Set(rankingTitle[i], "--", "--", "--", "--");
            }
        }
    }

}
