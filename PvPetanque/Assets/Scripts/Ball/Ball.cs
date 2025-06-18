using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    private Team team;
    public Team Team
    {
        get => team;
        set => team = value;
    }

    private bool hitGround = false;
    public bool HitGround
    {
        get => hitGround;
    }

    private bool inBounds = false;
    public bool InBounds
    {
        get => inBounds;
    }
    
    
    private void Start()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.Log("Ball has no rigidbody component.");
        }
    }

    public bool isMoving(float epsilon = 0.01f)
    {
        return hitGround && rb.linearVelocity.magnitude > epsilon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitGround && other.CompareTag("Ground"))
        {
            hitGround = true;
        }

        if (other.CompareTag("TerrainBounds"))
        {
            inBounds = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TerrainBounds"))
        {
            inBounds = false;
        }
    }
}
