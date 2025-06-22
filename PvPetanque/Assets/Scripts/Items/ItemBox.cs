using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

public class ItemBox : MonoBehaviour
{
    public float rotationSpeed = 50f;

    [SerializeField] private TeamItemSlot teamASlot;
    [SerializeField] private TeamItemSlot teamBSlot;

    private void Update()
    {
        // Rotate the box for visual effect
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log($"ItemBox: Triggered by {other.gameObject.name}");
        //if (other == ball)
        Ball ball = other.GetComponent<Ball>();
        if (ball == null)
        {
            // nothing, it's not a ball
            Debug.LogWarning($"ItemBox: Triggered by non-ball object {other.gameObject.name}");
            return;
        }

        // Check if the ball belongs to a team
        Team team = ball.Team;

        TeamItemSlot slot = null;
        if (team == Team.TeamA)
            slot = teamASlot;
        else if (team == Team.TeamB)
            slot = teamBSlot;
        else
            return; // it's a cochonnet

        if (slot != null && !slot.HasItem)
        {
            GameEffect randomItem = GetRandomItem();
            if (randomItem == null)
            {
                Debug.LogError("ItemBox: No item was selected to assign.");
                return; // No item to assign
            }
            slot.AssignItem(randomItem);
            Debug.Log($"ItemBox: Assigned {randomItem.name} to {team}.");
            Destroy(gameObject); // Remove box after pickup
        }
        else
        {
            Debug.LogWarning($"ItemBox: {team} already has an item or slot is null.");
        }
    }

    GameEffect GetRandomItem()
    {
        var selectedItems = MatchSettingsData.selectedItems;
        int index = Random.Range(0, selectedItems.Count);
        if (selectedItems.Count == 0)
        {
            Debug.LogError("ItemBox: No items available to select from.");
            return null; // No items available
        }

        Debug.Log($"ItemBox: Selected item index {index + 1} from {selectedItems.Count} items.");
        // convert to a list to access by index
        var itemList = selectedItems.ToList();
        return Instantiate(itemList[index]); // Get a random item from the selected items
    }
    
    public void SetTeamSlots(TeamItemSlot teamASlot, TeamItemSlot teamBSlot)
    {
        this.teamASlot = teamASlot;
        this.teamBSlot = teamBSlot;
    }
}


