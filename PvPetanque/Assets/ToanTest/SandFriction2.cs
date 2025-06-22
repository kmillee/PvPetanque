using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class SandFriction2 : MonoBehaviour
{
    private static readonly System.Random rng = new System.Random();
    
    private Rigidbody rb;
    
    private bool onGround = false;
    private const int MaxContactPoint = 4;
    private ContactPoint[] contactPoints = new ContactPoint[MaxContactPoint];

    private float depth;
    public float Depth
    {
        get => depth;
        private set => depth = Mathf.Max(0.0f, value);
    }

    private float sinkingSpeed;
    
    [Header("Depth to Drag Conversion parameters")]
    [SerializeField, Min(0f)] private float maxDepth = 0.3f;
    [SerializeField, Min(1f)] private float depthToDragCoef = 1f;

    [Header("Sinking parameters")] 
    [SerializeField, Min(0f)] private float maxSinkingRate = 0.2f;
    [SerializeField, Min(0f)] private float maxSinkingrateStandardDev = 0.05f;
    [SerializeField, Min(0f)] private float velocityForMaxSinkingRate = 2f;

    [Header("Ground Impact parameters")]
    [SerializeField, Min(0f)] private float maxDepthFromImpact = 0.2f;
    [SerializeField, Min(0f)] private float maxDepthFromImpactStandardDev = 0.05f;
    [SerializeField, Min(0f)] private float impactVelocityForMaxDepth = 10f;
    
    //
    // private void Start()
    // {
    //     if (!TryGetComponent(out rb))
    //     {
    //         Debug.Log($"No Rigidbody found on {gameObject}");
    //         enabled = false;
    //         return;
    //     }
    //
    //     Depth = 0f;
    // }
    //
    // private void OnEnable()
    // {
    //     SpdvcWarpSetup();
    // }
    //
    // private void FixedUpdate()
    // {
    //     if (onGround)
    //     {
    //         // float randomIntensity = NextGaussian(sinkingPerDistance,sinkingPerDistanceStandardDev);
    //         // float warpedDistance = SpdvcWarp(rb.linearVelocity.magnitude);
    //         Depth += warpedDistance * randomIntensity * Time.deltaTime;
    //     }
    //     else
    //     {
    //         Depth *= sinkingSpeedDecay;
    //     }
    //     
    //     ClampDepth();
    //     UpdateDamping();
    //     
    //     Debug.Log(Depth);
    // }
    //
    //
    //
    //
    //
    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.collider.CompareTag("Ground"))
    //     {
    //         onGround = true;
    //         HandleGroundCollision(other);
    //     }
    //     else
    //     {
    //         HandleOtherCollision(other);
    //     }
    // }
    //
    // private void OnCollisionExit(Collision other)
    // {
    //     if (other.collider.CompareTag("Ground"))
    //     {
    //         onGround = false;
    //         Depth = 0f;
    //     }
    // }
    //
    //    
    // private void HandleGroundCollision(Collision other)
    // {
    //     float angleFactor = 0f;
    //     
    //     int contactCount = Mathf.Min(other.GetContacts(contactPoints), MaxContactPoint);
    //     for (int it = 0; it < contactCount; it++)
    //     {
    //         ContactPoint contact = contactPoints[it];
    //         
    //         // 0 => grazing ; 1 => straight into sand
    //         angleFactor += Vector3.Dot(contact.normal, other.relativeVelocity.normalized);
    //     }
    //     angleFactor /= contactCount;
    //     float normalVelocity= angleFactor * other.relativeVelocity.magnitude;
    //         
    //     float velocityFactor = Mathf.Clamp01(normalVelocity / impactVelocityForMaxDepth);
    //     float randomFactor = NextGaussian(maxDepthFromImpact, maxDepthFromImpactStandardDev);
    //     
    //     Depth += randomFactor * velocityFactor;
    // }
    //
    // private void HandleOtherCollision(Collision other)
    // {
    // }
    //
    //
    // private void ClampDepth()
    // {
    //     Depth = Mathf.Clamp(Depth, 0, maxDepth);
    // }
    //
    // private void UpdateDamping()
    // {
    //     float d = Depth / maxDepth;
    //     d = Mathf.Pow(d, depthToDragCoef);
    //     d = Mathf.Tan(Mathf.PI * 0.4999f * d);
    //     d = Mathf.Clamp(d, 0, 1000f);
    //
    //     rb.linearDamping = d;
    //     rb.angularDamping = d;
    //     
    // }
    //
    //
    //
    //
    //
    //
    // private static bool _hasSpareValue = false;
    // private static double _spareValue; 
    // private static float NextGaussian(double mean = 0, double stdDev = 1)
    // {
    //     
    //     if (_hasSpareValue)
    //     {
    //         _hasSpareValue = false;
    //         return (float) (mean + stdDev * _spareValue);
    //     }
    //
    //     double u, v, s;
    //     do
    //     {
    //         u = 2.0 * rng.NextDouble() - 1.0;
    //         v = 2.0 * rng.NextDouble() - 1.0;
    //         s = u * u + v * v;
    //     } while (s >= 1.0 || s == 0);
    //
    //     s = Math.Sqrt(-2.0 * Math.Log(s) / s);
    //     _spareValue = v * s;
    //     _hasSpareValue = true;
    //     
    //     // Clamping within 3 stdDev
    //     double min = mean - 3 * stdDev;
    //     double max = mean + 3 * stdDev;
    //
    //     return (float) Math.Clamp(mean + stdDev * (u * s), min, max);
    // }
    

}
