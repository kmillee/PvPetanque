using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class ItemSelectionMenu : MonoBehaviour
{
    [Header("Item Selection Panel")]
    public GameObject itemPanel; // The panel that contains the item selection UI

    [Header("Item Selection Menu")]
    public GameEffect[] availableItems; // The items to assign (6 max)
    public Transform topContainer;      // Parent of top 3 buttons
    public Transform bottomContainer;   // Parent of bottom 3 buttons

    private HashSet<GameEffect> selectedItems = MatchSettingsData.selectedItems; //?? pointeur

    // This script sets up the item selection menu by assigning GameEffects to buttons

    // Start is called before the first frame update
    void Start()
    { 
        MatchSettingsData.availableItems.Clear(); // Clear previous items
        MatchSettingsData.availableItems.AddRange(availableItems); // Add new items
        
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
        // update game item selection
        MatchSettingsData.selectedItems.Clear();
        foreach (Transform child in topContainer)
        {
            ItemButton button = child.GetComponent<ItemButton>();

            if (button != null && button.IsSelected())
            {
                GameEffect item = button.item;
                selectedItems.Add(item);
            }
        }

        foreach (Transform child in bottomContainer)
        {
            ItemButton button = child.GetComponent<ItemButton>();

            if (button != null && button.IsSelected())
            {
                GameEffect item = button.item;
                selectedItems.Add(item);
            }
        }
    }

}

