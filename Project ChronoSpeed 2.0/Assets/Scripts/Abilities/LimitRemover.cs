
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
      
        movement.maxSpeed = limitRemoveSpeedNow;
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
       
        movement.maxSpeed = temp;
    }
}
