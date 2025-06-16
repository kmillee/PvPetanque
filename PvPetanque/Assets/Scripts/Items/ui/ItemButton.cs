using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public GameObject selectionOverlay;

    public Tooltip tooltip; // Reference to the Tooltip script

    private bool isSelected = true;
    public GameEffect item;

    public void Setup(GameEffect itemData)
    {
        item = itemData;
        iconImage.sprite = item.icon;
        nameText.text = item.effectName;
        SetSelected(false); // Default: not selected
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(item.description);
        // Debug.Log("ItemButton: OnPointerEnter called");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleSelected();
        Debug.Log($"ItemButton: OnPointerClick called, isSelected: {isSelected}");
    }

    private void ToggleSelected()
    {
        isSelected = !isSelected;
        SetSelected(isSelected);
    }

    public void SetSelected(bool selected)
    {
        selectionOverlay?.SetActive(selected);
    }

    public bool IsSelected() => isSelected;
}
