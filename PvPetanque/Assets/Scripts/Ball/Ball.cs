using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    public Team Team { get; set; }
    public bool Launched { get; private set; } = false;
    public bool HitGround { get; private set; } = false;
    public bool InBounds { get; private set; } = true;


    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.Log("Ball has no rigidbody component.");
        } 
    }

    public void Launch()
    {
        Launched = true;
    }

    public bool IsMoving(float epsilon = 0.01f)
    {
        return rb.linearVelocity.magnitude > epsilon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HitGround && other.CompareTag("Ground"))
        {
            HitGround = true;
        }

        if (other.CompareTag("TerrainBounds"))
        {
            InBounds = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TerrainBounds"))
        {
            InBounds = false;
        }
    }
    
    public void Disqualify() 
    {
        Debug.Log($"Ball {gameObject.name} has been disqualified.");
        GameManager.instance.OnBallDisqualified(this);
    }
}
