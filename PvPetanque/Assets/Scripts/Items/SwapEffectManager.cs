using UnityEngine;

public class SwapEffectManager : MonoBehaviour
{
    private bool pendingSwap = false;
    private GameObject swapTarget;
    public SwapEffect effect;
    public static SwapEffectManager Instance;

    void Awake()
    {    
        effect = null;
        Instance = this;
    }

    public void StartSwap(GameObject target, SwapEffect swapEffect)
    {
        pendingSwap = true;
        swapTarget = target;
        effect = swapEffect;
    }

    public void applySwap(GameObject target)
    {
        if (!pendingSwap || swapTarget == null || effect == null)
        {
            Debug.LogWarning("SwapEffectManager: No pending swap to apply.");
            return;
        }
        effect.ApplySwap(swapTarget);
        pendingSwap = false;
        swapTarget = null;
        effect = null;
    }

}
