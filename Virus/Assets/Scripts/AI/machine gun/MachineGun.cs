using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class MachineGun : MonoBehaviour
{
    #region variables

    #region private variables

    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] private GameObject machineGunTarget;
    [SerializeField] private RigBuilder machineRotationRig;
    [SerializeField] private Transform machineMuzzle;

    private static readonly int ToWar = Animator.StringToHash("toWar");

    #endregion

    #region public variables

    public Healthbar healthBar;
    public float bulletDamage = 2;
    public AudioClip shootingSound;
    public AudioClip idleSound;

    #endregion    

    #endregion

    #region buildin methods

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        if (healthBar.health > 0) return;
        WeaponsManager.FillNowWeapon(30,3);
        GameManager.FillPlayerHealth(100);
        GameManager.OpenDoor("task manager");
        GameManager.OpenDoor("Registry");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        _audioSource.clip = shootingSound;
        _audioSource.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        AttackPlayer(other,true);
    }

    private void OnTriggerExit(Collider other)
    {
        AttackPlayer(other, false);
        _audioSource.Stop();
    }
    
    #endregion

    #region custom methods
    
    private void AttackPlayer(Collider playerCollider, bool attack)
    {
        if (playerCollider.CompareTag("Player"))
            _animator.SetBool(ToWar, attack);
        machineGunTarget.SetActive(attack);
        machineRotationRig.enabled = attack;
        RaycastHit hit;
        if (Physics.Raycast(machineMuzzle.position, machineMuzzle.up, out hit))
        {
            if (hit.collider.TryGetComponent(out Player playerScript))
                GameManager._playerHealth -= bulletDamage;
        }
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
