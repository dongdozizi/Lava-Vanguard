using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Async;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using static System.Collections.Specialized.BitVector32;
public class MenuPanel : UIPanel
{
    public Button tutorialButton;
    public Button startButton;
    public Button quitButton;
    public Button continueButton;
    public Transform envTransform;
    public Transform platformContainer;
    public GameObject Trunk1;
    public GameObject continueBranch;

    public override void Init()
    {
        base.Init();
        tutorialButton.onClick.AddListener(() =>
        {
            Tutorial.Instance.tutorial = true;
            PlatformGenerator.Instance.Init();
            UIGameManager.Instance.SetCanOpen<PausePanel>(true);

            float y = -28;
            float t = 5;
            if (LevelManager.Instance.skipCredit)
            {
                envTransform.Find("Trunk1").gameObject.SetActive(false);
                platformContainer.transform.localPosition = new Vector3(0, 13.75f);
                y = -14;
                t = 1;
            }
            envTransform.DOMoveY(y, t).onComplete += () =>
            {
                
                Tutorial.Instance.Init();
            };
           
            Close();
            EventSystem.current.SetSelectedGameObject(null);
            tutorialButton.onClick.RemoveAllListeners();
        });
        startButton.onClick.AddListener(() =>
        {
            Tutorial.Instance.tutorial = false;
            PlatformGenerator.Instance.Init();
            PlatformGenerator.Instance.StartGenerating();
            UIGameManager.Instance.SetCanOpen<PausePanel>(true);

            float y = -28;
            float t = 5;
            if (LevelManager.Instance.skipCredit)
            {
                envTransform.Find("Trunk1").gameObject.SetActive(false);
                platformContainer.transform.localPosition = new Vector3(0, 13.75f);
                y = -14;
                t = 1;
            }
            envTransform.DOMoveY(y, t).onComplete += () =>
            {
                Tutorial.Instance.Init();
                SlotManager.Instance.PresetCardView();
            };

            Close();
            EventSystem.current.SetSelectedGameObject(null);
            startButton.onClick.RemoveAllListeners();
        });
        continueButton.onClick.AddListener(() =>
        {
            Tutorial.Instance.tutorial = false;
            PlatformGenerator.Instance.Init();
            PlatformGenerator.Instance.StartGenerating();
            UIGameManager.Instance.SetCanOpen<PausePanel>(true);

            float y = -28;
            float t = 5;
            if (LevelManager.Instance.skipCredit)
            {
                envTransform.Find("Trunk1").gameObject.SetActive(false);
                platformContainer.transform.localPosition = new Vector3(0, 13.75f);
                y = -14;
                t = 1;
            }
            envTransform.DOMoveY(y, t).onComplete += () =>
            {
                Tutorial.Instance.Init(isContinue: true);
            };

            Close();
            EventSystem.current.SetSelectedGameObject(null);
            startButton.onClick.RemoveAllListeners();
        });
        quitButton.onClick.AddListener(() =>
        {
            UIGameManager.Instance.Open<RankingPanel>();
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//    Application.Quit();
//#endif
        });
        if (GameDataManager.SavedLevelData.Health == 0)
        {
            Debug.Log("No saved data.");
            continueBranch.SetActive(false);
            continueButton.gameObject.SetActive(false);
        }
    }
    public override void Close()
    {
        gameObject.SetActive(false);
    }
}
