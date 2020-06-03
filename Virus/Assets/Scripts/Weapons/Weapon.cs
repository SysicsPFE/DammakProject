using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region variables

        #region public variables

        public int magazineInReserve;
        public int bulletsInMagazine;
        public int bulletsInFullMagazine;

        #endregion

        #region protected variables
        
        protected int fireRatePerSecond = 0;

        #endregion

        #region private variables

        private float _timer;
        private bool _isReloading = false;

        #endregion

    #endregion

    #region virtual buildin methods

    protected virtual void Start()
    {
        WeaponsManager.nowWeapon.bulletsInMagazine = WeaponsManager.nowWeapon.bulletsInFullMagazine;
    }

    public virtual void FixedUpdate()
    {
        bulletsInMagazine = WeaponsManager.nowWeapon.bulletsInMagazine;
        magazineInReserve = WeaponsManager.nowWeapon.magazineInReserve;
        if (Input.GetKey(InputManager.leftMouseButton))
        {
            _timer += Time.fixedDeltaTime * fireRatePerSecond;
            if (_timer >= 1)
            {
                if (bulletsInMagazine > 0)
                {
                    bulletsInMagazine -= 1;
                    _isReloading = false;
                }
                else if(magazineInReserve > 0)
                {
                    StartCoroutine(ReloadIn(1));
                    _isReloading = true;
                }
                _timer = 0;
            }
        }
        else
            _timer = 0;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(bulletsInMagazine < bulletsInFullMagazine)
                StartCoroutine(ReloadIn(1));
        }

        WeaponsManager.nowWeapon.bulletsInMagazine = bulletsInMagazine;
        WeaponsManager.nowWeapon.magazineInReserve = magazineInReserve;
    }

    #endregion

    private IEnumerator ReloadIn(int seconds)
    {
        if(_isReloading) yield break;
        yield return new WaitForSeconds(seconds);
        bulletsInMagazine = bulletsInFullMagazine;
        if(magazineInReserve > 0)
            magazineInReserve--;

    }
}
