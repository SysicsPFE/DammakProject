using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    #region variables

        #region public static variables

        public static Weapon nowWeapon;

        #endregion

        #region public variables

        private int _M4A1Magazines = 3;

        #endregion

    #endregion

    #region buildin methods

    void Awake()
    {
        nowWeapon = FindObjectOfType<M4A1>();
        nowWeapon.magazineInReserve = _M4A1Magazines;
    }
    
    #endregion
    
}
