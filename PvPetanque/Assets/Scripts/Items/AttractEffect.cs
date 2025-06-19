using UnityEngine;

[CreateAssetMenu(menuName = "Game Effects/Attract Effect")]
public class AttractEffect : GameEffect
{
    [SerializeField] private float attractForce = 1f; 
    [SerializeField] private float attractRadius = 0.5f;
    [SerializeField] private float minDistance = 0.2f; // Minimum distance between two balls to apply attraction

    public override void Apply(GameObject target)
    {
        AttractEffectManager.Instance.AddAttractor(target, this);
    }

    public void ApplyAttraction(GameObject target)
    {
        // Apply a attract force to all balls in radius around the target
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, attractRadius);

        foreach (var collider in hitColliders)
        {
            Ball ball = collider.GetComponent<Ball>();
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (ball == null || rb == null || ball.gameObject == target || ball is Cochonnet)
            {
                continue; // Skip if no Rigidbody or if it's the target itself
            }

            Ball targetBall = target.GetComponent<Ball>();
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetBall == null || targetRb == null)
            {
                continue; // Skip if target does not have a Ball or Rigidbody
            }


            Vector3 direction = target.transform.position - rb.transform.position;
            float distance = direction.magnitude; 
            if (distance < minDistance)
                continue; // Skip if the distance is less than the minimum distance

            distance = Mathf.Max(distance, 0.5f);
            direction.Normalize(); // Ensure the direction is a unit vector

            // Compute force magnitude based on distance
            float forceMagnitude = attractForce / (distance * distance + 1e-6f); // Avoid division by zero
            Vector3 force = direction * forceMagnitude;
            BallForceManager.Instance.AddForce(rb, force);
            BallForceManager.Instance.AddForce(targetRb, -force); 
        }
    }

    public override void Revert(GameObject target)
    {
    }
}
