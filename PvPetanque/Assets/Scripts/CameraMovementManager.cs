using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

// TODO: implement user inputs
public class CameraMovementManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Rigidbody _cameraRigidbody;
    [SerializeField] private Transform _cameraPivotTransform;
    [SerializeField] private Rigidbody _cameraPivotRigidbody;
    
    [SerializeField] private float positionRelaxingSpeed = 5.0f;
    [SerializeField] private float minimumTranslationSpeed = 0.5f;
    
    [SerializeField] private float rotationRelaxingSpeed = 5.0f;
    [SerializeField] private float minimumRotationSpeed = 0.5f;

    [SerializeField] private float fovRelaxingSpeed = 5.0f;
    [SerializeField] private float minimumFovSpeed = 0.5f;

    private float _speedMultiplier = 1.0f;
    public float SpeedMultiplier
    {
        get => _speedMultiplier;
        set => _speedMultiplier = value;
    }
    
    [SerializeField] private Vector3 _targetPosition;
    public Vector3 TargetPosition
    {
        get => _targetPosition;
        set => _targetPosition = value;
    }

    [SerializeField] private Quaternion _targetRotation;
    public Quaternion TargetRotation
    {
        get => _targetRotation;
        set => _targetRotation = value;
    }

    [SerializeField] private Quaternion _targetPivot;
    public Quaternion TargetPivot
    {
        get => _targetPivot;
        set => _targetPivot = value;
    }

    private float _targetFov;
    public float TargetFov
    {
        get => _targetFov;
        set => _targetFov = value;
    }

    void Awake()
    {
        _cameraRigidbody = GetComponent<Rigidbody>();

        TargetFov = _camera.fieldOfView;
        TargetPosition = transform.position;
        TargetRotation = transform.rotation;
    }
    
    void Update()
    {
        // UpdateCameraPosition();
        // UpdateRotation(_cameraRigidbody, TargetRotation);
        // UpdateRotation(_cameraPivotRigidbody, TargetPivot);
        // UpdateFOV();
    }

    void UpdateCameraPosition()
    {
        Vector3 translation = TargetPosition - transform.localPosition;
        float displacement = translation.magnitude;
        if (displacement < minimumTranslationSpeed * Time.deltaTime)
        {
            _cameraRigidbody.MovePosition(TargetPosition);
        }
        else
        {
            _cameraRigidbody.MovePosition(transform.position + translation.normalized * (Time.deltaTime * Mathf.Max(SpeedMultiplier * positionRelaxingSpeed * displacement, minimumTranslationSpeed)));
        }
    }

    void UpdateRotation(Rigidbody rb, Quaternion target)
    {
        float totalAngle = Quaternion.Angle(rb.rotation, target);
        if (totalAngle < minimumRotationSpeed * Time.deltaTime)
        {
            rb.MoveRotation(target.normalized);
        }
        else
        {
            float t = SpeedMultiplier * rotationRelaxingSpeed * Time.deltaTime;
            float angle = totalAngle * t;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, Mathf.Max(t, minimumRotationSpeed * Time.deltaTime / angle)).normalized);
        }
    }

    void UpdateFOV()
    {
        float translation = TargetFov - _camera.fieldOfView;
        float distance = Mathf.Abs(translation);
        if (distance < minimumFovSpeed * Time.deltaTime)
        {
            _camera.fieldOfView = TargetFov;
        }
        else
        {
            _camera.fieldOfView += Mathf.Sign(translation) * Time.deltaTime * Mathf.Max(SpeedMultiplier * fovRelaxingSpeed * distance, minimumFovSpeed);
        }
        
    }

}
