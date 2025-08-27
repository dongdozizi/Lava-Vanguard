using Async;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardSelectorView : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text description;
    public Button selectButton;
    public CardView cardView;
    public TMP_Text cost;

    private CardSpriteData data;
    private CardRankData rankData;

    [HideInInspector]
    public bool sold = false;
    public void Init(CardSpriteData data, CardRankData rankData)
    {
        
        this.title.text = data.Title;
        this.title.color = ColorCenter.CardTypeColors[data.Type];

        this.description.text = data.Description;

        this.cardView.Init(null, data, rankData);

        this.data = data;
        this.rankData = rankData;

        this.sold = false;

        
        UpdateSelectButton();

        gameObject.SetActive(true);
    }
    public void Reset()
    {
        gameObject.SetActive(false);
    }
    public void UpdateSelectButton()
    {
        if (sold)
            return;
        int price = (data.Type == "BuiltIn" ? UIGameManager.Instance.GetPanel<CardSelectorPanel>().RefreshPrice : data.Cost);
        this.cost.text = "$" + price;
        bool affordable = PlayerManager.Instance.playerView.GetCoin() >= price;
        this.cost.color = affordable ? ColorCenter.SelectorPanelColors["Green"] : ColorCenter.SelectorPanelColors["Red"];
        this.selectButton.image.color = affordable ? ColorCenter.SelectorPanelColors["Green"] : ColorCenter.SelectorPanelColors["Red"];
        this.selectButton.interactable = affordable;
        this.selectButton.onClick.RemoveAllListeners();
        this.selectButton.onClick.AddListener(() =>
        {
            UIGameManager.Instance.SetRedDot(true);
            AsyncManager.Instance.GainCard(rankData);
            selectButton.interactable = false;
            cost.text = "Sold";
            sold = true;
            PlayerManager.Instance.playerView.GainCoin(-data.Cost);
            UIGameManager.Instance.UpdateCoin();
            UIGameManager.Instance.GetPanel<CardSelectorPanel>().UpdateSelectButton();
        });
    }
}
