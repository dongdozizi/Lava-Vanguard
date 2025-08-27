using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance { get; private set; }

    //Panel buttons;
    public Button pauseButton;
    public Button weaponButton;

    public GameObject redDot;

    //HP and Coin
    public TMP_Text coinText;
    public Image hpBarFill;
    public TMP_Text hpLabel;
    public TMP_Text hpNum;

    //Boss
    public TMP_Text BossHPLabel;
    public Slider bossHPBar;

    public UIPanel[] UIPanels;

    public CanvasGroup BasicUI;
    public CanvasGroup BasicUI2;

    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        foreach (var p in UIPanels)
            p.Init();
        pauseButton.onClick.AddListener(() => Open<PausePanel>());
        weaponButton.onClick.AddListener(() => Open<WeaponPanel>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !(GetOpen<PausePanel>() || GetOpen<DeathPanel>()) && !CameraZoomAndMove.Instance.isMoving) 
        {
            Switch<WeaponPanel>();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !CameraZoomAndMove.Instance.isMoving) {
            Switch<PausePanel>();
        }
    }
    public void Open<T>() where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T && p.canOpen) p.Open();
        }
    }
    public void Close<T>() where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T && p.canClose) p.Close();
        }
    }
    public bool GetOpen<T>() where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T) return p.isOpen;
        }
        return false;
    }
    public void SetCanOpen<T>(bool canOpen) where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T) p.canOpen = canOpen;
        }
    }
    public void SetCanClose<T>(bool canClose) where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T) p.canClose = canClose;
        }
    }
    public void Switch<T>() where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T) p.Switch();
        }
    }
    public T GetPanel<T>() where T : UIPanel
    {
        foreach (var p in UIPanels)
        {
            if (p is T) return p as T; 
        }
        return null; 
    }

    public void UpdateHp()
    {
        int hp = PlayerManager.Instance.playerView.GetHP();
        int maxHP = PlayerManager.Instance.playerView.GetHPLimit();
        float percentage = 1.0f * hp / maxHP;
        hpLabel.text = "HP:";
        hpNum.text = hp + "/" + maxHP;
        hpBarFill.fillAmount = percentage;
    }
    public void UpdateCoin()
    {
        int coin = PlayerManager.Instance.playerView.GetCoin();
        coinText.text = "Money: " + coin;
    }
    public void SetRedDot(bool show)
    {
        redDot.SetActive(show);
        if (show) ShakeOnce(weaponButton.transform);

    }
    private void ShakeOnce(Transform t)
    {

        Vector3 originalRotation = t.eulerAngles;

        Sequence shakeSeq = DOTween.Sequence();
        shakeSeq.Append(t.DORotate(new Vector3(0, 0, 10), 0.05f))
                .Append(t.DORotate(new Vector3(0, 0, -10), 0.1f))
                .Append(t.DORotate(originalRotation, 0.05f))
                .SetEase(Ease.InOutSine);
    }
    public void SetFocus(bool focus)
    {
        BasicUI.interactable = !focus;
        GetPanel<CardSelectorPanel>().canvasGroup.interactable = !focus;
    }
}