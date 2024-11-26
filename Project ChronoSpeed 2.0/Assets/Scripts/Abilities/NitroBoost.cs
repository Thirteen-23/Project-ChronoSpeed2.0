using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static PlayerStateMachine;

[CreateAssetMenu]
public class NitroBoost : Ability
{
    public float boost; 
    public override void Activate(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        PlayerStateMachine m_StateMach = parent.GetComponent<PlayerStateMachine>();
        m_StateMach.ChangeCurrentState(PlayerStates.Boosting, true);
        movement.meBoosting = true;
       
    }

    
    public override void BeginCooldown(GameObject parent)
    {
        PlayerStateMachine m_StateMach = parent.GetComponent<PlayerStateMachine>();
        m_StateMach.ChangeCurrentState(PlayerStates.IdlePower, true);
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        movement.meBoosting = false;
    }
}
