using UnityEngine;

public class SpaceDroid : Drone
{
    protected override void Update()
    {
        base.Update();
        if (healthbar.health > 0) return;
        GameManager.AddPlayerHealth(10);
        WeaponsManager.AddMoreAmmo(10,0);
        Destroy(gameObject);
    }

}
