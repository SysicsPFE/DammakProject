using UnityEngine;


public class GameManager : MonoBehaviour
{
    
    #region variables

    #endregion

    #region buildin methods

    void Update()
    {
        LockCursor(true);
    }

    #endregion
    
    #region custom methods

    public static void LockCursor(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;   
        }
    }
    
    #endregion
    
}
