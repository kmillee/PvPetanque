using UnityEngine;
using UnityEngine.UI;

public class TeamItemSlot : MonoBehaviour
{
    [Header("UI")]
    public Button useItemButton;
    public Image itemIcon; // Optional: for showing an icon
    public GameObject greyPanel; // UI panel that blocks interaction when not allowed

    [Header("Logic")]
    public TooltipTrigger tooltipTrigger; // Reference to the tooltip trigger component
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
        tooltipTrigger.currentItem = currentItem; // Update tooltip trigger with the new item
        RefreshUI();
    }

    private void OnUseItemClicked()
    {
        Debug.Log("Button clicked");
        if (currentItem != null)
        {
            Debug.Log($"Using item: {currentItem.effectName} on team object: {teamObject.name}");
            currentItem.Apply(teamObject);

            currentItem = null;
            ClearItem(); // Clear the item after use
        }
    }

    public void ClearItem()
    {
        currentItem = null;
        tooltipTrigger.currentItem = null; // Clear the tooltip trigger
        RefreshUI();
    }

    public void RefreshUI()
    {
        bool hasItem = currentItem != null;

        if (itemIcon != null)
        {
            itemIcon.enabled = hasItem && currentItem.icon != null;
            itemIcon.sprite = hasItem ? currentItem.icon : null;
        }

        if (teamObject == null)
        {
            Debug.LogWarning("no teamobject");
            return;
        }
        Ball ball = teamObject.GetComponent<Ball>();
        if (ball == null)
        {
            Debug.LogWarning("TeamItemSlot: teamObject does not have a Ball component.");
            return;
        }

        Team team = ball.Team;
        bool isMyTurn = GameManager.instance.currentTeam == team;

        useItemButton.interactable = hasItem && isMyTurn;

        if (greyPanel != null)
        {
            greyPanel.SetActive(hasItem && !isMyTurn);
            Debug.Log($"Grey panel active: {greyPanel.activeSelf} (hasItem: {hasItem}, isMyTurn: {isMyTurn})");
        }
    }


    public void SetTeamObject(GameObject obj)
    {
        teamObject = obj;
    }


}
