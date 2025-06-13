using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;



public class ItemSelectionMenu : MonoBehaviour
{
    [Header("Item Selection Panel")]
    public GameObject itemPanel; // The panel that contains the item selection UI

    [Header("Item Selection Menu")]
    public GameEffect[] availableItems; // The items to assign (8 max)
    public Transform topContainer;      // Parent of top 4 buttons
    public Transform bottomContainer;   // Parent of bottom 4 buttons
    // This script sets up the item selection menu by assigning GameEffects to buttons
    // in both the top and bottom containers. It assumes there are 8 buttons total.

    // Start is called before the first frame update
    void Start()
    {
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
        for (int i = 0; i < allButtons.Count && i < availableItems.Length; i++)
        {
            allButtons[i].Setup(availableItems[i]);
        }

        // If fewer items than buttons, disable extra ones
        for (int i = availableItems.Length; i < allButtons.Count; i++)
        {
            allButtons[i].gameObject.SetActive(false);
        }
    }

    public void OpenItemMenu()
    {
        itemPanel.SetActive(true);
    }

    public void CloseItemMenu()
    {
        Debug.Log("Closing item selection menu");
        itemPanel.SetActive(false);
    }

}



// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections.Generic;

// public class ItemSelectionMenu : MonoBehaviour
// {
//     public Transform topContainer;
//     public Transform bottomContainer;

//     public List<GameEffect> availableItems; // must have 8 items
//     private List<ItemButton> allItemButtons = new List<ItemButton>();

//     private int selectedIndex = -1;

//     private void Awake()
//     {
//         SetupItemButtons();
//     }

//     void SetupItemButtons()
//     {
//         allItemButtons.Clear();

//         // Collect all existing buttons from both containers
//         foreach (Transform child in topContainer)
//         {
//             ItemButton btn = child.GetComponent<ItemButton>();
//             if (btn != null) allItemButtons.Add(btn);
//         }

//         foreach (Transform child in bottomContainer)
//         {
//             ItemButton btn = child.GetComponent<ItemButton>();
//             if (btn != null) allItemButtons.Add(btn);
//         }

//         for (int i = 0; i < allItemButtons.Count && i < availableItems.Count; i++)
//         {
//             var button = allItemButtons[i];
//             var item = availableItems[i];
            
//             button.iconImage.sprite = item.icon;
//             button.nameText.text = item.description;

//             button.Setup(item);

//             int index = i; // for closure
//             // button.SetSelectCallback(() => OnItemSelected(index)); 
//         }
//     }

//     void OnItemSelected(int index)
//     {
//         if (index < 0 || index >= allItemButtons.Count) return;

//         selectedIndex = index;

//         for (int i = 0; i < allItemButtons.Count; i++)
//         {
//             allItemButtons[i].SetSelected(i == selectedIndex);
//         }

//         Debug.Log("Selected item: " + availableItems[index].effectName);
//     }
// }
