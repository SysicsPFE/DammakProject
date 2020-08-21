using UnityEngine;

public class OrangeDrone : Drone
{
    protected override void Update()
    {
        base.Update();
        if (healthbar.health > 0) return;
        GameManager.appDataDroneNumbers--;
        if (GameManager.appDataDroneNumbers == 0)
        {
            GameManager.FillPlayerHealth(100);
            WeaponsManager.FillNowWeapon(30,3);
            GameManager.OpenDoor("Registry");
            GameManager.OpenDoor("System32");
        }
        WeaponsManager.AddMoreAmmo(10,0);
        Destroy(gameObject);
    }
}
