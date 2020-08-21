using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    #region variables

        #region public static variables

        public static Weapon nowWeapon;

        #endregion

        #region public variables

        public static int _m4A1Magazines = 3;

        #endregion

    #endregion

    #region buildin methods

    void Awake()
    {
        nowWeapon = FindObjectOfType<M4A1>();
        nowWeapon.magazineInReserve = _m4A1Magazines;
    }

    public static void AddMoreAmmo(int bulletsInMagazine, int magazineInReserve)
    {
        nowWeapon.bulletsInMagazine += bulletsInMagazine;
        if (nowWeapon.bulletsInMagazine > nowWeapon.bulletsInFullMagazine)
        {
            nowWeapon.magazineInReserve++;
            nowWeapon.bulletsInMagazine = nowWeapon.bulletsInFullMagazine;
        }
        nowWeapon.magazineInReserve += magazineInReserve;
    }
    public static void FillNowWeapon(int bulletsInMagazine, int magazineInReserve)
    {
        nowWeapon.bulletsInMagazine = bulletsInMagazine;
        nowWeapon.magazineInReserve = magazineInReserve;
    }
    
    #endregion
    
}
