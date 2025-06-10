using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

// TODO: implement user inputs
public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Rigidbody _rigidbody;
    
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
    
    private Vector3 _targetPosition;
    public Vector3 TargetPosition
    {
        get => _targetPosition;
        set
        {
            Debug.Log($"Target position = {value}");
            _targetPosition = value;
        }
    }

    private Quaternion _targetRotation;
    public Quaternion TargetRotation
    {
        get => _targetRotation;
        set => _targetRotation = value;
    }

    private float _targetFov;
    public float TargetFov
    {
        get => _targetFov;
        set => _targetFov = value;
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        TargetFov = _camera.fieldOfView;
        TargetPosition = transform.position;
        TargetRotation = transform.rotation;
    }
    
    void Update()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
        UpdateFOV();
    }

    void UpdateCameraPosition()
    {
        Vector3 translation = TargetPosition - transform.position;
        float displacement = translation.magnitude;
        if (displacement < minimumTranslationSpeed * Time.deltaTime)
        {
            _rigidbody.MovePosition(TargetPosition);
        }
        else
        {
            _rigidbody.MovePosition(transform.position + translation.normalized * (Time.deltaTime * Mathf.Max(SpeedMultiplier * positionRelaxingSpeed * displacement, minimumTranslationSpeed)));
        }
    }
    
    void UpdateCameraRotation()
    {
        float totalAngle = Quaternion.Angle(transform.rotation, TargetRotation);
        if (totalAngle < minimumRotationSpeed * Time.deltaTime)
        {
            _rigidbody.MoveRotation(TargetRotation);
        }
        else
        {
            float t = SpeedMultiplier * rotationRelaxingSpeed * Time.deltaTime;
            float angle = totalAngle * t;
            _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, TargetRotation, Mathf.Max(t, minimumRotationSpeed * Time.deltaTime / angle)));
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
