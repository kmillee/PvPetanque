using UnityEngine;
using UnityEngine.UI;

public class TeamItemSlot : MonoBehaviour
{
    [Header("UI")]
    public Button useItemButton;
    public Image itemIcon; // Optional: for showing an icon
    public GameObject buttonHighlight; // Optional: highlight if it's the team's turn

    [Header("Logic")]
    public GameObject teamObject; // reference to team root/player if needed

    private GameEffect currentItem;

    private void Start()
    {
        // Debug.Log("TeamItemSlot initialized for " + teamObject.name);
        Debug.Log("Interactable state: " + useItemButton.interactable);
        useItemButton.onClick.AddListener(OnUseItemClicked);
        RefreshUI();
    }

    public bool HasItem => currentItem != null;

    public void AssignItem(GameEffect newItem)
    {
        if (HasItem)
        {
            Debug.LogWarning("Item slot already occupied!");
            return;
        }

        currentItem = newItem;
        RefreshUI();
    }

    private void OnUseItemClicked()
    {
        Debug.Log("Button clicked");
        if (currentItem != null)
        {
            currentItem.Apply(teamObject);
            currentItem = null;
            RefreshUI();
        }
    }

    public void ClearItem()
    {
        currentItem = null;
        RefreshUI();
    }

    private void RefreshUI()
    {
        useItemButton.interactable = currentItem != null;

        if (itemIcon != null)
        {
            if (currentItem != null)
            {
                itemIcon.enabled = true;
                itemIcon.sprite = currentItem.icon; 
            }
            else
            {
                itemIcon.enabled = false;
                itemIcon.sprite = null; // Clear the icon if no item is assigned
            }
        }

        if (buttonHighlight != null)
            buttonHighlight.SetActive(currentItem != null);
    }

    public void SetTeamObject(GameObject obj) {
        teamObject = obj;
    }

}
