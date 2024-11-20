
using UnityEngine;
using static PlayerStateMachine;

[CreateAssetMenu]
public class LimitRemover : Ability
{
    public float limitRemoveSpeedNow;
    [SerializeField] float temp;

    public override void Activate(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
         PlayerStateMachine m_StateMach = parent.GetComponent<PlayerStateMachine>();
        m_StateMach.ChangeCurrentState(PlayerStates.LimitRemover, true);
        movement.maxSpeed = limitRemoveSpeedNow;
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        PlayerStateMachine m_StateMach = parent.GetComponent<PlayerStateMachine>();
        m_StateMach.ChangeCurrentState(PlayerStates.LimitRemover, false);
        movement.maxSpeed = temp;
    }
}
