using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


// This script is used to trigger a tooltip when the mouse hovers over an object
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Tooltip tooltip;
    public GameEffect currentItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            tooltip.ShowTooltip(currentItem.effectName,currentItem.description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
}
