using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ItemSelectionMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject itemPanel; // The panel that contains the item selection UI
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;


    [Header("Item Selection Menu")]
    public GameEffect[] availableItems; // The items to assign (6 max)
    public Transform topContainer;      // Parent of top 3 buttons
    public Transform bottomContainer;   // Parent of bottom 3 buttons


    // This script sets up the item selection menu by assigning GameEffects to buttons

    // Start is called before the first frame update
    void Start()
    { 
        MatchSettingsData.availableItems.Clear(); // Clear previous items
        MatchSettingsData.availableItems.AddRange(availableItems); // Add new items

        MatchSettingsData.InitializeItems(new List<GameEffect>(availableItems));
        
        List<ItemButton> allButtons = new List<ItemButton>();

        // Collect all buttons from both containers
        foreach (Transform child in topContainer)
        {
            ItemButton button = child.GetComponent<ItemButton>();
            if (button != null) allButtons.Add(button);
        }

        foreach (Transform child in bottomContainer)
        {
            ItemButton button = child.GetComponent<ItemButton>();
            if (button != null) allButtons.Add(button);
        }

        // Assign GameEffects to each button
        for (int i = 0; i < allButtons.Count; i++)
        {
            if (i < availableItems.Length)
            {
                // Debug.Log($"Assigning {availableItems[i]?.effectName ?? "null"} to button {i} ({allButtons[i].name})");
                allButtons[i].Setup(availableItems[i]);
            }
            else
            {
                // Debug.LogWarning($"No item for button {i} ({allButtons[i].name}), disabling.");
                allButtons[i].gameObject.SetActive(false);
            }
        }

    }

    // close the menu and update selection when clicking outside the panel
    void Update()
    {
        if (itemPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Pointer event
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            bool clickedOnPanel = false;
            foreach (var result in results)
            {
                if (result.gameObject == itemPanel || result.gameObject.transform.IsChildOf(itemPanel.transform))
                {
                    clickedOnPanel = true;
                    break;
                }
            }

            if (!clickedOnPanel)
            {
                CloseItemMenu();
            }
        }
    }

    public void OpenItemMenu()
    {
        itemPanel.SetActive(true);
    }

    public void CloseItemMenu()
    {
        UpdateItemSelection();

        Debug.Log("Closing item selection menu");
        itemPanel.SetActive(false);
    }

    public void UpdateItemSelection()
    {
        MatchSettingsData.selectedItems.Clear();

        foreach (Transform child in topContainer)
        {
            var button = child.GetComponent<ItemButton>();
            if (button != null && button.IsSelected())
                MatchSettingsData.selectedItems.Add(button.item);
        }

        foreach (Transform child in bottomContainer)
        {
            var button = child.GetComponent<ItemButton>();
            if (button != null && button.IsSelected())
                MatchSettingsData.selectedItems.Add(button.item);
        }
    }


}

