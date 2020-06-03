using Cinemachine;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    #region variables

        #region private variables

        private CinemachineFreeLook _playerCinemachineCamera;
        private CinemachineFreeLook _playerAimCinemachineCamera;

        #endregion

    #endregion

    #region buildin methods

    void Awake()
    {
        _playerCinemachineCamera = GameObject.FindWithTag("VirtualMainCamera").GetComponent<CinemachineFreeLook>();
        _playerAimCinemachineCamera = GameObject.FindWithTag("VirtualAimCamera").GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        SetActiveCamera();
    }

    #endregion

    #region custom methods

    private void SetActiveCamera()
    {
        const int highPriority = 1;
        const int lowPriority = 0;
        bool isAiming = Input.GetKey(InputManager.rightMouseButton);
        if (isAiming)
        {
            _playerCinemachineCamera.Priority = lowPriority;
            _playerAimCinemachineCamera.Priority = highPriority;
            _playerCinemachineCamera.m_YAxis.Value = _playerAimCinemachineCamera.m_YAxis.Value;
            _playerCinemachineCamera.m_XAxis.Value = _playerAimCinemachineCamera.m_XAxis.Value;
        }
        else
        {
            _playerCinemachineCamera.Priority = highPriority;
            _playerAimCinemachineCamera.Priority = lowPriority;
            _playerAimCinemachineCamera.m_YAxis.Value = _playerCinemachineCamera.m_YAxis.Value;
            _playerAimCinemachineCamera.m_XAxis.Value = _playerCinemachineCamera.m_XAxis.Value;
        }
    }

    #endregion
    
}
