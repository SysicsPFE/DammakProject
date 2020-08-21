using UnityEngine;

public class TargetIsPlayer : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private string enemyTag;
    public float speed = 10f;

    private void Start() => _player = GameObject.FindWithTag("Player").transform;

    void Update()
    {
        bool getPlayerHeartPosition = (enemyTag == "robot fighter" || enemyTag == "enemy");
        Vector3 position = transform.position;
        Vector3 targetPos = new Vector3(position.x,_player.position.y,position.z);
        if (getPlayerHeartPosition) targetPos.y += 7f;
        Vector3 newPosition = Vector3.Lerp(targetPos,_player.position, Time.deltaTime * speed);
        position = newPosition;
        transform.position = position;
    }
}
