using System;
using Unity.Mathematics;
using UnityEngine;

public class ball_controller_test : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startingPosition;

    private void Start()
    {
        TryGetComponent(out rb);
        startingPosition = transform.position;
    }


    private void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            transform.position = startingPosition;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
