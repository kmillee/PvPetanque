using UnityEngine;

public class SwapEffectManager : MonoBehaviour
{
    private bool pendingSwap = false;
    private GameObject swapTarget;
    public SwapEffect effect;
    public static SwapEffectManager Instance;

    void Awake()
    {
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
        effect.ApplySwap(swapTarget);
        pendingSwap = false;
        swapTarget = null;
        effect = null;
    }

}
