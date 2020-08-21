using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    #region variables

    #region private variables

    private Animator _playerAnimator;
    private Transform _playerCamera;
    private CharacterController _playerController;
    private AudioSource[] _audioSources;
    private AudioSource _playerMovingSound;
    private AudioSource _m4A1Sound;
    private int _soundState;
    private bool _isShooting;

    [SerializeField] private float playerSpeed = 1;

    #region private static variables

    private static readonly int Angle = Animator.StringToHash("Angle");
    private static readonly int XAxis = Animator.StringToHash("xAxis");
    private static readonly int YAxis = Animator.StringToHash("yAxis");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");
    private static readonly int IsShooting = Animator.StringToHash("isShooting");

    #endregion

    #endregion

    #region constent variables

    private const float PlayerRotationSensitivity = 7f;
    private const float ConstantSpeed = 100;

    #endregion

    #region public variables

    public float bulletDamage = 2;
    public Healthbar healthBar;
    public AudioClip walkSound;
    public AudioClip runSound;

    #endregion

    #endregion

    #region buildin methods

    void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerCamera = GameObject.FindWithTag("MainCam").transform;
        _playerController = GetComponent<CharacterController>();
        healthBar.health = healthBar.maximumHealth;
        _audioSources = GetComponents<AudioSource>();
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.clip != null)
                _m4A1Sound = audioSource;
            else
                _playerMovingSound = audioSource;
        }
    }

    void Update()
    {
        ControllingPlayer();
        UpdateHealthStatus();
    }

    #endregion

    #region custom methods

    private void ControllingPlayer()
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
            _playerAnimator.SetFloat(Angle, LookUpDownAngle());
            UiManager.CrossHairStatus(true);
        }
        else
        {
            _playerAnimator.SetFloat(Angle, 0);
            UiManager.CrossHairStatus(WASDinput == Vector2.zero);
        }

        if(Input.GetKey(InputManager.leftMouseButton))
            Shoot();

        ManageMovingSound(WASDinput);
        ManageWeaponSound();
    }

    private void LookForward()
    {
        Vector3 playerCameraEulerAngles = _playerCamera.eulerAngles;
        Quaternion turnAngle = Quaternion.Euler(0, playerCameraEulerAngles.y, 0);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, turnAngle, Time.deltaTime * PlayerRotationSensitivity);
    }

    private void MoveIn8Directions(Vector2 movementInput)
    {
        _playerAnimator.SetFloat(XAxis, movementInput.x);
        _playerAnimator.SetFloat(YAxis, movementInput.y);
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        _playerController.Move(move * playerSpeed);
    }

    private void SetPLayerStatus(bool isNormal, bool isAiming, bool isShooting)
    {
        if (isNormal)
        {
            _playerAnimator.SetBool(IsAiming, false);
            _playerAnimator.SetBool(IsShooting, false);
        }
        else
        {
            _playerAnimator.SetBool(IsAiming, isAiming);
            _playerAnimator.SetBool(IsShooting, isShooting);
        }
    }

    private void SetPlayerSpeedAccordingToSituation(bool isAiming, bool isShooting)
    {
        if (isAiming)
            playerSpeed = (ConstantSpeed * Time.deltaTime) / 5;
        else if (isShooting)
            playerSpeed = (ConstantSpeed * Time.deltaTime) / 3;
        else
            playerSpeed = ConstantSpeed * Time.deltaTime;
    }

    private static float LookUpDownAngle()
    {
        CinemachineFreeLook playerCinemachineCamera =
            GameObject.FindWithTag("VirtualMainCamera").GetComponent<CinemachineFreeLook>();
        float angle = Mathf.Lerp(-90, 90, playerCinemachineCamera.m_YAxis.Value);
        return (angle);
    }

    private void Shoot()
    {
        if (!Physics.Raycast(_playerCamera.position, _playerCamera.forward, out RaycastHit hit)) return;
        if (WeaponsManager.nowWeapon.isReloading) return;
        Transform enemy = hit.collider.transform;
        if (enemy.TryGetComponent(out SafeProgram enemyIsSafeProgram))
            enemyIsSafeProgram.currentHealth -= bulletDamage;
        if (enemy.TryGetComponent(out MachineGun enemyIsMachineGun))
            enemyIsMachineGun.healthBar.health -= bulletDamage;
        if (enemy.TryGetComponent(out RobotFighter enemyIsRobotFighter))
            enemyIsRobotFighter.healthbar.health -= bulletDamage;
        if (enemy.TryGetComponent(out OrangeDrone enemyIsOrangeDrone))
            enemyIsOrangeDrone.healthbar.health -= bulletDamage;
        if (enemy.TryGetComponent(out SpaceDroid enemyIsSpaceDroid))
            enemyIsSpaceDroid.healthbar.health -= bulletDamage;
        if (enemy.TryGetComponent(out Tank enemyIsTank))
            enemyIsTank.healthbar.health -= bulletDamage;


    }

    private void UpdateHealthStatus()
    {
        healthBar.health = GameManager._playerHealth;
    }

    private void ManageMovingSound(Vector2 input)
    {
        if (Input.GetKey(InputManager.rightMouseButton))
        {
            if (input != Vector2.zero && _soundState != 1)
            {
                _playerMovingSound.clip = walkSound;
                _playerMovingSound.Play();
                _soundState = 1;
            }
            else if(input == Vector2.zero)
            {
                _playerMovingSound.Stop();
                _soundState = 0;
            }
        }
        else if (input != Vector2.zero && _soundState != 2)
        {
            _playerMovingSound.clip = runSound;
            _playerMovingSound.Play();
            _soundState = 2;
        }
        else if(input == Vector2.zero)
        {
            _playerMovingSound.Stop();
            _soundState = 0;
        }
    }

    private void ManageWeaponSound()
    {
        if (Input.GetKey(InputManager.leftMouseButton) && !_isShooting && !WeaponsManager.nowWeapon.isReloading)
        {
            _m4A1Sound.Play();
            _isShooting = true;
        }
        else if(!Input.GetKey(InputManager.leftMouseButton) || WeaponsManager.nowWeapon.isReloading)
        {
            _m4A1Sound.Stop();
            _isShooting = false;
        }
    }
    #endregion
}