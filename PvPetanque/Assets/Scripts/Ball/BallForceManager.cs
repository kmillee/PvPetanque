using UnityEngine;
using System.Collections.Generic;

public class BallForceManager : MonoBehaviour
{

    public static BallForceManager Instance;
    private Dictionary<Rigidbody, Vector3> forceMap = new();
    void Awake()
    {
        Instance = this;
    }

    public void AddForce(Rigidbody rb, Vector3 force)
    {
        if (rb == null)
        {
            return;
        }
        if (forceMap.ContainsKey(rb))
            forceMap[rb] += force;
        else
            forceMap.Add(rb, force);
    }

    void FixedUpdate()
    {
        foreach (var entry in forceMap)
            entry.Key.AddForce(entry.Value, ForceMode.Impulse);

        forceMap.Clear(); // Clear the map after applying forces
    }
}
