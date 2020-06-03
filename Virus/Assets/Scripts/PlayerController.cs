using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    
    #region variables
        #region private variables

        private Animator _playerAnimator;
        private Transform _playerCamera;
        private CharacterController _playerController;
         
        [SerializeField] private float _playerSpeed = 1;

        #endregion

        #region constent variables

        private const float PlayerRotationSensitivity = 7f;
        private const float ConstantSpeed = 100;

        #endregion

    #endregion

    #region buildin methods

    void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerCamera = GameObject.FindWithTag("MainCam").GetComponent<Camera>().transform;
        _playerController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveOnXAxis = InputManager.rightLeftArrowKeys;
        float moveOnYAxis = InputManager.upDownArrowKeys;
        bool aimingState = Input.GetKey(InputManager.rightMouseButton);
        bool shootingState = Input.GetKey(InputManager.leftMouseButton);
        bool normalState = (!aimingState && !shootingState);
        Vector2 WASDinput = new Vector2(moveOnXAxis, moveOnYAxis);

        MoveIn8Directions(WASDinput);
        SetPLayerStatus(normalState, aimingState, shootingState);
        SetPlayerSpeedAccordingToSituation(aimingState, shootingState);

        if (WASDinput != Vector2.zero || !normalState)
            LookForward();
        if (!normalState)
        {
            _playerAnimator.SetFloat("Angle",LookUpDownAngle());
            UiManager.CrossHairStatus(true);
        }
        else
        {   
            _playerAnimator.SetFloat("Angle",0);
            UiManager.CrossHairStatus(WASDinput == Vector2.zero);
        }
    }

    #endregion
    
    #region custom methods

    private void LookForward()
    {
        Vector3 playerCameraEulerAngles = _playerCamera.eulerAngles;
        Quaternion turnAngle = Quaternion.Euler(0,playerCameraEulerAngles.y, 0);
        transform.rotation =
            Quaternion.Slerp(transform.rotation, turnAngle, Time.deltaTime * PlayerRotationSensitivity);
    }

    private void MoveIn8Directions(Vector2 movementInput)
    {
        _playerAnimator.SetFloat("xAxis",movementInput.x);
        _playerAnimator.SetFloat("yAxis",movementInput.y);
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        _playerController.Move(move * _playerSpeed);
    }

    private void SetPLayerStatus(bool isNormal ,bool isAiming, bool isShooting)
    {
        if (isNormal)
        {
            _playerAnimator.SetBool("isAiming", false);
            _playerAnimator.SetBool("isShooting", false);
        }
        else
        {
            _playerAnimator.SetBool("isAiming", isAiming);
            _playerAnimator.SetBool("isShooting", isShooting);
        }
    }

    private void SetPlayerSpeedAccordingToSituation(bool isAiming, bool isShooting)
    {
        if (isAiming)
            _playerSpeed = (ConstantSpeed * Time.deltaTime) / 5;
        else if (isShooting)
            _playerSpeed = (ConstantSpeed * Time.deltaTime) / 3;
        else
            _playerSpeed = ConstantSpeed * Time.deltaTime;
    }

    private float LookUpDownAngle()
    {
        CinemachineFreeLook playerCinemachineCamera =
            GameObject.FindWithTag("VirtualMainCamera").GetComponent<CinemachineFreeLook>();
        float angle = Mathf.Lerp(-50, 90, playerCinemachineCamera.m_YAxis.Value);
        return (angle);
    }

    #endregion
    
}
