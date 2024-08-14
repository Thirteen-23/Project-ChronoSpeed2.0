using UnityEngine;

[CreateAssetMenu]
public class AllTerrain : Ability
{
    public override void Activate(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();

        movement.turnOnAllTerrain = true; 
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();

        movement.turnOnAllTerrain = false;
    }
}
