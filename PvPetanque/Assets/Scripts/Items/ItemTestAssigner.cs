using UnityEngine;

// test function, will be replaced by an actual game mechanic (either by turn or by hitting an item box)
public class ItemTestAssigner : MonoBehaviour
{
    [SerializeField] private TeamItemSlot teamASlot; 
    [SerializeField] private TeamItemSlot teamBSlot; 
    [SerializeField] private GameEffect gameEffect; // The item to assign
    [SerializeField] private GameEffect gameEffect2; // Optional second item for testing

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press "1" to give item to team A
        {
            teamASlot.AssignItem(gameEffect);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // Press "2" to give item to team B
        {
            teamBSlot.AssignItem(gameEffect2);
        }
    }
}
