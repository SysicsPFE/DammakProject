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
        _playerAnimator.SetFloat("xAxis",movementInput.x);
        _playerAnimator.SetFloat("yAxis",movementInput.y);
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        _playerController.Move(move*playerSpeed);
    }

    #endregion
    
}
