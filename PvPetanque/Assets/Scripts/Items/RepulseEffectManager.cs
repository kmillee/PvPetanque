using UnityEngine;
using System.Collections.Generic;

public class RepulseEffectManager : MonoBehaviour
{
    public static RepulseEffectManager Instance;
    private class RepulseEntry
    {
        public GameObject repulsor;
        public RepulseEffect effect;
        public int turnsLeft;
    }

    private List<RepulseEntry> activeRepulsors = new List<RepulseEntry>();


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        foreach (var entry in activeRepulsors)
        {
            if (entry == null)
            {
                activeRepulsors.Remove(entry);
            }
            entry.effect.ApplyRepulsion(entry.repulsor);
        }

    }

    public void AddRepulsor(GameObject repulsor, RepulseEffect effect)
    {
        activeRepulsors.Add(new RepulseEntry
        {
            repulsor = repulsor,
            effect = effect,
            turnsLeft = Mathf.RoundToInt(effect.duration)
        });
    }

    public void OnTurnStart()
    {
        for (int i = activeRepulsors.Count - 1; i >= 0; i--)
        {
            var entry = activeRepulsors[i];
            entry.effect.ApplyRepulsion(entry.repulsor);
            entry.turnsLeft--;
            if (entry.turnsLeft <= 0)
            {
                entry.effect.Revert(entry.repulsor);
                activeRepulsors.RemoveAt(i);
            }
        }
    }
    
    public void ClearRepulsors()
    {
        activeRepulsors.Clear();
    }
}
