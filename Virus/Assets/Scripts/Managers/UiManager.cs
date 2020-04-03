using UnityEngine;

public class UiManager : MonoBehaviour
{
    #region variables

        #region private static variables

        private static GameObject _crossHair; 

        #endregion
    

    #endregion

    #region buildin methods

    void Awake()
    {
        _crossHair = GameObject.Find("crossHair");
    }

    #endregion

    #region custom methods

    public static void CrossHairStatus(bool onOrOff)
    {
        _crossHair.SetActive(onOrOff);
    }

    #endregion
}
