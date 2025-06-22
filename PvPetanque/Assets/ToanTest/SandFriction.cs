using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class SandFriction : MonoBehaviour
{
    private Rigidbody rb;

    public float Drag { get; private set; }
    [FormerlySerializedAs("startingDrag")]
    [Header("Global")]
    [SerializeField] private float defaultDrag = 1f;
    [SerializeField] private float dragRampUpRate = 1.01f;
    [SerializeField] private float dragDecayRate = 0.95f;
    [SerializeField] private float dragThresholdToInfinity = 100f;

    [Header("Ground impact")]
    [SerializeField] private float angleImpactFactor = 1f;
    [SerializeField] private float velocityImpactFactor = 0.1f;

    [Header("Collision with other")] 
    [SerializeField] private float collisionDragFactor = 2f;
    
    private bool onGround = false;
    private ContactPoint[] contactPoints = new ContactPoint[4];
    

    private void Start()
    {
        if (!TryGetComponent(out rb))
        {
            Debug.Log($"No Rigidbody found on {gameObject}");
            enabled = false;
            return;
        }

        Drag = defaultDrag;
        UpdateLinearDamping();
    }

    private void Update()
    {
        Debug.Log($"{Drag}");
    }

    private void FixedUpdate()
    {
        if (!onGround)
        {
            if(Mathf.Approximately(Drag, defaultDrag)) { return; }
            
            Drag *= dragDecayRate;
        }
        else if (Drag < dragThresholdToInfinity)
        {
            Drag *= dragRampUpRate;
        }
        
        UpdateLinearDamping();
        
    }
    

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            onGround = true;

            float totalImpact = 0f;
            int contactCount = other.GetContacts(contactPoints);
            for (int it = 0; it < contactCount; it++)
            {
                ContactPoint contact = contactPoints[it];

                // 0 => grazing ; 1 => straight into sand
                float angleFactor = Vector3.Dot(contact.normal, other.relativeVelocity.normalized);
                angleFactor = Mathf.Clamp01(angleFactor) * angleImpactFactor;

                float velocityFactor = rb.linearVelocity.magnitude * velocityImpactFactor;

                float impact = angleFactor * velocityFactor;
                totalImpact += impact;
            }

            if (totalImpact > 0f)
            {
                Drag *= Mathf.Pow(dragRampUpRate, totalImpact);
                UpdateLinearDamping();
                
            }
        }
        else
        {
            HandleCollision(other);
        }
    }

    private void HandleCollision(Collision other)
    {
        Rigidbody otherRb = other.collider.attachedRigidbody;
        if (!otherRb || otherRb.isKinematic) return;

        GameObject otherObject = other.gameObject;
        SandFriction otherSf;
        if (!otherObject.TryGetComponent(out otherSf)) return;

        Drag = (Drag + otherSf.Drag) * 0.5f;
        UpdateLinearDamping();

    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    private void UpdateLinearDamping()
    {
        Drag = Mathf.Max(Drag, defaultDrag);
        rb.linearDamping = Drag > dragThresholdToInfinity ? float.PositiveInfinity : Drag;
    }
    
}
