using UnityEngine;

public class LockDoor : MonoBehaviour
{
    private static readonly int IsOpening = Animator.StringToHash("isOpening");
    public SkinnedMeshRenderer doorRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        doorRenderer.material = GameManager.instance.doorLockMaterial;
        GetComponentInParent<Animator>().SetBool(IsOpening,false);
        GetComponentInParent<Door>().enabled = false;
        Destroy(gameObject);
    }
}
