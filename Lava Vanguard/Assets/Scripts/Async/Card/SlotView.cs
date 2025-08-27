using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Async
{

    public class SlotView : MonoBehaviour
    {
        public CardView content;
        public RectTransform rectTransform;
        public void Init(CardView content = null)
        {
            this.content = content;
            rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = Vector2.one * GameDataManager.CardConfig.CardSize;
        }
        public bool CheckInside(Vector2 position)
        {
            Vector2 center = rectTransform.position;
            var size = rectTransform.sizeDelta;
            var leftBottom = center - size / 2;
            var rightTop = center + size / 2;
            if (position.x >= leftBottom.x && position.x <= rightTop.x &&
                position.y >= leftBottom.y && position.y <= rightTop.y)
                return true;
            else
                return false;
        }
        public void AddCardView(CardView cardView)
        {
            content = cardView;
            content.slot = this;
            cardView.transform.SetParent(rectTransform, false);
            cardView.transform.localPosition = Vector3.zero;
        }
        public void RemoveCardView()
        {
            content.slot = null;
            content = null;
        }

    }
}