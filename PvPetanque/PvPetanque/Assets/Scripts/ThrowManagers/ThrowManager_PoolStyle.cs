using System.Collections;
using System.Numerics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ThrowManager_PoolStyle : ThrowManager
{
    // Indicators
    [SerializeField] private GameObject indicatorsPrefab;
    private GameObject indicator;
    private CalibratingIndicator calibratingIndicator;
    private Renderer springRenderer;
    
    [Header("AimingSettings")] 
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Vector2 aimingBounds = new Vector2(60f, 45f);
    
    [Header("Cameras")] 
    [SerializeField] private CinemachineCamera aimingCamera;
    [SerializeField] private CinemachineCamera hittingCamera;
    [SerializeField] private CinemachineCamera flightCamera;
    
    // Aiming stage
    private Vector2 mouseLookAbsolute;
    
    // Hitting stage
    private float hitStrength;
    [Header("Hit stage variables")]
    [SerializeField] private float springConstant = 1f;
    [SerializeField] private float extendingPower = 1f;
    [SerializeField] private float maxHitDuration = 1f;
    [SerializeField] private Gradient springColor;
    
    // Flight stage
    [SerializeField] private float maxTimer = 30f;
    
    protected override void SetUpThrow()
    {
        // Indicators set up
        indicator = Instantiate(indicatorsPrefab, CurrentBall.transform);
        if (indicator.TryGetComponent<CalibratingIndicator>(out calibratingIndicator))
        {
            calibratingIndicator.Display(false);
        }
        else
        {
            Debug.Log("No calibratingIndicator found in indicators prefab.");
            return;
        }

        if (!calibratingIndicator.GetSpring().TryGetComponent(out springRenderer))
        {
            Debug.Log("No Renderer found on spring in indicator prefab.");
        }
        
        // Cameras set up
        aimingCamera.Follow = CurrentBall.transform;
        
        hittingCamera.Follow = calibratingIndicator.GetTip().transform;
        hittingCamera.LookAt = CurrentBall.transform;

        flightCamera.LookAt = CurrentBall.transform;

    }

    protected override void CleanUpThrow()
    {
        Destroy(indicator);

        aimingCamera.Follow = null;
        
        hittingCamera.Follow = null;
        hittingCamera.LookAt = null;
    }

    protected override IEnumerator BallThrowSequence()
    {
        yield return AimingStage();

        yield return ReleasingStage();

        yield return FlightStage();
    }


    private IEnumerator AimingStage()
    {
        mouseLookAbsolute = Vector2.zero;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        aimingCamera.Priority = 10;

        for (;;)
        {
            // Left click to start hitting
            if (Input.GetMouseButtonDown(0)) { break; }

            // Right click to reset orientation
            if (Input.GetMouseButtonDown(1)) { mouseLookAbsolute = Vector2.zero; }
            
            // Press space to unlock cursor
            if (Input.GetKeyDown("space"))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                do
                {
                    aimingCamera.Priority = useGlobalCamera ? 0 : 10;
                    yield return null;
                } while (!Input.GetKeyDown("space"));

                aimingCamera.Priority = 10;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            // Compute new angle
            mouseLookAbsolute += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
            Vector2 homogeneousMouseLook = mouseLookAbsolute / aimingBounds;
            Vector2 mouseLook = aimingBounds * (2f * homogeneousMouseLook.Sigmoid() - Vector2.one);
            
            // Adjust ball
            CurrentBall.transform.rotation = Quaternion.Euler(mouseLook.y, mouseLook.x, 0);
            
            yield return null;
        }

        aimingCamera.Priority = 0;
    }

    private IEnumerator ReleasingStage()
    {
        // Camera set up
        hittingCamera.Priority = 10;
        calibratingIndicator.Display(true);
        
        hitStrength = 0f;
        float effectiveSpringConstant = springConstant * CurrentBallRb.mass;
        float effectiveExtendingPower = extendingPower * CurrentBallRb.mass;

        float maxLength = Mathf.Sqrt(2f * maxHitDuration * effectiveExtendingPower / effectiveSpringConstant);
        
        // Wait for button release
        float start = Time.time;
        float springEnergy = 0f;
        float extendedSpringLength = 0f;
        for (;;)
        {
            // Release right click to hit the ball
            if (Input.GetMouseButtonUp(0)) { break; }
            
            // Compute spring 
            float duration = Mathf.Min(Time.time - start, maxHitDuration);
            
            springEnergy = duration * effectiveExtendingPower;
            extendedSpringLength = Mathf.Sqrt(2f * springEnergy / effectiveSpringConstant);
            
            // Animation
            calibratingIndicator.UpdateLength(extendedSpringLength);
            springRenderer.material.color = springColor.Evaluate(extendedSpringLength / maxLength);
            
            yield return null;
        }
        
        // Animation - Release spring
        float angularFrequency = Mathf.Sqrt(effectiveSpringConstant / CurrentBallRb.mass);
        float releasedSpringLength = extendedSpringLength;
        start = Time.time;
        for(int it = 0; releasedSpringLength > 0f && it < 1000; it++)
        {
            // Compute new spring length
            float duration = Time.time - start;
            releasedSpringLength = Mathf.Max(0.0f, extendedSpringLength * Mathf.Cos(angularFrequency * duration));

            // Animation
            calibratingIndicator.UpdateLength(releasedSpringLength);
            springRenderer.material.color = springColor.Evaluate(releasedSpringLength / maxLength);

            yield return null;
        }
        
        // Compute hit
        hitStrength = Mathf.Sqrt(effectiveSpringConstant * CurrentBallRb.mass) * extendedSpringLength;

        hittingCamera.Priority = 0;
        calibratingIndicator.Display(false);

    }

    private IEnumerator FlightStage()
    {
        flightCamera.Priority = 10;

        CurrentBallRb.useGravity = true;
        Vector3 hitDirection = CurrentBall.transform.rotation * Vector3.forward;
        CurrentBallRb.AddForce(hitStrength * hitDirection, ForceMode.Impulse);
        CurrentBallScript.Launch();

        // Wait for the force to be applied
        while (CurrentBallRb.linearVelocity == Vector3.zero)
        {
            yield return null;
        }

        
        // Wait for the ball to hit the ground
        float flightStart = Time.time;
        while (!CurrentBallScript.HitGround)
        {
            if (Time.time - flightStart >= maxTimer)
            {
                Debug.Log("out of time");
                CurrentBallScript.Disqualify();
                yield return new WaitForSeconds(0.5f);
                break;
            }

            if (CurrentBallScript.IsDisqualified)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
            
            yield return null;
        }
        
        flightCamera.Priority = 0;
        
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    
}
