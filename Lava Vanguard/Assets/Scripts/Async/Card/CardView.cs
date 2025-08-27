using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Async
{
    public class CardView : MonoBehaviour
    {
        public bool addSlot = false;
        public CardSpriteData cardSpriteData;
        public CardRankData cardRankData;

        public SlotView slot;

        public RectTransform rectTransform;
        public Image background;
        public Image outline;
        public Image content;
        private void Start()
        {
            if (addSlot)
            {
                cardSpriteData = GameDataManager.CardData["Card_AddSlot"];
            }
        }
        public void Init(SlotView slot, CardSpriteData cardSpriteData, CardRankData cardRankData)//Consider using inheritance and other data like atk.
        {
            
            this.slot = slot;
            this.cardSpriteData = cardSpriteData;
            this.cardRankData = cardRankData;
            //Init size
            rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = Vector2.one * GameDataManager.CardConfig.CardSize;

            //Init sprite
            background.sprite = GameDataManager.BackgroundSprite[cardSpriteData.Background];
            outline.sprite = GameDataManager.OutlineSprite[cardSpriteData.Outline];
            content.sprite = GameDataManager.ContentSprite[cardSpriteData.Content];
            if (cardSpriteData.Type == "Bullet")
            {
                content.color = ColorCenter.CardColors[cardSpriteData.Type + cardRankData.Level];
                outline.color = ColorCenter.CardColors[cardSpriteData.Type + cardRankData.Level];
            }
            else
            {
                content.color = ColorCenter.CardTypeColors[cardSpriteData.Type];
                outline.color = ColorCenter.CardTypeColors[cardSpriteData.Type];
            }

            
        }
        public void SetLevel(bool reset = true)
        {
            if (reset)
                cardRankData.Level = 1;
            if (cardSpriteData.Type == "Bullet")
            {
                content.color = ColorCenter.CardColors[cardSpriteData.Type + cardRankData.Level];
                outline.color = ColorCenter.CardColors[cardSpriteData.Type + cardRankData.Level];
            }
        }
    }
}