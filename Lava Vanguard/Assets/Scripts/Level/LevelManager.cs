using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public int wave = -1;
    public int time;
    public static readonly int[] TotalTime = new int[] { 15, 20, 25, 30 };
    public TMP_Text timeText;
    public TMP_Text waveText;
    public ParticleSystem star;
    public GameObject lightGameObjects;
    [HideInInspector] public string healthForWave;
    [HideInInspector] public bool hashLongPlatform = true;
    [HideInInspector] public bool skipCredit = true;
    [HideInInspector] public bool waveEnded = false;
    [HideInInspector] public bool genLongPlatform = false;
    [HideInInspector] public bool enteredNext = false;
    private void Awake()
    {
        Instance = this;
        var m = star.main;
        m.useUnscaledTime = true;
    }
    private void ResetTimer()
    {
        time = TotalTime[Mathf.Min(wave, TotalTime.Length - 1)];
    }

    public IEnumerator CountdownTimer()
    {
        while (time > 0)
        {
            timeText.text = "Time: " + time;
            yield return new WaitForSeconds(1f);
            time--;
        }
        timeText.text = "Time's Up!";
        if (WaveHasBoss() && EnemyManager.Instance.bossRef.gameObject != null)
        {
            CameraController.Instance.StopCamera();
            yield return new WaitForSeconds(1f);
            timeText.text = "Kill the Boss!";
            yield return new WaitUntil(() => { return EnemyManager.Instance.bossRef.gameObject == null; });
            timeText.text = "Boss Killed!";
        }
        EnemyManager.Instance.StopSpawn();
        EnemyManager.Instance.killAll();
        waveEnded = true;
        if (hashLongPlatform)
        {
            genLongPlatform = true;
            CameraController.Instance.SetCameraSpeed(1.5f);
            yield return new WaitUntil(() => ShowPanel());
        }
        else
        {
            CameraController.Instance.StopCamera();
            yield return new WaitForSecondsRealtime(1f);
            yield return new WaitUntil(() => PlayerManager.Instance.playerView.isGround);
            yield return new WaitForSecondsRealtime(0.5f);
        }

        UIGameManager.Instance.Open<CardSelectorPanel>();
        GameDataManager.SaveData();
    }

    public bool WaveHasBoss()
    {
        return wave >= 3 && (wave + 1) % 2 == 0;
    }
    public void Init(bool isContinue)
    {
        wave = isContinue ? GameDataManager.SavedLevelData.Wave : GameDataManager.LevelData.Wave;
        wave--;
    }
    public void NextWave()
    {
        enteredNext = false;
        waveEnded = false;
        wave++;
        waveText.text = "Wave " + (wave + 1);
        ResetTimer();
        RecordHealthForThisWave();
        EnemyManager.Instance.StartSpawn();
        StartCoroutine("CountdownTimer");
    }
    public void StopTimer()
    {
        StopCoroutine("CountdownTimer");
        timeText.text= "Time's Up!";
        waveText.text = "Wave " + (wave + 1);
    }
    public void RecordHealthForThisWave()
    {
        int health = PlayerManager.Instance.playerView.GetHP();
        healthForWave += $"Wave {wave+1}: HP {health}\n"; 
    }

    private bool ShowPanel()
    {
        return waveEnded && CameraController.Instance.CameraStopped() && enteredNext;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            lightGameObjects.SetActive(!lightGameObjects.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            skipCredit = !skipCredit;
        }
    }
}
