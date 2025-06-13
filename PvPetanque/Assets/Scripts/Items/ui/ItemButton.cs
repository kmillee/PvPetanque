using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public GameObject selectionOverlay;

    private bool isSelected = false;
    private GameEffect item;

    public void Setup(GameEffect itemData)
    {
        item = itemData;
        iconImage.sprite = item.icon;
        nameText.text = item.effectName;
        SetSelected(false); // Default: not selected
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleSelected();
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
