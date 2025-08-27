using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Async
{
    public class AsyncManager : MonoBehaviour
    {
        public static AsyncManager Instance { get; private set; }
        [HideInInspector]
        public string cardSelection = "";
        private void Awake()
        {
            Instance = this;
        }
        public void Init(bool isContinue = false)
        {
            SlotManager.Instance.Init(isContinue);
            InventoryManager.Instance.Init(isContinue);
        }
        public void GainCard(CardRankData data)
        {
            RecordCardSelection(data);
            FindObjectOfType<ButtonSound>()?.PlayPurchaseSound();// sepcial sound effect for purchase
            if (data.CardID == "Card_RestoreHealth")
            {
                PlayerManager.Instance.playerView.RestoreHealth();
                return;
            }
            InventoryManager.Instance.inventoryView.AddCardView(data);
        }
        private void OnApplicationQuit()
        {
            //GameDataManager.SaveData();
        }
        public void RecordCardSelection(CardRankData data)
        {
            int wave = LevelManager.Instance.wave;
            cardSelection += $"Wave {wave + 1}:  {data.CardID} \n";
        }

        public void RecordSlotPurchase()
        {
            int wave = LevelManager.Instance.wave;
            if (wave >= 0)
            {
                cardSelection += $"Wave {wave + 1}:  slot \n";
            }
        }

    }
}