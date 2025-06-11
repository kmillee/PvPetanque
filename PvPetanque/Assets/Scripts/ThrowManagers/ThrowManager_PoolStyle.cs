using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class ThrowManager_PoolStyle : ThrowManager
{
    [SerializeField] private CameraMovementManager cameraManager;

    [Header("AimingSettings")] 
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Vector2 aimingBounds = new Vector2(60f, 45f);

    
    
    private Vector2 _mouseLookAbsolute;
    
    
    
    
    protected override void SetUpBall(GameObject ball)
    {
        
    }

    protected override IEnumerator BallThrowSequence(GameObject ball)
    {
        yield return AimingStage(ball);

        yield return HittingStage(ball);
    }

    private IEnumerator AimingStage(GameObject ball)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        for (;;)
        {
            if (Input.GetMouseButtonDown(0)) { break; }
            
            // Compute new angle
            _mouseLookAbsolute += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
            Vector2 homogeneousMouseLook = _mouseLookAbsolute / aimingBounds;
            Vector2 mouseLook = aimingBounds * (2f * homogeneousMouseLook.Sigmoid() - Vector2.one);
            
            // Adjust camera
            
            yield return null;
            
            //
            break;
        }
        
        
        
    }

    private IEnumerator HittingStage(GameObject ball)
    {
        yield break;
    }




    
}
