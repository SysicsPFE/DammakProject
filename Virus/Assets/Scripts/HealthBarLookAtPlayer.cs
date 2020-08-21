using UnityEngine;

public class HealthBarLookAtPlayer : MonoBehaviour
{
    private Transform _mainCamera;
    void Start() => _mainCamera = GameObject.FindWithTag("MainCam").transform;
    void Update() => transform.LookAt(_mainCamera);
}
