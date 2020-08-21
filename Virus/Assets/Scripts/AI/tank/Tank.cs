using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    #region variables

    #region private variables

    private NavMeshAgent _navMeshAgent;
    private Transform _player;
    private Vector3 _playerPosition;
    private Vector3 _target;
    private bool _enemyFound;

    #endregion

    #region public variables

    public Transform tankHead;
    public float thornsDamage = 0.01f;
    public Healthbar healthbar;

    #endregion
    public static bool inAimingRange = true;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_enemyFound) return;
        _playerPosition = _player.position;
        _target = new Vector3(_playerPosition.x,tankHead.position.y,_playerPosition.z);
        _navMeshAgent.destination = _playerPosition;
        tankHead.LookAt(_target);
        if(healthbar.health > 0) return;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_enemyFound) return;
        _enemyFound = true;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager._playerHealth -= thornsDamage;
    }
}
