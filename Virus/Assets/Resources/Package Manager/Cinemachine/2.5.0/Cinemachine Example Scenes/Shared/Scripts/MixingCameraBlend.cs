using UnityEngine;

namespace Cinemachine.Examples
{

[AddComponentMenu("")] // Don't display in add component menu
public class MixingCameraBlend : MonoBehaviour
{
    public enum AxisEnum {X,Z,Xz};

    public Transform followTarget;
    public float initialBottomWeight = 20f;
    public AxisEnum axisToTrack;

    private CinemachineMixingCamera _vcam;
    
    void Start()
    {
        if (followTarget)
        {
            _vcam = GetComponent<CinemachineMixingCamera>();
            _vcam.m_Weight0 = initialBottomWeight;
        }
    }

    void Update()
    {
        if (followTarget)
        {
            switch (axisToTrack)
            {
                case (AxisEnum.X):
                    _vcam.m_Weight1 = Mathf.Abs(followTarget.transform.position.x);
                    break;
                case (AxisEnum.Z):
                    _vcam.m_Weight1 = Mathf.Abs(followTarget.transform.position.z);
                    break;
                case (AxisEnum.Xz):
                    _vcam.m_Weight1 =
                        Mathf.Abs(Mathf.Abs(followTarget.transform.position.x) +
                                  Mathf.Abs(followTarget.transform.position.z));
                    break;
            }
        }
    }
}

}
