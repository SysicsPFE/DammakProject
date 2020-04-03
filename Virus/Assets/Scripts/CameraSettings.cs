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
        bool isAiming = Input.GetKey(InputManager.leftMouseButton);
        SetAimAngleTo(isAiming);
    }

    #endregion

    #region custom methods

    private void SetAimAngleTo(bool isAiming)
    {
        int highPriority = 1;
        int lowPriority = 0;
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
