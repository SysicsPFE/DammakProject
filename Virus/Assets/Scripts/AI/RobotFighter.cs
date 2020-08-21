using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotFighter : MonoBehaviour
{
    #region variables

    #region private variables

    private Transform _player;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private float _wheelsRotationAngle;
    private AudioSource _audioSource;
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int MoveSpeed = Animator.StringToHash("moveSpeed");
    [SerializeField] private Transform leftHandWeapon;
    [SerializeField] private Transform rightHandWeapon;

    #endregion
    
    #region public variables

    public Healthbar healthbar;
    public float bulletDamage = 2;
    public AudioClip shootingSound;
    public AudioClip idleSound;

    #endregion  

    #endregion

    #region buildin methods

    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (healthbar.health > 0) return;
        WeaponsManager.FillNowWeapon(30,10);
        GameManager.FillPlayerHealth(500);
        GameManager.OpenDoor("System32");
        GameManager.OpenDoor("AppData");
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _audioSource.clip = shootingSound;
        _audioSource.Play();
    }

    private void OnTriggerStay(Collider other) => AttackPlayer(other);

    private void OnTriggerExit(Collider other)
    {
        BeIdle(other);
        _audioSource.Stop();
    }

    #endregion

    #region custom methods

    private void AttackPlayer(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SetAndFollow();
        ShootWith(leftHandWeapon);
        ShootWith(rightHandWeapon);
    }
    private void SetAndFollow()
    {
        _animator.SetBool(Attack,true);
        _navMeshAgent.destination = _player.position;
        _wheelsRotationAngle = _navMeshAgent.velocity.magnitude * Time.deltaTime;
        _animator.SetFloat(MoveSpeed, _wheelsRotationAngle);
        _animator.SetLayerWeight(1,(_wheelsRotationAngle == 0)? 0 : 1);
    }

    private void BeIdle(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _animator.SetBool(Attack,false);
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        _animator.SetLayerWeight(1,0);
    }
    
    private void ShootWith(Transform weapon)
    {
        if (!Physics.Raycast(weapon.position, weapon.up, out RaycastHit hit)) return;
        if (!hit.collider.TryGetComponent(out Player playerScript)) return;
        GameManager._playerHealth -= bulletDamage;
    }
    
    public void StartArmSound()
    {
        _audioSource.clip = idleSound;
        _audioSource.Play();
    }

    public void StopArmSound()
    {
        _audioSource.Stop();
    }

    #endregion
}
