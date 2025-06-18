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

    private bool isCurrentlyMoving = false;
    private bool isDisqualified = false;
    private float timer = 0f;
    [SerializeField] private float maxTimer = 30f;

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

    void Update() 
    {
        if(isDisqualified) return;

        if(isCurrentlyMoving) 
        {
            timer += Time.deltaTime;
            Debug.Log($"Ball {gameObject.name} is moving. Timer: {timer:F2}s");
            if(!isMoving() && timer > 0.1f) 
            {
                isCurrentlyMoving = false;
                timer = 0f; // Reset timer when the ball stops moving
                Debug.Log($"Ball {gameObject.name} has stopped moving.");
            }

            if (timer >= maxTimer) 
            {
                Disqualify();
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null) 
                {
                    gm.OnBallDisqualified(this);
                } 
                else 
                {
                    Debug.LogWarning("GameManager not found. Cannot call OnBallDisqualified.");
                }
            }
        }   

        if(!isCurrentlyMoving && isMoving()) 
        {
            Debug.Log($"Ball {gameObject.name} has started moving.");
            isCurrentlyMoving = true; 
            timer = 0f; // Reset timer when the ball starts moving
        }
    }

    public void StartThrownTimer() 
    {
        isCurrentlyMoving = true;
        timer = 0f; 
    }

    private void Disqualify() 
    {
        isDisqualified = true;
        Debug.Log($"Ball {gameObject.name} has been disqualified.");
    }
}
