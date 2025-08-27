using Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlotManager : MonoBehaviour
{
    public static readonly int ROW = 4;
    public static readonly int COL = 5;
    public static readonly float TOTAL_TIME = 0.25f * ROW * COL;
    public static readonly int TOTAL_GRID = ROW * COL;
    public static int START_GRID = 2;
    [HideInInspector]
    public SlotView[,] slotViews = new SlotView[ROW, COL];
    public GameObject slotPrefab;
    public GameObject cardPrefab;
    public Transform slotContainer;
    public Transform draggingTransform;
    [HideInInspector]
    public int currentTotalGrid = 0;
    
    public static SlotManager Instance { get; private set; }

    private Sequence sequence;
    private void Awake()
    {
        Instance = this;
    }
    public void Init(bool isContinue)
    {
        currentTotalGrid = 0;
        for (int i = 0; i < ROW; i++)
            for (int j = 0; j < COL; j++)
                if (slotViews[i, j] != null)
                {
                    Destroy(slotViews[i, j].gameObject);
                    slotViews[i, j] = null;
                }
        var datas = isContinue ? GameDataManager.SavedSequenceData.CardDatas : GameDataManager.SequenceData.CardDatas;
        START_GRID = datas.Count;
        foreach(var data in datas)
        {
            var t = AddSlot();
            if (t != null && data.CardID != "Card_Empty")
            {
                var cardView = Instantiate(cardPrefab, t.transform).GetComponent<CardView>();
                cardView.Init(null, GameDataManager.CardData[data.CardID], data);
                cardView.GetComponent<CardDrag>().Init(GameDataManager.CardData[data.CardID].Draggable);
                t.content = cardView;
                cardView.slot = t;
            }
        }
        UpdateAndRunSequence();
    }
    public void PresetCardView()
    {
        var cardView = InventoryManager.Instance.inventoryView.cardViews[0];
        if (cardView != null)
        {
            InventoryManager.Instance.inventoryView.RemoveCardView(cardView);
            slotViews[0, 0].AddCardView(cardView);
            UpdateAndRunSequence();
        }
    }
    public int GetCardViewNum()
    {
        int cnt = 0;
        foreach (var slot in slotViews)
            if (slot != null && slot.content != null)
                cnt++;
        return cnt;
    }
    public SlotView AddSlot()
    {
        if (currentTotalGrid == TOTAL_GRID) return null;
        int i = currentTotalGrid / COL;
        int j = currentTotalGrid % COL;
        slotViews[i, j] = Instantiate(slotPrefab, slotContainer).GetComponent<SlotView>();
        slotViews[i, j].transform.SetSiblingIndex(slotContainer.childCount - 2);
        currentTotalGrid++;
        Async.AsyncManager.Instance.RecordSlotPurchase();//record slot purchase
        return slotViews[i, j];
    }
    public void HideBuySlot()
    {
        slotContainer.GetChild(slotContainer.childCount - 1).gameObject.SetActive(false);
    }
    public void ShowBuySlot()
    {
        if (slotContainer.childCount == TOTAL_GRID + 1) return;
        slotContainer.GetChild(slotContainer.childCount - 1).gameObject.SetActive(true);
    }
    public (SlotView,bool) CheckDrag(CardView cardView)
    {
        var cardPosition = cardView.rectTransform.position;
        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                if (!slotViews[i, j]) continue;
                if (slotViews[i, j].CheckInside(cardPosition)) 
                {
                    return (slotViews[i, j], slotViews[i, j].content == null);
                }
            }
        }
        return (null,false);
    }
    public void UpdateAndRunSequence()
    {
        sequence.Kill();
        sequence = DOTween.Sequence();


        //Reset Level;
        for (int i = 0; i < ROW; i++)
            for (int j = 0; j < COL; j++)
            {
                if (!slotViews[i, j]) continue;
                if (slotViews[i, j].content != null)
                    slotViews[i, j].content.cardRankData.Level = 1;
            }

        PlayerManager.Instance.playerView.ResetSpeed();
        int currentHP = PlayerManager.Instance.playerView.GetHP();
        PlayerManager.Instance.playerView.ResetHealthLimit();
        for (int i = 0; i < ROW; i++)
            for (int j = 0; j < COL; j++)
            {
                if (!slotViews[i, j]) continue;
                var content = slotViews[i, j].content;
                if (content != null && content.cardSpriteData.Type == "Functional")
                {
                    switch (content.cardSpriteData.ID) 
                    {
                        case "Card_LevelUp":
                            for (int k = i - 1; k <= i + 1; k++)
                            {
                                for (int l = j - 1; l <= j + 1; l++)
                                {
                                    if ((k == i && l == j) || k < 0 || l < 0 || k >= ROW || l >= COL || slotViews[k, l] == null || slotViews[k, l].content == null) 
                                        continue;
                                    slotViews[k, l].content.cardRankData.Level++;
                                }
                            }
                            break;
                        case "Card_SpeedUp":
                            PlayerManager.Instance.playerView.SpeedUp();
                            break;
                        case "Card_HealthUp":
                            PlayerManager.Instance.playerView.HealthUp(currentHP);
                            break;
                    }
                }
            }

        for (int i = 0; i < ROW; i++)
            for (int j = 0; j < COL; j++)
            {
                if (!slotViews[i, j])
                {
                    sequence.AppendInterval(0.05f);
                    continue;
                }
                var slot = slotViews[i, j];
                if (slot.content != null)
                {
                    var content = slot.content;
                    if (content.cardSpriteData.Type == "Bullet")
                    {
                        content.SetLevel(false);
                        sequence.AppendCallback(() => BulletManager.Instance.GenerateBullet(content));
                    }
                }
                sequence.AppendInterval(0.05f);
            }
        sequence.SetLoops(-1);
    }

    public string GetAllSlotCardData()
    {
        string formattedData = "All Cards in Slots: ";

        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                if (!slotViews[i, j]) continue;
                var slot = slotViews[i, j];
                if (slot.content != null)
                {
                    formattedData += $"[{slot.content.cardRankData.CardID}], ";
                }
            }
        }

        if (formattedData == "All Cards in Slots: ")
        {
            return "No cards found in any slot.";
        }

        return formattedData.TrimEnd(',', ' '); 
    }
    public bool CheckANKH()
    {
        foreach(var slot in slotViews)
        {
            if (slot != null && slot.content != null && slot.content.cardSpriteData.ID == "Card_ANKH")
            {
                var cardView = slot.content;
                slot.RemoveCardView();
                Destroy(cardView.gameObject);
                return true;
            }
        }
        return false;
    }
    private void OnDestroy()
    {
        sequence.Kill();
    }

}
