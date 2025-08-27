using UnityEngine;
using UnityEngine.EventSystems;
namespace Async
{

    [RequireComponent(typeof(CardView))]
    public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private CardView cardView;
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 originalPosition;
        private bool draggable = false;
        private bool dragging = false;
        private Transform originalParent;
        private Transform draggingParent;

        public enum DragType
        {
            Sequence,
            Inventory
        }
        private DragType dragStartType;
        private void Awake()
        {

        }
        private void Start()
        {
            cardView = GetComponent<CardView>();
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            draggingParent = SlotManager.Instance.draggingTransform;
            //originalParent = transform.parent;
        }
        public void Init(bool draggable)
        {
            this.draggable = draggable;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!draggable)
                return;
            dragging = true;
            originalParent = transform.parent;
            originalPosition = rectTransform.anchoredPosition;
            transform.SetParent(draggingParent);

            if (cardView.slot == null)
                dragStartType = DragType.Inventory;
            else
                dragStartType = DragType.Sequence;

            FindObjectOfType<ButtonSound>()?.PlayPurchaseSound();// sepcial sound effect for drag

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!draggable)
                return;
            cardView.SetLevel();
            if (canvas == null) return;

            Vector2 delta = eventData.delta / canvas.scaleFactor;
            rectTransform.anchoredPosition += delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!draggable)
                return;
            dragging = false;

            //Remove from old position
            if (dragStartType == DragType.Inventory)
            {
                InventoryManager.Instance.inventoryView.RemoveCardView(cardView);
            }
            // 1. Drag to inventory.
            if (GameDataManager.InventoryConfig.CheckInside(rectTransform.anchoredPosition))
            {
                if (cardView.slot != null)
                    cardView.slot.RemoveCardView();
                InventoryManager.Instance.inventoryView.AddCardView(cardView);
                SlotManager.Instance.UpdateAndRunSequence();
                return;
            }
            // 2. Drag to another empty slot.
            var slotInfo = SlotManager.Instance.CheckDrag(cardView);
            if (slotInfo.Item1 != null && slotInfo.Item2)
            {
                if (cardView.slot != null)
                {
                    cardView.slot.RemoveCardView();
                }
                slotInfo.Item1.AddCardView(cardView);
                SlotManager.Instance.UpdateAndRunSequence();
                return;
            }
            // 3. Drag to another slot. Swap.
            if (slotInfo.Item1 != null && !slotInfo.Item2 && slotInfo.Item1 != cardView.slot) 
            {
                var targetSlot = slotInfo.Item1;
                //From a Slot
                if (cardView.slot != null)
                {
                    var currentSlot = cardView.slot;
                    var targetView = targetSlot.content;
                    currentSlot.RemoveCardView();
                    targetSlot.RemoveCardView();

                    targetSlot.AddCardView(cardView);
                    currentSlot.AddCardView(targetView);
                }
                //From inventory
                else
                {
                    var targetView = targetSlot.content;
                    //currentSlot.RemoveCardView();
                    targetSlot.RemoveCardView();

                    targetSlot.AddCardView(cardView);
                    InventoryManager.Instance.inventoryView.AddCardView(targetView);
                    //currentSlot.AddCardView(targetView);
                }
                //slotInfo.Item1.AddCardView(cardView);
                SlotManager.Instance.UpdateAndRunSequence();
                return;
            }
            // 4. Drag to self.
            SlotManager.Instance.UpdateAndRunSequence();
            // 5. Drag to somewhere else.
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (dragging)
                return;
            originalPosition = rectTransform.anchoredPosition;
            float e = 0.001f;
            if (Vector3.Distance(originalPosition, rectTransform.anchoredPosition) < e)
            {
                Tooltip.Instance.ShowTooltip(cardView);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Instance.HideTooltip();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Tooltip.Instance.HideTooltip();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (cardView.cardSpriteData.ID == "Card_AddSlot") UIGameManager.Instance.GetPanel<WeaponPanel>().BuySlot();
        }
    }
}