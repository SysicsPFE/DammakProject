using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    #region variables

        #region private static variables

        private static GameObject _crossHair;

        #endregion

        #region public variables

        public TextMeshProUGUI ammo;
        public TextMeshProUGUI magazine;

        #endregion

        #region private variables

        private Weapon _nowWeapon;
        private int _allMagazines;

        #endregion
        
    #endregion

    #region buildin methods

    void Awake()
    {
        _crossHair = GameObject.Find("crossHair");
        _nowWeapon = WeaponsManager.nowWeapon;
        _allMagazines = _nowWeapon.magazineInReserve;

    }

    void Update()
    {
        ammo.text = _nowWeapon.bulletsInMagazine.ToString("D2") + "/" + _nowWeapon.bulletsInFullMagazine;
        magazine.text = _nowWeapon.magazineInReserve.ToString("D2") + "/" + _allMagazines.ToString("D2");
        if (_nowWeapon.magazineInReserve == 0)
        {
            ammo.textInfo.textComponent.color = Color.red;
            magazine.textInfo.textComponent.color=Color.red;
        }
            
    }

    #endregion

    #region custom methods

    public static void CrossHairStatus(bool onOrOff)
    {
        _crossHair.SetActive(onOrOff);
    }

    #endregion
}
