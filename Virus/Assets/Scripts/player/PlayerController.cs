using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    #region variables

        #region public variables

        public float playerSpeed = 1;

        #endregion

        #region private variables

        private Animator _playerAnimator;
        private Transform _playerCamera;
        private CharacterController _playerController;

        #endregion

        #region constent variables

        private const float PlayerRotationSensitivity = 5f;

        #endregion

    #endregion

    #region buildin methods

    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        _playerController = GetComponent<CharacterController>();
        playerSpeed *= Time.deltaTime;
    }

    void Update()
    {
        float moveOnXAxis = Input.GetAxis("Horizontal");
        float moveOnYAxis = Input.GetAxis("Vertical");
        Vector2 WASDinput = new Vector2(moveOnXAxis, moveOnYAxis);
        MoveIn8Directions(WASDinput);
        if(WASDinput != Vector2.zero)
            LookForward();
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
        Vector3 YjoystickAxis = MoveForwardAndBackward(movementInput.y);
        Vector3 XjoystickAxis = MoveRightAndLeft(movementInput.x);
        _playerController.Move(YjoystickAxis + XjoystickAxis);
    }

    private Vector3 MoveForwardAndBackward(float WSaxis)
    {
        _playerAnimator.SetFloat("yAxis",WSaxis);
        Vector3 normalizedPlayerAndCameraDistance = (transform.position - _playerCamera.position).normalized;
        Vector3 playerSpeedForwardAndBackward =
            new Vector3(playerSpeed * WSaxis * normalizedPlayerAndCameraDistance.x, 0,
                playerSpeed * WSaxis * normalizedPlayerAndCameraDistance.z);
        return playerSpeedForwardAndBackward;
    }

    private Vector3 MoveRightAndLeft(float ADaxis)
    {
        _playerAnimator.SetFloat("xAxis",ADaxis);
        /*Vector3 normalizedPlayerCameraDistance = (transform.position - _playerCamera.position).normalized;
        normalizedPlayerCameraDistance.x = Mathf.Abs(normalizedPlayerCameraDistance.x);
        normalizedPlayerCameraDistance.z = Mathf.Abs(normalizedPlayerCameraDistance.z);
        Vector3 newPosition = transform.position - normalizedPlayerCameraDistance;
        Vector3 playerSpeedRightAndLeft = new Vector3(playerSpeed * ADaxis * newPosition.x, 0,
            playerSpeed * ADaxis * newPosition.z);
        return playerSpeedRightAndLeft;*/
        Vector3 normalizedPlayerAndCameraDistance = (transform.position - _playerCamera.position).normalized;
        Vector3 playerSpeedForwardAndBackward =
            new Vector3(playerSpeed * ADaxis * normalizedPlayerAndCameraDistance.z, 0,
                playerSpeed * ADaxis * normalizedPlayerAndCameraDistance.x);
        return playerSpeedForwardAndBackward;
    }

    #endregion
    
}
