using UnityEngine;

public class AllManagers : MonoBehaviour
{
    public static AllManagers instance;
    
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
}
