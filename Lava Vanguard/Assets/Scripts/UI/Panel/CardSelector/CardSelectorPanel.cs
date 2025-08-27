using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Async;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Tilemaps;
public class CardSelectorPanel : UIPanel
{
    //public Tilemap background;
    public int optionNumber = 3;

    public Transform cardSelectorContainer;
    public GameObject cardSeletorPrefab;
    public Button nextWaveButton;
    public CanvasGroup canvasGroup;
    public List<CardSelectorView> cardSeletorViews = new List<CardSelectorView>();
    private CardSelectorView refreshView;
    private int refreshCount = 0;
    private TMP_Text refreshText;
    private readonly int initialRefreshPrice = 3;
    private readonly int addRefreshPrice = 3;
    public int RefreshPrice { get => initialRefreshPrice + addRefreshPrice * refreshCount; }

    public override void Init()
    {
        base.Init();
        for (int i = 0; i < optionNumber; i++)
        {
            var view = Instantiate(cardSeletorPrefab, cardSelectorContainer).GetComponent<CardSelectorView>();
            view.Reset();
            cardSeletorViews.Add(view);
        }
        cardSeletorViews.Add(refreshView = Instantiate(cardSeletorPrefab, cardSelectorContainer).GetComponent<CardSelectorView>());
        refreshText = refreshView.cost;
        nextWaveButton.onClick.AddListener(NextWaveFunc);
        SetRefreshButton(false);
    }
    public override void Open()
    {
        base.Open();
        CameraController.Instance.canMove = false;
        refreshCount = -1;
        if (Tutorial.Instance.cnt > 8)
            RefreshCard();
    }
    public void SetRefreshButton(bool show)
    {
        refreshView.gameObject.SetActive(show);
    }
    public void RefreshCard()
    {
        var collectableCardsList = GameDataManager.CardData
            .Where(kv => kv.Value.Collectable)
            .Select(kv => kv.Value)
            .ToList();
        for (int i = 0; i < optionNumber; i++)
        {
            var data = collectableCardsList[Random.Range(0, collectableCardsList.Count)];
            collectableCardsList.Remove(data);
            var rankData = new CardRankData(data);
            cardSeletorViews[i].Init(data, rankData);
        }
        refreshCount++;
        refreshView.Init(GameDataManager.CardData["Card_Refresh"], new CardRankData(GameDataManager.CardData["Card_Refresh"]));
        refreshView.selectButton.onClick.RemoveAllListeners();
        refreshView.selectButton.onClick.AddListener(RefreshFunc);
        refreshText.text = "$" + RefreshPrice;
    }
    public void PresetCard(List<string> IDs, List<int> prices)
    {
        int i = 0;
        for (; i < IDs.Count; i++) 
        {
            var data = GameDataManager.CardData[IDs[i]];
            var rankData = new CardRankData(data);
            data.Cost = prices[i];
            cardSeletorViews[i].Init(data, rankData);
        }
        for (; i < optionNumber; i++)
            cardSeletorViews[i].Reset();
        refreshView.Reset();
    }
    public override void Close()
    {
        base.Close();
        CameraController.Instance.canMove = true;
    }
    public void UpdateSelectButton()
    {
        foreach (var v in cardSeletorViews)
            v.UpdateSelectButton();
        refreshView.selectButton.onClick.RemoveAllListeners();
        refreshView.selectButton.onClick.AddListener(RefreshFunc);
        refreshText.text = "$" + RefreshPrice;
    }
    private void NextWaveFunc()
    {
        CameraController.Instance.ResumeCamera();
        LevelManager.Instance.NextWave();
        PlatformGenerator.Instance.shopBackground.gameObject.SetActive(false);
        PlatformGenerator.Instance.shopBackground.gameObject.transform.SetParent(PlatformGenerator.Instance.platformContainer);
        PlatformGenerator.Instance.canGenerate = true;
        Close();
    }
    private void RefreshFunc()
    {
        int price = RefreshPrice;
        if (PlayerManager.Instance.playerView.GetCoin() >= price)
        {
            PlayerManager.Instance.playerView.GainCoin(-price);
            UIGameManager.Instance.UpdateCoin();

            RefreshCard();
        }
    }
}
