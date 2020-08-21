using UnityEngine;

namespace Cinemachine.Examples
{

[AddComponentMenu("")] // Don't display in add component menu
public class CharacterMovement2D : MonoBehaviour
{
    public KeyCode sprintJoystick = KeyCode.JoystickButton2;
    public KeyCode jumpJoystick = KeyCode.JoystickButton0;
    public KeyCode sprintKeyboard = KeyCode.LeftShift;
    public KeyCode jumpKeyboard = KeyCode.Space;
    public float jumpVelocity = 7f;
    public float groundTolerance = 0.2f;
    public bool checkGroundForJump = true;

    private float _speed = 0f;
    private bool _isSprinting = false;
    private Animator _anim;
    private Vector2 _input;
    private float _velocity;
    private bool _headingleft = false;
    private Quaternion _targetrot;
    private Rigidbody _rigbody;

	// Use this for initialization
	void Start ()
	{
	    _anim = GetComponent<Animator>();
	    _rigbody = GetComponent<Rigidbody>();
	    _targetrot = transform.rotation;        
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    _input.x = Input.GetAxis("Horizontal");

        // Check if direction changes
	    if ((_input.x < 0f && !_headingleft) || (_input.x > 0f && _headingleft))
	    {  
            if (_input.x < 0f) _targetrot = Quaternion.Euler(0, 270, 0);
	        if (_input.x > 0f) _targetrot = Quaternion.Euler(0, 90, 0);
	        _headingleft = !_headingleft;
	    }
        // Rotate player if direction changes
        transform.rotation = Quaternion.Lerp(transform.rotation, _targetrot, Time.deltaTime * 20f);

		// set speed to horizontal inputs
	    _speed = Mathf.Abs(_input.x);
        _speed = Mathf.SmoothDamp(_anim.GetFloat("Speed"), _speed, ref _velocity, 0.1f);
        _anim.SetFloat("Speed", _speed);

        // set sprinting
	    if ((Input.GetKeyDown(sprintJoystick) || Input.GetKeyDown(sprintKeyboard))&& _input != Vector2.zero) _isSprinting = true;
	    if ((Input.GetKeyUp(sprintJoystick) || Input.GetKeyUp(sprintKeyboard))|| _input == Vector2.zero) _isSprinting = false;
        _anim.SetBool("isSprinting", _isSprinting);

        // Jump
	    if ((Input.GetKeyDown(jumpJoystick) || Input.GetKeyDown(jumpKeyboard)) && IsGrounded())
	    {
	        _rigbody.velocity = new Vector3(_input.x, jumpVelocity, 0f);
	    }
	}

    public bool IsGrounded()
    {
        if (checkGroundForJump)
            return Physics.Raycast(transform.position, Vector3.down, groundTolerance);
        else
            return true;
    }
}

}
