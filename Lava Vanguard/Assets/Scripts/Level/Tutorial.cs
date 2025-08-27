using Async;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance { get; private set; }
    public CanvasGroup[] tutorialGameObjects;
    public bool tutorial = true;
    public Canvas tutorialCanvas;
    public CanvasGroup basicUI;
    public CanvasGroup basicUI2;

    public TMP_Text specialText;
    public GameObject specialPlatform;

    [HideInInspector]
    public int cnt = -1;
    private void Awake()
    {
        Instance = this;
    }
    public void Init(bool isContinue = false)
    {
        PlayerManager.Instance.Init(isContinue);
        if (!tutorial)
        {
            basicUI.alpha = 1;
            basicUI2.alpha = 1;
            cnt = 15;
            //PlatformGenerator.Instance.StartGenerating();
            LevelManager.Instance.Init(isContinue);
            LevelManager.Instance.NextWave();

            AsyncManager.Instance.Init(isContinue);

            Lava.Instance.SetCameraDistance(5);
            CameraController.Instance.StartMove();
            UIGameManager.Instance.SetCanOpen<WeaponPanel>(true);
            UIGameManager.Instance.GetPanel<CardSelectorPanel>().nextWaveButton.gameObject.SetActive(true);
            SlotManager.Instance.ShowBuySlot();
            SetTutorialGameObject();
            UIGameManager.Instance.GetPanel<CardSelectorPanel>().SetRefreshButton(true);
            SetTutorialGameObject();
        }
        else
        {
            cnt++;
            AsyncManager.Instance.Init();
            SetTutorialGameObject();
        }
    }
    private void Update()
    {
        if (cnt == 0 && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))) 
        { 
            cnt++;
            SetTutorialGameObject();
            PlatformGenerator.Instance.GenerateOneLayer(new bool[] { false, true, false, false, false });

        }
        if (cnt==1 &&  PlayerManager.Instance.playerView.transform.position.y > -1 && PlayerManager.Instance.playerView.isGround)
        {
            cnt++;
            PlatformGenerator.Instance.GenerateOneLayer(new bool[5] { false, false, true, false, false });
        }
        if (cnt == 2 && PlayerManager.Instance.playerView.transform.position.y > 1.4f && PlayerManager.Instance.playerView.isGround)
        {
            cnt++;
            SetTutorialGameObject();// Show Key S
        }
        if (cnt == 3 && PlayerManager.Instance.playerView.transform.position.y < -1 && PlayerManager.Instance.playerView.isGround)
        {
            cnt++;
            SetTutorialGameObject();
            UIGameManager.Instance.SetCanOpen<WeaponPanel>(true);
            EnemyManager.Instance.GenerateSpecificEnemy(0, new Vector3(5, 5, 0));
        }
        if (cnt == 4 && UIGameManager.Instance.GetOpen<WeaponPanel>()) 
        {
            cnt++;
            SetTutorialGameObject();
            tutorialCanvas.sortingOrder = 3;
            
            UIGameManager.Instance.SetCanClose<WeaponPanel>(false);
        }
        if (cnt == 5 && SlotManager.Instance.GetCardViewNum() >= 1)
        {
            cnt++;
            SetTutorialGameObject();
            UIGameManager.Instance.SetCanClose<WeaponPanel>(true);
            
        }
        if (cnt == 6 && !UIGameManager.Instance.GetOpen<WeaponPanel>())
        {
            cnt++;
            SetTutorialGameObject();
            tutorialCanvas.sortingOrder = 1;
        }
        if (cnt == 7 && EnemyManager.Instance.enemyViews.Count == 0) 
        {
            cnt++;
            SetTutorialGameObject();

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1f);
            sequence.AppendCallback(PlatformGenerator.Instance.GenerateOneLayer);
            sequence.AppendInterval(1f);
            sequence.AppendCallback(PlatformGenerator.Instance.GenerateOneLayer);
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                basicUI.DOFade(1, 0.5f);
                basicUI2.DOFade(1, 0.5f);
            });
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                PlatformGenerator.Instance.StartGenerating();
                Lava.Instance.SetCameraDistance(5);
                CameraController.Instance.StartMove();
                LevelManager.Instance.NextWave();
            });
        }
        if (cnt == 8 && UIGameManager.Instance.GetOpen<CardSelectorPanel>())
        {
            cnt++;
            SetTutorialGameObject();

            tutorialCanvas.sortingOrder = 3;
            var panel = UIGameManager.Instance.GetPanel<CardSelectorPanel>();
            panel.PresetCard(new List<string>() { "Card_LevelUp" }, new List<int>() { 0 });
            panel.nextWaveButton.gameObject.SetActive(false);
            //buySlotButton.gameObject.SetActive(true);

        }
        if (cnt == 9 && UIGameManager.Instance.GetPanel<CardSelectorPanel>().cardSeletorViews[0].sold) 
        {
            cnt++;
            SetTutorialGameObject();
            var panel = UIGameManager.Instance.GetPanel<CardSelectorPanel>();
            panel.nextWaveButton.gameObject.SetActive(true);
            
        }
        if (cnt == 10 && !UIGameManager.Instance.GetOpen<CardSelectorPanel>())
        {
            cnt++;
            SetTutorialGameObject();

            tutorialCanvas.sortingOrder = 1;
        }
        if (cnt == 11 && UIGameManager.Instance.GetOpen<WeaponPanel>()) 
        {
            cnt++;
            SetTutorialGameObject();
            tutorialCanvas.sortingOrder = 3;
            UIGameManager.Instance.SetCanClose<WeaponPanel>(false);
        }
        if (cnt == 12 && SlotManager.Instance.GetCardViewNum() >= 2) 
        {
            cnt++;
            SetTutorialGameObject();
            UIGameManager.Instance.SetCanClose<WeaponPanel>(true);
        }
        if (cnt == 13 && !UIGameManager.Instance.GetOpen<WeaponPanel>())
        {
            cnt++;
            tutorialCanvas.sortingOrder = 1;
            SlotManager.Instance.ShowBuySlot();
            SetTutorialGameObject();
        }
        if (cnt == 14 )
        {
            cnt++;
            UIGameManager.Instance.GetPanel<CardSelectorPanel>().SetRefreshButton(true);
            Invoke("SetTutorialGameObject", 4f);
        }
        if (PlayerManager.Instance.playerView.transform.position.y < -6.5)
        {
            specialText.gameObject.SetActive(true);
            specialPlatform.gameObject.SetActive(true);
        }
        if (cnt >= 4)
        {
            specialText.gameObject.SetActive(false);
        }
    }
    private void SetTutorialGameObject()
    {
        for (int i = 0; i < tutorialGameObjects.Length; i++) 
        {
            tutorialGameObjects[i].DOFade(i == cnt ? 1 : 0, 0.5f).SetUpdate(true);
        }
    }
}
