using UnityEngine;
using System.Collections.Generic;

public class AttractEffectManager : MonoBehaviour
{
    public static AttractEffectManager Instance;
    private class AttractEntry {
        public GameObject attractor;
        public AttractEffect effect;
        public int turnsLeft;
    }

    private List<AttractEntry> activeAttractors = new List<AttractEntry>();
    
    
    void Awake()
    {
        Instance = this;
    }

    void Update() {
        foreach (var entry in activeAttractors)
            entry.effect.ApplyAttraction(entry.attractor);
    }

    public void AddAttractor(GameObject attractor, AttractEffect effect) {
        activeAttractors.Add(new AttractEntry {
            attractor = attractor,
            effect = effect,
            turnsLeft = Mathf.RoundToInt(effect.duration)
        });
    }

    public void OnTurnStart() {
        for (int i = activeAttractors.Count - 1; i >= 0; i--) {
            var entry = activeAttractors[i];
            // entry.effect.ApplyAttraction(entry.attractor);
            entry.turnsLeft--;
            if (entry.turnsLeft <= 0) {
                entry.effect.Revert(entry.attractor);
                activeAttractors.RemoveAt(i);
            }
        }
    }
}
