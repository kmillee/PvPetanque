using UnityEngine;


// This script defines a Shock wave effect
// When applied to a ball, it creates a shock wave effect when the ball hits the ground
// All gameObject (even non-ball objects) within a certain radius will be affected by the shock wave
[CreateAssetMenu(menuName = "Game Effects/ShockWave Effect")]
public class ShockWaveEffect : GameEffect
{
    [SerializeField] private float shockwaveRadius = 5f; // Radius of the shockwave
    [SerializeField] private float shockwaveForce = 10f; // Force applied to balls in the shockwave radius

    public override void Apply(GameObject target)
    {
        // Apply the shockwave effect to the target ball
        ShockWaveManager.Instance.AddShockwave(target, this);
    }
    public override void Revert(GameObject target) { }
    
    public void ApplyShockwave(GameObject target)
    {
        // Get all colliders in the shockwave radius
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, shockwaveRadius);

        foreach (var collider in hitColliders)
        {
            Ball ball = collider.GetComponent<Ball>();
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb == null)
                continue; // Skip if no Rigidbody component
            
            if (ball != null && ball.gameObject == target)
                continue; // Skip if it's the target ball itself

            // Calculate the direction from the target to the ball
            Vector3 direction = rb.transform.position - target.transform.position;
            float distance = direction.magnitude;
            direction.Normalize(); // Ensure the direction is a unit vector

            // Compute force magnitude based on distance
            float forceMagnitude = shockwaveForce / (distance * distance + 1e-6f); // Avoid division by zero
            Vector3 force = direction * forceMagnitude;
            // Apply the force to the ball's Rigidbody
            BallForceManager.Instance.AddForce(rb, force);
        }
    }

}
