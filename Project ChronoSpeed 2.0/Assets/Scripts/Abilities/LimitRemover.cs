
using UnityEngine;
using static PlayerStateMachine;

[CreateAssetMenu]
public class LimitRemover : Ability
{
    public float limitRemoveSpeedNow;
    public float limitRemoveSpeedFriction = 1.5f;
    [SerializeField] float temp;
    [SerializeField] float tempFriction = 1;

    public override void Activate(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
         PlayerStateMachine m_StateMach = parent.GetComponent<PlayerStateMachine>();
        m_StateMach.ChangeCurrentState(PlayerStates.LimitRemover, true);
        movement.maxSpeed = limitRemoveSpeedNow;
        movement.m_TForwardFrictionValue = movement.m_TSidewaysFrictionValue = limitRemoveSpeedFriction;
         
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        PlayerStateMachine m_StateMach = parent.GetComponent<PlayerStateMachine>();
        m_StateMach.ChangeCurrentState(PlayerStates.IdlePower, true);
        movement.m_TForwardFrictionValue = movement.m_TSidewaysFrictionValue = tempFriction;
       movement.maxSpeed = temp;
    }
}
