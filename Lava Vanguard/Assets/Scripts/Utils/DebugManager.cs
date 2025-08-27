using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }
    private int cnt = 0;
    private bool enableDebug = false;
    private void Awake()
    {
        Instance = this;
    }
    public int GetCnt()
    {
        return cnt;
    }
    public bool typing = false;
    private void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private System.Collections.IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(desktopPath, fileName);

        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Screenshot saved to: " + filePath);
    }

    private void Update()
    {
        if (typing) return;
        if (Input.GetKeyDown(KeyCode.O) && enableDebug)
        {
            PlayerManager.Instance.playerView.GainCoin(1000);
            cnt++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)&&enableDebug)
        {
            PlayerManager.Instance.playerView.playerData.currentHealthLimit += 1000;
            UIGameManager.Instance.UpdateHp();
            cnt++;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) && enableDebug)
        {
            PlayerManager.Instance.playerView.RestoreHealth();
            UIGameManager.Instance.UpdateHp();
            cnt++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && enableDebug)
        {
            LevelManager.Instance.wave++;
            cnt++;
        }
        if (Input.GetKeyDown(KeyCode.C) && enableDebug)
            TakeScreenshot();
        if (Input.GetKeyDown(KeyCode.H))
            enableDebug = !enableDebug;
    }
}
