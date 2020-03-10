using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region variables

        #region public static variables

         public static GameManager instance;

        #endregion
    
    #endregion

    #region buildin methods

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
            Destroy(gameObject);    
    }

    void Update()
    {
        lockCursor(true);
    }

    #endregion
    
    #region custom methods

    public static void lockCursor(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;   
        }
    }
    
    #endregion
}
