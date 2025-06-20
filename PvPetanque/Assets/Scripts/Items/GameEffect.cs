using UnityEngine;

public abstract class GameEffect : ScriptableObject
{
    public string effectName;
    public string description;
    public Sprite icon; 
    public float duration = 0f;

    // Called when the item is activated
    public abstract void Apply(GameObject target);

    // Optionally revert after a duration
    public virtual void Revert(GameObject target) { }
}