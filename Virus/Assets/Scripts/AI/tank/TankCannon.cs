using System.Collections;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    private Transform _player;
    private AudioSource _audioSource;
    public Transform target;

    [SerializeField] private float bullet = 30;
    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        StartCoroutine(Shoot());
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Tank.inAimingRange ? _player : null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Tank.inAimingRange = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Tank.inAimingRange = true;
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(10);
        if (!Tank.inAimingRange) yield break;
        _audioSource.Play();
        if (Physics.Linecast(transform.position,target.position,out RaycastHit hitInfo))
        {
            if (hitInfo.collider.transform.TryGetComponent(out Player targetIsPlayer))
                targetIsPlayer.healthBar.health -= bullet;
               
        }
        StartCoroutine(Shoot());
    }
}
