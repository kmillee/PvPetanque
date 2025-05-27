
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class BallManager_test1 : MonoBehaviour
{
    private Coroutine _ballThrowCoroutine;
    
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private GameObject ballPrefab;
    private GameObject _ball;
    private Rigidbody _ballRb;
    
    // Different stages indicator
    private AimingIndicator _aimingIndicator;
    private CalibratingIndicator _calibratingIndicator;
    private ReleasingIndicator _releasingIndicator;
    
    // Camera manager
    [SerializeField] private CameraManager cameraManager;
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


    private void Start()
    {
        Debug.Log("Press r to reset");
    }

    private void Update()
    {
        // reset
        if (Input.GetKeyDown("r"))
        {
            Debug.Log("reset");
            if(_ballThrowCoroutine is not null) StopCoroutine(_ballThrowCoroutine);
            _ballThrowCoroutine = StartCoroutine(BallThrowSequence());
        }
        
    }

    private IEnumerator BallThrowSequence()
    {
        // Debug.Log("Starting turn.");
        
        CreateBall();
        
        // Debug.Log("Aim (press space).");
        yield return AimingStage();

        // Debug.Log("Calibrate strength (press and then release space).");
        yield return CalibratingStage();

        // Debug.Log("Release the ball (press and then release space).");
        yield return ReleasingStage();
        
        // Debug.Log("Wait for ball to stop.");
        yield return FlightStage();
    }
    
    private void CreateBall()
    {
        if (_ball) { Destroy(_ball); }
        _ball = Instantiate(ballPrefab, transform);
        _ball.transform.position = startingPosition;
        
        if (_ball.TryGetComponent<Rigidbody>(out _ballRb))
        {
            _ballRb.useGravity = false;
        }
        else
        {
            Debug.Log("No RigidBody found in ball prefab.");
            return;
        }

        if (_ball.TryGetComponent<AimingIndicator>(out _aimingIndicator))
        {
            _aimingIndicator.Display(false);
        }
        else
        {
            Debug.Log("No aimingIndicator found in ball prefab.");
            return;
        }
        
        if (_ball.TryGetComponent<CalibratingIndicator>(out _calibratingIndicator))
        {
            _calibratingIndicator.Display(false);
        }
        else
        {
            Debug.Log("No calibratingIndicator found in ball prefab.");
            return;
        }
        
        if (_ball.TryGetComponent<ReleasingIndicator>(out _releasingIndicator))
        {
            _releasingIndicator.Display(false);
            _releasingIndicator.setLocalLength(distanceToAnchor);
        }
        else
        {
            Debug.Log("No releasingIndicator found in ball prefab.");
            return;
        }
    }

    private IEnumerator AimingStage()
    {
        // Setting up camera and indicators
        cameraManager.TargetPosition = startingPosition + aimingCameraPosition;
        cameraManager.TargetRotation = aimingCameraRotation;
        _aimingIndicator.Display(true);
        cameraManager.TargetFov = cameraManager.startingFov;
        
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
            _ball.transform.rotation = Quaternion.Euler(0, _aimAngle, 0);
            
            yield return null;
        }
        
        // Reset indicators
        _aimingIndicator.Display(false);
    }

    private IEnumerator CalibratingStage()
    {
        // Setting up ball
        _ball.transform.rotation = Quaternion.Euler(-startingReleaseAngle, _aimAngle, 0);
        
        // Setting up camera and indicators
        _calibratingIndicator.UpdateLength(0.0f);   
        _releasingIndicator.Display(true);
        cameraManager.TargetPosition = startingPosition + Quaternion.Euler(0,_aimAngle,0) * calibratingCameraStartingPosition;
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
        cameraManager.TargetPosition = startingPosition + Quaternion.Euler(0,_aimAngle,0) * calibratingCameraStartingPosition;
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
            cameraManager.TargetPosition = startingPosition + Quaternion.Euler(0,_aimAngle,0) * Vector3.Lerp(calibratingCameraStartingPosition, calibratingCameraMaxStrengthPosition, t);
            cameraManager.TargetRotation = Quaternion.Euler(0,_aimAngle,0) * Quaternion.Slerp(calibratingCameraStartingRotation, calibratingCameraMaxStrengthRotation, t);
            cameraManager.TargetFov = Mathf.Lerp(calibratingCameraStartingFOV, calibratingCameraMaxStrengthFOV, t);
            
            if(Input.GetKeyUp("space")) { break; }
            yield return null;
        }
        _initialVelocity *= velocityAtMax; // in [0, velocityAtMax]

        // Spring Animation
        float springVelocity = 0.0f;
        float w0 = springConstant / _ballRb.mass;
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
            cameraManager.TargetPosition = startingPosition + Quaternion.Euler(0,_aimAngle,0) * Vector3.Lerp(calibratingCameraMaxStrengthPosition, calibratingCameraFinalPosition, t);
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
        // Setting up camera and indicators
        _releasingIndicator.Display(true);
        
        // Waiting for key press
        Vector3 anchorPosition = _releasingIndicator.getPosition();
        float l = (anchorPosition - _ball.transform.position).magnitude;
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
            _ballRb.MovePosition(anchorPosition + l * (Quaternion.Euler(-_releaseAngle, _aimAngle, 0) * Vector3.down));
            _ballRb.MoveRotation(Quaternion.Euler(-_releaseAngle, _aimAngle, 0));
            
            // Camera management
            cameraManager.TargetPosition = startingPosition + releasingCameraPosition;
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
        _ballRb.useGravity = true;
        _ballRb.linearVelocity = _ball.transform.rotation * Vector3.forward * _initialVelocity;
        
        cameraManager.SpeedMultiplier = 1.0f;
        cameraManager.TargetPosition = flightCameraPosition;

        while (_ballRb.linearVelocity.magnitude > 0.001f && _ball.transform.position.y > 0)
        {
            Vector3 lookDirection = _ball.transform.position - cameraManager.TargetPosition;
            float distance = lookDirection.magnitude;
            cameraManager.TargetRotation = Quaternion.LookRotation(lookDirection) * flightCameraRotation;
            cameraManager.TargetFov = 120.0f / Mathf.Pow(distance, ballFollowingFovExponent);
            yield return null;
        }

        cameraManager.TargetFov = 60.0f;
        cameraManager.SpeedMultiplier = 1.0f;
    }
    
    
    
    
}
