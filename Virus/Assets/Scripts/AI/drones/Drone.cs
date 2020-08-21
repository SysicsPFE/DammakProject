using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Drone : MonoBehaviour
{
    #region variables

    #region public variables

    public float bulletDamage = 2;
    public Transform targetIsPlayer;
    public Healthbar healthbar;

    #endregion

    #region private variables
    
    private Vector3 _newPosition;
    private bool _enemyFound;
    private AudioSource _audioSource;

    #endregion

    #endregion

    #region buildin methods

    protected virtual void Start()
    {
        _newPosition = transform.position;
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if (!_enemyFound) return;
        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime);
        transform.LookAt(targetIsPlayer);
        Attack();
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _enemyFound = true;
        GetComponent<BoxCollider>().enabled = false;
        _audioSource.Play();
        StartCoroutine(SetNewPositionEvery(Random.Range(5,11)));
    }
    
    #endregion

    #region custom methods

    private IEnumerator SetNewPositionEvery(float second)
    {
        yield return new WaitForSeconds(second);
        Vector2 playerPosition = new Vector2(GameManager._playerTransform.position.x,GameManager._playerTransform.position.z);
        _newPosition = new Vector3
        {
            x = RandomPointInAnnulus(playerPosition, 50f, 100f).x,
            y = Random.Range(800f, 820f),
            z = RandomPointInAnnulus(playerPosition, 50f, 100f).y
        };
        StartCoroutine(SetNewPositionEvery(second));
    }
    
    private Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
    {
        Vector2 randomDirection = (Random.insideUnitCircle * origin).normalized;
 
        float randomDistance = Random.Range(minRadius, maxRadius);
 
        Vector2 point = origin + randomDirection * randomDistance;
 
        return point;
    }
    private void Attack()
    {
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo)) return;
        if (!hitInfo.collider.TryGetComponent(out Player playerScript)) return;
        GameManager._playerHealth -= bulletDamage;
    }

    #endregion
    
}
