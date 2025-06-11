
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class ThrowManager_v1 : ThrowManager
{
    // indicators
    [SerializeField] private GameObject _indicatorsPrefab;
    private AimingIndicator _aimingIndicator;
    private CalibratingIndicator _calibratingIndicator;
    private ReleasingIndicator _releasingIndicator;
    
    // Camera manager
    [SerializeField] private CameraMovementManager cameraManager;
    private float baseFOV;
    
    // Aiming camera
    [SerializeField] private Vector3 aimingCameraPosition;
    [SerializeField] private Quaternion aimingCameraRotation;
    
    // Aiming specs
    private float _aimAngle;
    [SerializeField] private float minAimAngle, maxAimAngle;
    [SerializeField] private float aimSpeed;
    
    // Calibrating cameras
    [SerializeField] private Vector3 calibratingCameraStartingPosition;
    [SerializeField] private Quaternion calibratingCameraStartingRotation;
    [SerializeField] private float calibratingCameraStartingFOV;
    [SerializeField] private Vector3 calibratingCameraMaxStrengthPosition;
    [SerializeField] private Quaternion calibratingCameraMaxStrengthRotation;
    [SerializeField] private float calibratingCameraMaxStrengthFOV;
    [SerializeField] private Vector3 calibratingCameraFinalPosition;
    [SerializeField] private Quaternion calibratingCameraFinalRotation;
    [SerializeField] private float calibratingCameraFinalFOV;
    
    // Calibrating specs
    private float _initialVelocity;
    [SerializeField] private float maxCalibratingDuration;
    [SerializeField] private float durationToVelocityExponent;
    [SerializeField] private float velocityAtMin;
    [SerializeField] private float velocityAtMax;
    [SerializeField] private float lengthAtMaxStrength;
    [SerializeField] private float springConstant;
    
    // Releasing camera
    [SerializeField] private Vector3 releasingCameraPosition;
    [SerializeField] private Quaternion releasingCameraRotation;
    
    // Releasing specs
    private float _releaseAngle;
    [SerializeField] private float startingReleaseAngle;
    [SerializeField] private float swingSpeed;
    [SerializeField] private float maxReleaseAngle;
    [SerializeField] private float distanceToAnchor;
    
    // Flight camera
    [SerializeField] private Vector3 flightCameraPosition;
    [SerializeField] private Quaternion flightCameraRotation;
    [SerializeField] private float ballFollowingFovExponent;

    
    protected override void SetUpBall(GameObject ball)
    {
        var indicators = Instantiate(_indicatorsPrefab, _currentBall.transform);

        if (indicators.TryGetComponent<AimingIndicator>(out _aimingIndicator))
        {
            _aimingIndicator.Display(false);
        }
        else
        {
            Debug.Log("No aimingIndicator found in indicators prefab.");
            return;
        }
        
        if (indicators.TryGetComponent<CalibratingIndicator>(out _calibratingIndicator))
        {
            _calibratingIndicator.Display(false);
        }
        else
        {
            Debug.Log("No calibratingIndicator found in indicators prefab.");
            return;
        }
        
        if (indicators.TryGetComponent<ReleasingIndicator>(out _releasingIndicator))
        {
            _releasingIndicator.Display(false);
            _releasingIndicator.setLocalLength(distanceToAnchor);
        }
        else
        {
            Debug.Log("No releasingIndicator found in indicators prefab.");
            return;
        }
    }
    
    protected override IEnumerator BallThrowSequence(GameObject ball)
    {
        yield return AimingStage();

        yield return CalibratingStage();

        yield return ReleasingStage();
        
        yield return FlightStage();
    }
    
    private IEnumerator AimingStage()
    {
        Debug.Log("Aiming Stage (press space)");
        
        // Setting up camera and indicators
        cameraManager.TargetPosition = _startingPosition + aimingCameraPosition;
        cameraManager.TargetRotation = aimingCameraRotation;
        _aimingIndicator.Display(true);
        
        // Wait for key press
        _aimAngle = Random.Range(minAimAngle, maxAimAngle);
        float direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
        for(;;)
        {
            if (Input.GetKeyUp("space")) { break; }
            
            _aimAngle += direction * aimSpeed * Time.deltaTime;
            if (_aimAngle < minAimAngle)
            {
                direction = 1.0f;
                _aimAngle = minAimAngle;
            }
            else if (_aimAngle > maxAimAngle)
            {
                direction = -1.0f;
                _aimAngle = maxAimAngle;
            }
            _currentBall.transform.rotation = Quaternion.Euler(0, _aimAngle, 0);

            yield return null;
        }
        
        // Reset indicators
        _aimingIndicator.Display(false);
    }

    private IEnumerator CalibratingStage()
    {
        Debug.Log("Calibrating Stage (hold space and release)");
        
        // Setting up ball
        _currentBall.transform.rotation = Quaternion.Euler(-startingReleaseAngle, _aimAngle, 0);
        
        // Setting up camera and indicators
        _calibratingIndicator.UpdateLength(0.0f);   
        _releasingIndicator.Display(true);
        cameraManager.TargetPosition = _startingPosition + Quaternion.Euler(0,_aimAngle,0) * calibratingCameraStartingPosition;
        cameraManager.TargetRotation = Quaternion.Euler(0,_aimAngle,0) * calibratingCameraStartingRotation;
        float originalFOV = cameraManager.TargetFov;
        cameraManager.TargetFov = calibratingCameraStartingFOV;
        
        // Wait for initial key press
        for(;;)
        {
            if (Input.GetKeyDown("space")) { break;}
            yield return null;
        }
        
        // Setting up camera and indicators
        _calibratingIndicator.Display(true);
        _calibratingIndicator.UpdateLength(0.0f);
        cameraManager.TargetPosition = _startingPosition + Quaternion.Euler(0,_aimAngle,0) * calibratingCameraStartingPosition;
        cameraManager.TargetRotation = Quaternion.Euler(0,_aimAngle,0) * calibratingCameraStartingRotation;
        cameraManager.TargetFov = calibratingCameraStartingFOV;
        cameraManager.SpeedMultiplier = 3.0f;
        
        // Wait for key release
        float start = Time.time;
        float springLength;
        for (;;)
        {
            // Compute initial velocity
            float advancement = Mathf.Min(1.0f, (Time.time - start) / maxCalibratingDuration);
            _initialVelocity = Mathf.Pow(advancement, durationToVelocityExponent); // in [0,1]

            // Animation & Camera movements
            springLength = lengthAtMaxStrength * _initialVelocity; // in [0, lengthAtMaxStrength]
            _calibratingIndicator.UpdateLength(springLength);
            float t = springLength / lengthAtMaxStrength;
            cameraManager.TargetPosition = _startingPosition + Quaternion.Euler(0,_aimAngle,0) * Vector3.Lerp(calibratingCameraStartingPosition, calibratingCameraMaxStrengthPosition, t);
            cameraManager.TargetRotation = Quaternion.Euler(0,_aimAngle,0) * Quaternion.Slerp(calibratingCameraStartingRotation, calibratingCameraMaxStrengthRotation, t);
            cameraManager.TargetFov = Mathf.Lerp(calibratingCameraStartingFOV, calibratingCameraMaxStrengthFOV, t);
            
            if(Input.GetKeyUp("space")) { break; }
            yield return null;
        }
        _initialVelocity *= velocityAtMax; // in [0, velocityAtMax]

        // Spring Animation
        float springVelocity = 0.0f;
        float w0 = springConstant / _currentBallRb.mass;
        while(springLength > 0.0f)
        {
            // "Physics Simulation" (may have to move it to FixedUpdate, but it works here so)
            // Semi Implicit Euler methods
            float springAcceleration = -w0 * springLength;
            springVelocity += springAcceleration * Time.deltaTime;
            springLength += springVelocity * Time.deltaTime;
            
            // Animation & Camera movements
            _calibratingIndicator.UpdateLength(springLength);
            float t = 1 - springLength / lengthAtMaxStrength;
            cameraManager.TargetPosition = _startingPosition + Quaternion.Euler(0,_aimAngle,0) * Vector3.Lerp(calibratingCameraMaxStrengthPosition, calibratingCameraFinalPosition, t);
            cameraManager.TargetRotation = Quaternion.Euler(0,_aimAngle,0) * Quaternion.Slerp(calibratingCameraMaxStrengthRotation, calibratingCameraFinalRotation, t);
            cameraManager.TargetFov = Mathf.Lerp(calibratingCameraMaxStrengthFOV, calibratingCameraFinalFOV, t);
            
            yield return null;
        }
        
        // Reset indicators
        _calibratingIndicator.Display(false);
        _releasingIndicator.Display(false);
        cameraManager.SpeedMultiplier = 1.0f;
        cameraManager.TargetFov = originalFOV;
    }

    private IEnumerator ReleasingStage()
    {
        Debug.Log("Releasing Stage (press space)");
        
        // Setting up camera and indicators
        _releasingIndicator.Display(true);
        
        // Waiting for key press
        Vector3 anchorPosition = _releasingIndicator.getPosition();
        float l = (anchorPosition - _currentBall.transform.position).magnitude;
        float w02 = -Physics.gravity.y / l;
        _releaseAngle = startingReleaseAngle;
        float angularVelocity = _initialVelocity;
        for (;;)
        {
            // "Physics Simulation" (may have to move it to FixedUpdate, but it works here so)
            // Semi Implicit Euler methods, it seems to work even for angle integration
            float angularAcceleration = -w02 * Mathf.Sin(_releaseAngle * Mathf.Deg2Rad) * swingSpeed;
            angularVelocity += angularAcceleration * Time.deltaTime;
            _releaseAngle += angularVelocity * Time.deltaTime;
            _currentBallRb.MovePosition(anchorPosition + l * (Quaternion.Euler(-_releaseAngle, _aimAngle, 0) * Vector3.down));
            _currentBallRb.MoveRotation(Quaternion.Euler(-_releaseAngle, _aimAngle, 0));
            
            // Camera management
            cameraManager.TargetPosition = _startingPosition + releasingCameraPosition;
            cameraManager.TargetRotation = Quaternion.Euler(0,_aimAngle,0) * releasingCameraRotation;

            if (Input.GetKeyDown("space") || _releaseAngle > maxReleaseAngle) { break; }
            yield return null;
        }

        // Store the velocity to apply at the release stage
        _initialVelocity = angularVelocity * Mathf.Deg2Rad * l;
        
        // Reset indicators
        _releasingIndicator.Display(false);
    }

    private IEnumerator FlightStage()
    {
        Debug.Log("Flight Stage");
        _currentBallRb.useGravity = true;
        _currentBallRb.linearVelocity = _currentBall.transform.rotation * Vector3.forward * _initialVelocity;
        
        cameraManager.SpeedMultiplier = 1.0f;
        cameraManager.TargetPosition = flightCameraPosition;

        while (_currentBallRb.linearVelocity.magnitude > 0.001f && _currentBall.transform.position.y > 0)
        {
            Vector3 lookDirection = _currentBall.transform.position - cameraManager.TargetPosition;
            float distance = lookDirection.magnitude;
            cameraManager.TargetRotation = Quaternion.LookRotation(lookDirection) * flightCameraRotation;
            cameraManager.TargetFov = 120.0f / Mathf.Pow(distance, ballFollowingFovExponent);
            yield return null;
        }

        cameraManager.TargetFov = 60.0f;
        cameraManager.SpeedMultiplier = 1.0f;
    }
    
}
