using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    #region variables

    private Animator _doorAnimator;
    private static readonly int IsOpening = Animator.StringToHash("isOpening");

    #endregion

    #region methods

    private void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !enabled) return;
        _doorAnimator.SetBool(IsOpening,true);

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !enabled) return;
        _doorAnimator.SetBool(IsOpening,false);
    }

    #endregion
    
}