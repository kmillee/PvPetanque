using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Team _team;
    public Team Team
    {
        get => _team;
        set => _team = value;
    }

    private Rigidbody _rb;
    
    private bool _isThrown = false;
    public bool IsThrown
    {
        set => _isThrown = value;
    }
    
    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out _rb))
        {
            Debug.Log("Ball has no rigidbody component.");
        }
    }

    public bool isMoving(float epsilon = 0.01f)
    {
        return !_isThrown || _rb.linearVelocity.magnitude > epsilon;
    }
}
