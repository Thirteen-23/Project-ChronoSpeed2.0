
using UnityEngine;

[CreateAssetMenu]
public class LimitRemover : Ability
{
    public float limitRemoveSpeedNow;
    [SerializeField] float temp;
    //TrailRenderer leftTrail;
    //TrailRenderer rightTrail;
    public override void Activate(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        movement.leftTrail.emitting = true;
        movement.rightTrail.emitting = true;
        movement.maxSpeed = limitRemoveSpeedNow;
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        movement.leftTrail.emitting = false;
        movement.rightTrail.emitting = false;
        movement.maxSpeed = temp;
    }
}
