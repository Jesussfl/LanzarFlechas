using UnityEngine;
using Cinemachine;

public class CameraActions : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _1stPersonCamera;
    [SerializeField] private CinemachineFreeLook _3rdPersonCamera;
    [SerializeField] private CinemachineVirtualCamera _aimingCamera;

    private int firstPersonCamera = 0;
    private int thirdPersonCamera = 1;
    private int aimingCamera = 2;

    private void ResetPriorities()
    {
        _1stPersonCamera.Priority = 0;
        _3rdPersonCamera.Priority = 0;
        _aimingCamera.Priority = 0;
    }

    public void SwitchCamera(int IdCamera)
    {
        ResetPriorities();

        if (IdCamera == firstPersonCamera) _1stPersonCamera.Priority = 1;

        if (IdCamera == thirdPersonCamera) _3rdPersonCamera.Priority = 1;

        if (IdCamera == aimingCamera) _aimingCamera.Priority = 1;

    }
}
