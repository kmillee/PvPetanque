using UnityEngine;

public class ShockWaveManager : MonoBehaviour
{
    public static ShockWaveManager Instance;
    private bool pendingShockwave = false;
    private GameObject shockwaveTarget;
    public ShockWaveEffect effect;


    public void AddShockwave(GameObject target, ShockWaveEffect effect)
    {
        pendingShockwave = true;
        shockwaveTarget = target;
        this.effect = effect;
    }

    void Awake()
    {
        Instance = this;
    }

    public void ApplyShockwave(GameObject target)
    {
        if (pendingShockwave)
        {
            effect.ApplyShockwave(target);
            ClearShockwave();
        }
    }

    public void ClearShockwave()
    {
        pendingShockwave = false;
        shockwaveTarget = null;
        effect = null;
    }
}
