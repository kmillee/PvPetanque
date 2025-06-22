using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SandFriction : MonoBehaviour
{
    private static readonly RandomGaussian rng = new RandomGaussian();
    
    private Rigidbody rb;
    
    private bool onGround = false;
    private Vector3 groundNormal;
    private const int MaxContactPoint = 4;
    private ContactPoint[] contactPoints = new ContactPoint[MaxContactPoint];
    
    private float depth = 0f;
    public float Depth
    {
        get => depth;
        private set => depth = Mathf.Clamp(value, 0.0f, maxDepth);
    }

    [Header("Depth To Drag Conversion parameters")]
    [SerializeField, Min(0f)] private float maxDepth = 0.3f;
    [SerializeField, Min(0f)] private float dragMultiplier = 1f;
    [SerializeField] private AnimationCurve depthToDrag;
    
    [Header("Velocity To Sink Rate Conversion parameters")]
    [SerializeField, Min(0f)] private float velocityAtMaxSinkRate = 10f;
    [SerializeField, Min(0f)] private float maxSinkRate = 0.1f;
    [SerializeField, Min(0f)] private float maxSinkRateStandardDev = 0.025f;
    [SerializeField] private AnimationCurve velocityToSinkRate;
    
    [Header("Ground Impact parameters")]
    [SerializeField, Min(0f)] private float maxDepthFromImpact = 0.2f;
    [SerializeField, Min(0f)] private float maxDepthFromImpactStandardDev = 0.05f;
    [SerializeField, Min(0f)] private float impactVelocityForMaxDepth = 10f;
    
    [Header("Other Impact parameters")]
    [SerializeField, Min(0f)] private float impulseAtMaxDislogement = 5f;
    [SerializeField, Min(0f)] private float maxDislogement = 0.2f;
    [SerializeField, Min(0f)] private float maxDislogementStandardDev = 0.05f;
    [SerializeField] private AnimationCurve impulseToDislogement;
    [SerializeField] private AnimationCurve angleToDislogement;

    [Header("Velocity Limit")]
    [SerializeField] private float velocityLimitParameter = 1f;
    
    private void HandleFriction()
    {
        float velocity = rb.linearVelocity.magnitude;
        float v = Mathf.Clamp01(velocity / velocityAtMaxSinkRate);
        float sinkRate = velocityToSinkRate.Evaluate(v);

        float randomFactor = rng.NextGaussian(maxSinkRate, maxSinkRateStandardDev);

        float sink = randomFactor * sinkRate * velocity * Time.deltaTime;
        
        // add randomness

        Depth += sink;
        
    }

    private void HandleGroundCollision(Collision other)
    {
        float angleFactor = 0f;
        
        int contactCount = Mathf.Min(other.GetContacts(contactPoints), MaxContactPoint);
        for (int it = 0; it < contactCount; it++)
        {
            ContactPoint contact = contactPoints[it];
            
            // 0 => grazing ; 1 => straight into sand
            float af = Vector3.Dot(contact.normal, other.relativeVelocity.normalized);
            angleFactor = Mathf.Max(af, angleFactor);
        }
        float normalVelocity= angleFactor * other.relativeVelocity.magnitude;
            
        float velocityFactor = Mathf.Clamp01(normalVelocity / impactVelocityForMaxDepth);
        float randomFactor = rng.NextGaussian(maxDepthFromImpact, maxDepthFromImpactStandardDev);
        float d = randomFactor * velocityFactor;
        
        Depth += d;
        
    }
    
    private void HandleOtherCollision(Collision other)
    {
        if (!onGround) { return; }
        
        ContactPoint contact = other.GetContact(0);

        // [-1, 1] :
        // -1 => the other ball comes from directly above
        // 0 => the other ball comes from the side
        // 1 => the other ball comes from directly below
        float impactAngle = Vector3.Dot(groundNormal, other.impulse);
        float angleFactor = angleToDislogement.Evaluate((impactAngle + 1f) * 0.5f) * 2f - 1f;

        float i = Mathf.Clamp01(other.impulse.magnitude / impulseAtMaxDislogement);
        float impulseFactor = impulseToDislogement.Evaluate(i);

        float randomFactor = rng.NextGaussian(maxDislogement, maxDislogementStandardDev);
        
        float d = randomFactor * angleFactor * impulseFactor;
        Depth -= d;
        
        // apply impulse after removing drag
        Vector3 impulse = contact.impulse;
        rb.AddForce(-impulse, ForceMode.Impulse);
        UpdateDamping();
        StartCoroutine(ReapplyImpulse(impulse));
    }

    private IEnumerator ReapplyImpulse(Vector3 impulse)
    {
        yield return new WaitForFixedUpdate();
        rb.AddForce(impulse, ForceMode.Impulse);
    }
 
    private void UpdateDamping()
    {
        float d = Mathf.Clamp01(Depth / maxDepth);
        float drag = depthToDrag.Evaluate(d);
        drag = Mathf.Tan(Mathf.PI * 0.4999f * drag);
        drag = dragMultiplier * Mathf.Clamp(drag, 0, float.PositiveInfinity);
        
        rb.linearDamping = drag;
        rb.angularDamping = drag;
        
    }
    

    private void Start()
    {
        if (!TryGetComponent(out rb))
        {
            Debug.Log($"No Rigidbody found on {gameObject}");
            enabled = false;
            return;
        }
    }
    
    private void FixedUpdate()
    {
        if (onGround)
        {
            HandleFriction();
            
        }
        
        UpdateDamping();
        
        // Stopping mechanism
        float velocity = rb.linearVelocity.magnitude;
        float limit = -Mathf.Log(1 - Depth / maxDepth) * velocityLimitParameter;
        if (velocity < limit)
        {
            rb.linearDamping = float.PositiveInfinity;
            rb.angularDamping = float.PositiveInfinity;
        }

    }

    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            if (!onGround)
            {
                onGround = true;
                HandleGroundCollision(other);
            }
        }
        else
        {
            HandleOtherCollision(other);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            groundNormal = Vector3.zero;
            int contactCount = Mathf.Min(other.GetContacts(contactPoints), MaxContactPoint);
            for (int it = 0; it < contactCount; it++)
            {
                ContactPoint contact = contactPoints[it];
                groundNormal += contact.normal;
            }

            groundNormal /= contactCount;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            onGround = false;
            groundNormal = Vector3.zero;
            Depth = 0f;
        }
    }
    


    
}
