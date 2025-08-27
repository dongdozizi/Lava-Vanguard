using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : UIPanel
{
    public Button restartButton;
    public Button exitButton;
    public override void Init()
    {
        base.Init();
        canOpen = false;
        
        restartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }); ;
    }
    public override void Open()
    {
        if (UIGameManager.Instance.GetOpen<WeaponPanel>())
        {
            var g = UIGameManager.Instance.GetPanel<WeaponPanel>();
            g.gameObject.SetActive(false);
            g.isOpen = false;
            base.Open();
        }
        else
        {
            CameraZoomAndMove.Instance.ZoomAndMove(base.Open);
        }
        UIGameManager.Instance.SetFocus(true);
    }
    public override void Close()
    {
        base.Close();
        CameraZoomAndMove.Instance.ResetCamera();
        Tooltip.Instance.HideTooltip();
        UIGameManager.Instance.SetFocus(false);
    }
}
