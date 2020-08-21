using UnityEngine;

namespace Cinemachine.Examples
{

public class ScriptingExample : MonoBehaviour
{
    CinemachineVirtualCamera _vcam;
    CinemachineFreeLook _freelook;

    void Start()
    {
        // Create a Cinemachine brain on the main camera
        var brain = GameObject.Find("Main Camera").AddComponent<CinemachineBrain>();
        brain.m_ShowDebugText = true;
        brain.m_DefaultBlend.m_Time = 1;

        // Create a virtual camera that looks at object "Cube", and set some settings
        _vcam = new GameObject("VirtualCamera").AddComponent<CinemachineVirtualCamera>();
        _vcam.m_LookAt = GameObject.Find("Cube").transform;
        _vcam.m_Priority = 10;
        _vcam.gameObject.transform.position = new Vector3(0, 1, 0);

        // Install a composer.  You can install whatever CinemachineComponents you need,
        // including your own custom-authored Cinemachine components.
        var composer = _vcam.AddCinemachineComponent<CinemachineComposer>();
        composer.m_ScreenX = 0.30f;
        composer.m_ScreenY = 0.35f;

        // Create a FreeLook vcam on object "Cylinder"
        _freelook = new GameObject("FreeLook").AddComponent<CinemachineFreeLook>();
        _freelook.m_LookAt = GameObject.Find("Cylinder/Sphere").transform;
        _freelook.m_Follow = GameObject.Find("Cylinder").transform;
        _freelook.m_Priority = 11;

        // You can access the individual rigs in the freeLook if you want.
        // FreeLook rigs come with Composers pre-installed.
        // Note: Body MUST be Orbital Transposer.  Don't change it.
        CinemachineVirtualCamera topRig = _freelook.GetRig(0);
        CinemachineVirtualCamera middleRig = _freelook.GetRig(1);
        CinemachineVirtualCamera bottomRig = _freelook.GetRig(2);
        topRig.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.35f;
        middleRig.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.25f;
        bottomRig.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.15f;
    }

    float _lastSwapTime = 0;
    void Update()
    {
        // Switch cameras from time to time to show blending
        if (Time.realtimeSinceStartup - _lastSwapTime > 5)
        {
            _freelook.enabled = !_freelook.enabled;
            _lastSwapTime = Time.realtimeSinceStartup;
        }
    }
}

}
