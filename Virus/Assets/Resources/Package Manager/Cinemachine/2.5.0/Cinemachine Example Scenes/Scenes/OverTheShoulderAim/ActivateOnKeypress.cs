using UnityEngine;

public class ActivateOnKeypress : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.LeftControl;
    public int priorityBoostAmount = 10;
    public GameObject reticle;

    Cinemachine.CinemachineVirtualCameraBase _vcam;
    bool _boosted = false;

    void Start()
    {
        _vcam = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
    }

    void Update()
    {
        if (_vcam != null)
        {
            if (Input.GetKey(activationKey))
            {
                if (!_boosted)
                {
                    _vcam.Priority += priorityBoostAmount;
                    _boosted = true;
                }
            }
            else if (_boosted)
            {
                _vcam.Priority -= priorityBoostAmount;
                _boosted = false;
            }
        }
        if (reticle != null)
            reticle.SetActive(_boosted);
    }
}
