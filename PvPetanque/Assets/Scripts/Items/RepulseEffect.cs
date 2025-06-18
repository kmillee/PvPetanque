using UnityEngine;

[CreateAssetMenu(menuName = "Game Effects/Repulse Effect")]
public class RepulseEffect : GameEffect
{
    [SerializeField] private float repulseForce = 1f; 
    [SerializeField] private float repulseRadius = 0.5f;

    public override void Apply(GameObject target) {
        RepulseEffectManager.Instance.AddRepulsor(target, this);
    }

    public void ApplyRepulsion(GameObject target)
    {
        // Apply a repulse force to all balls in radius around the target
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, repulseRadius);
        
        foreach (var collider in hitColliders) {
            Ball ball = collider.GetComponent<Ball>();
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (ball == null || rb == null || collider.transform.root.gameObject == target.transform.root.gameObject) {
                continue; // Skip if no Rigidbody or if it's the target itself
            }

            // Debug print colliders
            Debug.Log($"Repulsing {collider.name} from {target.name}");

            Vector3 direction = rb.transform.position - target.transform.position;
            float distance = direction.magnitude;
        
            direction.Normalize(); // Ensure the direction is a unit vector

            // Compute force magnitude based on distance
            float forceMagnitude = repulseForce / (distance * distance + 1e-6f); // Avoid division by zero
            Vector3 force = direction * forceMagnitude;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    public override void Revert(GameObject target)
    {
    }
}
