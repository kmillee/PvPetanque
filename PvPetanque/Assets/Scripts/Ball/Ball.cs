using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    private Coroutine checkBoundsCoroutine;

    public Team Team { get; set; }
    public bool Launched { get; private set; } = false;
    public bool HitGround { get; private set; } = false;
    public bool InBounds { get; private set; } = true;
    public bool IsDisqualified { get; private set; } = false;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.Log("Ball has no rigidbody component.");
        } 
    }

    private void OnDestroy()
    {
        if(Launched) { StopCoroutine(checkBoundsCoroutine); }
    }

    public void Launch()
    {
        Launched = true;
        checkBoundsCoroutine = StartCoroutine(CheckBounds());
    }

    private IEnumerator CheckBounds()
    {
        for(;;)
        {
            // Make sure the ball stay in bounds
            int boundsTest = 0;
            while (!InBounds)
            {
                boundsTest++;

                if (boundsTest >= 4)
                {
                    Debug.Log("out of bounds");
                    Disqualify();
                    yield break;
                }

                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public bool IsMoving(float epsilon = 0.01f)
    {
        return rb.linearVelocity.magnitude > epsilon;
    }

    private void OnTriggerEnter(Collider other)
    {
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
    
    private void OnCollisionEnter(Collision other)
    {
        if (!HitGround && other.collider.CompareTag("Ground"))
        {
            HitGround = true;
        }
    }
    
    public void Disqualify() 
    {
        Debug.Log($"Ball {gameObject.name} has been disqualified.");
        IsDisqualified = true;
        GameManager.instance.OnBallDisqualified(this);
    }
}
