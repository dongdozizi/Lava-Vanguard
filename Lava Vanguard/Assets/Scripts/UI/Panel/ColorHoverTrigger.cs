using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColorHoverTrigger : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 pivotPosition = new Vector2(
            rectTransform.position.x + rectTransform.rect.width/2,
            rectTransform.position.y - rectTransform.rect.height/2
        );
        
        //Tooltip.Instance.ShowColorTooltip(pivotPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Tooltip.Instance.HideColorTooltip();
    }
}
