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

public class DeathPanel : UIPanel
{
    //public TMP_Text survivalTime;
    public Button reviveButton;
    public Button menuButton;
    public Button exitButton;

    [Header("Submit score")]
    public Button submitScoreButton;
    public TMP_InputField nameInputField;
    public TMP_Text currentWaveText;
    public TMP_Text currentKilledText;
    public TMP_Text currentReviveText;

    [Header("No Revive")]
    public GameObject noReviveSubPanel;
    public Button noReviveHeadButton;
    public Button noReviveSortByWaveButton;
    public Button noReviveSortByKilledButton;
    public RankingNoReviveRow[] noReviveRows;
    public RankingNoReviveRow userNoReviveRow;

    [Header("With Revive")]
    public GameObject withReviveSubPanel;
    public Button withReviveHeadButton;
    public Button withReviveSortByWaveButton;
    public Button withReviveSortByKilledButton;
    public Button withReviveSortByReviveButton;
    public RankingWithReviveRow[] withReviveRows;
    public RankingWithReviveRow userWithReviveRow;

    private bool isSubmit = false;

    private int revive = 0;
    private string[] rankingTitle = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th" };

    private int noReviveWaveData = -1;
    private int noReviveKilledData = -1;
   
    //firestore
    private string projectId = "";
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
        menuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        reviveButton.onClick.AddListener(Revive);
        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }); ;

        
        // Setup submit score
        nameInputField.onValidateInput = (text, index, ch) => (char.IsLetterOrDigit(ch) && text.Length < 9) ? ch : '\0';
        submitScoreButton.onClick.AddListener(submitScore);

        // Setup no revive
        noReviveHeadButton.onClick.AddListener(noReviveSetUp);
        noReviveSortByWaveButton.onClick.AddListener(noReviveSortByWave);
        noReviveSortByKilledButton.onClick.AddListener(noReviveSortByKilled);
        // Setup with revive
        withReviveHeadButton.onClick.AddListener(withReviveSetUp);
        withReviveSortByWaveButton.onClick.AddListener(withReviveSortByWave);
        withReviveSortByKilledButton.onClick.AddListener(withReviveSortByKilled);
        withReviveSortByReviveButton.onClick.AddListener(withReviveSortByRevive);
        currentRankType = "noRevive";
        isSubmit = false;
    }

    public override void Open()
    {
        CameraZoomAndMove.Instance.ZoomAndMove(callback:() =>
        {
            base.Open();
            noReviveSetUp();
            DebugManager.Instance.typing = true;
        }); ;
        UIGameManager.Instance.SetFocus(true);
//        reviveButton.interactable = true;
        if (Tutorial.Instance.cnt < 15 || LevelManager.Instance.wave < 1)
            reviveButton.interactable = false;
        else
            reviveButton.interactable = true;
        Debug.Log(DebugManager.Instance.GetCnt());
        if (DebugManager.Instance.GetCnt() == 0)
        {
            nameInputField.interactable = true;
            submitScoreButton.interactable = true;
        }
        else
        {
            nameInputField.interactable = false;
            submitScoreButton.interactable = false;
        }
        //TODO: survivalTime
        currentWaveText.text="Wave: "+LevelManager.Instance.wave;
        currentKilledText.text = "Killed: "+EnemyManager.Instance.enemyKilled;
        currentReviveText.text = "Revive: " + revive;
    }
    public override void Close()
    {
        base.Close();
        DebugManager.Instance.typing = false;
        CameraZoomAndMove.Instance.ResetCamera();
        Tooltip.Instance.HideTooltip();
        UIGameManager.Instance.SetFocus(false);
    }
    public void Revive()
    {
        if (revive == 0)
        {
            noReviveWaveData = LevelManager.Instance.wave;
            noReviveKilledData = EnemyManager.Instance.enemyKilled;
        }
        revive++;
        isOpen = false;
        gameObject.SetActive(false);
        blocker.SetActive(false);
        Tooltip.Instance.HideTooltip();
        UIGameManager.Instance.SetFocus(false);

        PlatformGenerator.Instance.canGenerate = false;
        EnemyManager.Instance.StopSpawn();
        EnemyManager.Instance.killAll();
        EnemyManager.Instance.KillBoss();
        
        CameraZoomAndMove.Instance.ResetCameraInstantly();
        Vector3 position = PlatformGenerator.Instance.GetRevivePosition();
       
        CameraController.Instance.StopCamera();
        CameraController.Instance.SetCameraY(position.y + 4.5f);
        PlayerManager.Instance.playerView.SetPosition(position + new Vector3(0, 0.5f, 0));

        LevelManager.Instance.StopTimer();

        LevelManager.Instance.wave = GameDataManager.SavedLevelData.Wave;
        LevelManager.Instance.waveText.text = "Wave " + (GameDataManager.SavedLevelData.Wave + 1);

        PlayerManager.Instance.playerView.playerData.health = GameDataManager.SavedLevelData.Health;
        PlayerManager.Instance.playerView.playerData.coin = GameDataManager.SavedLevelData.Coin;

        UIGameManager.Instance.UpdateHp();
        UIGameManager.Instance.UpdateCoin();

        SlotManager.Instance.Init(true);
        InventoryManager.Instance.Init(true);

        UIGameManager.Instance.Open<CardSelectorPanel>();
    }

    private void submitScore()
    {
        if (nameInputField.text.Length == 0)
        {
            return;
        }
        reviveButton.interactable = false;
        submitScoreButton.interactable = false;
        nameInputField.interactable = false;
        StartCoroutine(postScore());
    }

    private IEnumerator postScore()
    {
        if (revive == 0)
        {
            noReviveWaveData = LevelManager.Instance.wave;
            noReviveKilledData = EnemyManager.Instance.enemyKilled;
        }
        string url = baseUrlFireStore + projectId + writePath + "noRevive"+apiKey;
        string requestJson = @"
    {
      ""fields"": {
        ""name"":   { ""stringValue"": """ + nameInputField.text + @""" },
        ""wave"":   { ""integerValue"": """ + noReviveWaveData + @""" },
        ""killed"": { ""integerValue"": """ + noReviveKilledData + @""" }
      }
    }";
        byte[] body = System.Text.Encoding.UTF8.GetBytes(requestJson);
        using var uwr = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
            Debug.LogError("Submit noRevive Fail: " + uwr.error);
        else
            Debug.Log("Submit noRevive Success, response:\n" + uwr.downloadHandler.text);

        if (revive > 0)
        {
            url = baseUrlFireStore + projectId + writePath + "withRevive"+apiKey;
            requestJson = @"
        {
          ""fields"": {
            ""name"":   { ""stringValue"": """ + nameInputField.text + @""" },
            ""wave"":   { ""integerValue"": """ + LevelManager.Instance.wave + @""" },
            ""killed"": { ""integerValue"": """ + EnemyManager.Instance.enemyKilled + @""" },
            ""revive"": { ""integerValue"": """ + revive + @""" }
          }
        }";
            body = System.Text.Encoding.UTF8.GetBytes(requestJson);
            using var uwr2 = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(body),
                downloadHandler = new DownloadHandlerBuffer()
            };
            uwr2.SetRequestHeader("Content-Type", "application/json");
            yield return uwr2.SendWebRequest();

            if (uwr2.result != UnityWebRequest.Result.Success)
                Debug.LogError("Submit withRevive Fail: " + uwr2.error);
            //else
            //    Debug.Log("Submit withRevive Success, response:\n" + uwr2.downloadHandler.text);
        }
        if (currentRankType == "noRevive")
        {
            noReviveSetUp();
        }
        else if(currentRankType == "withRevive")
        {
            withReviveSetUp();
        }
        isSubmit = true;
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
        userNoReviveRow.Set("--/--", "You", "--", "--");
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
        userWithReviveRow.Set("--/--", "You", "--", "--", "--");
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

        // Get total count
        yield return StartCoroutine(getUserRank(collectionId,sortByField,greaterThanValue));
    }

    public IEnumerator getUserRank(string collectionId,string sortByField, int greaterThanValue)
    {
        int totalCount = -1, rank = -1;
        string url = baseUrlFireStore + projectId + aggregatePath+apiKey;
        string totalCountJson =
              "{"
            + "\"structuredAggregationQuery\":{"
            + "\"aggregations\":[{\"count\":{}}],"
        + "\"structuredQuery\":{"
            + "\"from\":[{\"collectionId\":\"" + collectionId + "\"}]"
            + "}"
            + "}"
            + "}";
        byte[] totalCountBody = Encoding.UTF8.GetBytes(totalCountJson);
        using var totalCountUwr = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(totalCountBody),
            downloadHandler = new DownloadHandlerBuffer()
        };
        totalCountUwr.SetRequestHeader("Content-Type", "application/json");

        yield return totalCountUwr.SendWebRequest();

        if (totalCountUwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[TotalCount] Error: {totalCountUwr.error}");
            yield break;
        }
        //Debug.Log("total return " + totalCountUwr.downloadHandler.text);
        string wrapped = "{ \"items\": " + totalCountUwr.downloadHandler.text + " }";
        var list = JsonUtility.FromJson<AggregationResponseList>(wrapped);


        if (list.items != null && list.items.Length > 0)
            totalCount = int.Parse(list.items[0].result.aggregateFields.field_1.integerValue);
        else
            Debug.LogError("TotalCount error: Empty response array");

        if (isSubmit==true&&!(revive==0&&currentRankType=="withRevive"))
        {
            string rankJson =
                  "{"
                + "\"structuredAggregationQuery\":{"
                + "\"aggregations\":[{\"count\":{}}],"
                + "\"structuredQuery\":{"
                + "\"from\":[{\"collectionId\":\"" + collectionId + "\"}],"
                + "\"where\":{"
                + "\"fieldFilter\":{"
                + "\"field\":{\"fieldPath\":\""+sortByField+"\"},"
                + "\"op\":\"GREATER_THAN\","
                + "\"value\":{\"integerValue\":\"" + greaterThanValue + "\"}"
                + "}"
                + "}"
                + "}"
                + "}"
                + "}";
            byte[] rankBody = Encoding.UTF8.GetBytes(rankJson);
            using var rankUwr = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(rankBody),
                downloadHandler = new DownloadHandlerBuffer()
            };
            rankUwr.SetRequestHeader("Content-Type", "application/json");
            yield return rankUwr.SendWebRequest();

            if (rankUwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[TotalCount] Error: {rankUwr.error}");
                yield break;
            }
            //Debug.Log("total return " + rankUwr.downloadHandler.text);
            wrapped = "{ \"items\": " + rankUwr.downloadHandler.text + " }";
            list = JsonUtility.FromJson<AggregationResponseList>(wrapped);

            if (list.items != null && list.items.Length > 0)
                rank = int.Parse(list.items[0].result.aggregateFields.field_1.integerValue)+1;
            else
                Debug.LogError("TotalCount error: Empty response array");
            if (collectionId == "noRevive")
            {
                userNoReviveRow.Set(rank.ToString() + "/" + totalCount.ToString(), "You", noReviveWaveData.ToString(), noReviveKilledData.ToString());
            }
            else
            {
                userWithReviveRow.Set(rank.ToString() + "/" + totalCount.ToString(), "You", LevelManager.Instance.wave.ToString(), EnemyManager.Instance.enemyKilled.ToString(), revive.ToString());
            }
        }
        else
        {
            if (collectionId == "noRevive")
            {
                userNoReviveRow.Set("--/" + totalCount.ToString(), "You", "--","--");
            }
            else
            {
                userWithReviveRow.Set("--/" + totalCount.ToString(), "You", "--","--","--");
            }
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
