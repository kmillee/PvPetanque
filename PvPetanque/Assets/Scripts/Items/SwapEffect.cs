using UnityEngine;


// This script defines a SwapEffect class.
// The swap effect is a type of game effect that can swap the positions of two game objects.
// More precisely, it is used to swap the position of two balls from opposite teams.
// If there are no balls within the specified radius, it does nothing.
// If there are multiple balls within the radius, it swaps with the closest one.
[CreateAssetMenu(menuName = "Game Effects/Swap Effect")]
public class SwapEffect : GameEffect
{
    [SerializeField] private float swapRadius = 1f; // Radius within which to find the other ball to swap with

    public void ApplySwap(GameObject target)
    {
        Collider[] colliders = Physics.OverlapSphere(target.transform.position, swapRadius);
        if (colliders.Length == 0) return; // No balls found within the radius

        GameObject otherBall = null;
        float closestDistance = float.MaxValue;
        

        foreach (var collider in colliders)
        {
            Ball ball = collider.GetComponent<Ball>();
            if (ball == null || ball.gameObject == target || ball is Cochonnet || ball.Team == target.GetComponent<Ball>().Team)
                continue; // Skip if no Ball component, if it's the target itself, or if it's the Cochonnet or same team

            float distance = Vector3.Distance(target.transform.position, collider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                otherBall = collider.gameObject;
            }
        }

        if (otherBall != null)
            swapBalls(target, otherBall);
    }

    public override void Apply(GameObject target)
    {
        SwapEffectManager.Instance.StartSwap(target, this);
    }
    public override void Revert(GameObject target) { }

    private void swapBalls(GameObject ball1, GameObject ball2)
    {
        if (ball1 == null || ball2 == null)
            return;
        
        Vector3 tmp = ball1.transform.position;
        ball1.transform.position = ball2.transform.position;
        ball2.transform.position = tmp;
    }

}
