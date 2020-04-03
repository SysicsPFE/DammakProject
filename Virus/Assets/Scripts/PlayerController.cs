using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    
    #region variables
        #region private variables

        private Animator _playerAnimator;
        private Transform _playerCamera;
        private Transform _playerEyes;
        private CharacterController _playerController;

        private float _mouseY = 0;
        private float _playerLookUpDownSensitivity = 100;
         
        [SerializeField] private float _playerSpeed = 1;

        #endregion

        #region constent variables

        private const float PlayerRotationSensitivity = 1f;
        private const float ConstantSpeed = 100;
        

        #endregion

    #endregion
    
    #region buildin methods

    void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        _playerController = GetComponent<CharacterController>();
        _playerLookUpDownSensitivity *= Time.fixedDeltaTime;
        _playerEyes = GameObject.Find("eyes").transform;
    }

    void Update()
    {
        float moveOnXAxis = InputManager.rightLeftArrowKeys;
        float moveOnYAxis = InputManager.upDownArrowKeys;
        bool isAiming = Input.GetKey(InputManager.leftMouseButton);
        bool isShooting = Input.GetKey(InputManager.rightMouseButton);
        bool isNormal = (!isAiming && !isShooting);
        Vector2 WASDinput = new Vector2(moveOnXAxis, moveOnYAxis);
        
        _mouseY += InputManager.mouseY;

        MoveIn8Directions(WASDinput);
        SetPLayerStatus(isNormal, isAiming, isShooting);
        SetPlayerSpeedAccordingToSituation(isAiming, isShooting);

        if (WASDinput != Vector2.zero || !isNormal)
            LookForward();
        if (!isNormal)
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
        Vector3 playerEyesPosition = new Vector3(0, _playerEyes.position.y, _playerEyes.position.z); 
        Vector3 cameraPosition = new Vector3(0, _playerCamera.position.y, _playerCamera.position.z); 
        float angleBetween = Vector3.Angle(cameraPosition, playerEyesPosition);
        float finalAngle = -_mouseY * angleBetween * _playerLookUpDownSensitivity;
        return finalAngle;
    }

    #endregion
    
}
