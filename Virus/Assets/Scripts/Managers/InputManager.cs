using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region variables

        #region public static variables
    
        public static KeyCode leftMouseButton;
        public static KeyCode rightMouseButton;
        public static float mouseY = 0;
        public static float mouseX = 0;
        public static float upDownArrowKeys = 0;
        public static float rightLeftArrowKeys = 0;

        #endregion
    
    #endregion

    #region buildin methods

    void Update()
    {
        leftMouseButton = KeyCode.Mouse0;
        rightMouseButton = KeyCode.Mouse1;
        mouseY = Input.GetAxis("Mouse Y");
        mouseX = Input.GetAxis("Mouse X");
        upDownArrowKeys = Input.GetAxis("Vertical");
        rightLeftArrowKeys = Input.GetAxis("Horizontal");
    }

    #endregion
}
