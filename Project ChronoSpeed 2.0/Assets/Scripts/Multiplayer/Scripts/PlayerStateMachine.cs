using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateMachine : NetworkBehaviour
{
    public PlayerStates CurrentDrivingState;
    public PlayerStates CurrentPowerState;
    private VFXContainer vfxCon;

    [Header("Refrences")]
    [SerializeField] private BoxCollider colForFakeCollision;

    [Header("VFX Refrences")]
    [SerializeField] private Material HologramShader;

    private void Awake()
    {
        vfxCon = GetComponentInChildren<VFXContainer>();
    }
    public enum PlayerStates
    {
        IdleDriving, Driving, Reversing,
        Breaking, StartDrifting, Drifting, Boosting, LimitRemover, 


        IdlePower, DroppingPortal, Blinking, Rewinding, TempInvonrability


    }
    

    public void ChangeCurrentState(PlayerStates newState, bool switchTo)
    {
        if((int)newState > 6)
        {
            if (newState == CurrentPowerState)
                return;

            ActivateRelativeFunc(CurrentPowerState, false);
            CurrentPowerState = newState;
            ActivateRelativeFunc(CurrentPowerState, true);
            ChangeCurrentStateRpc(newState, switchTo);
        }
        else
        {
            if (newState == CurrentDrivingState)
                return;
            ActivateRelativeFunc(CurrentDrivingState, false);
            CurrentDrivingState = newState;
            ActivateRelativeFunc(CurrentDrivingState, true);
            ChangeCurrentStateRpc(newState, switchTo);
        }
        
        //I dont know if we want it to be called on ourselves yet, will have to ask gabe

        //ActivateRelativeFunc(CurrentState, false);
        //ActivateRelativeFunc(curState, true);
        
    }

    //Sends it to this script version on all the other clients, so they will get updated about this
    [Rpc(SendTo.NotMe)]
    private void ChangeCurrentStateRpc(PlayerStates newState, bool switchTo)
    {

        if ((int)newState > 6)
        {
            ActivateRelativeFunc(CurrentPowerState, false);
            CurrentPowerState = newState;
            ActivateRelativeFunc(CurrentPowerState, true);
        }
        else
        {
            ActivateRelativeFunc(CurrentDrivingState, false);
            CurrentDrivingState = newState;
            ActivateRelativeFunc(CurrentDrivingState, true);
        }
    }

    private void ActivateRelativeFunc(PlayerStates curState, bool SwitchTo)
    {
        switch(curState)
        {
            case PlayerStates.IdleDriving:
                IdleDriving(SwitchTo); break;
            case PlayerStates.Driving:
                Driving(SwitchTo); break;
            case PlayerStates.Reversing:
                Reversing(SwitchTo); break;
            case PlayerStates.Breaking:
                Braking(SwitchTo); break;
            case PlayerStates.Drifting:
                Drifting(SwitchTo); break;
            case PlayerStates.DroppingPortal:
                DroppingPortal(SwitchTo); break;
            case PlayerStates.Rewinding:
                Rewinding(SwitchTo); break;
            case PlayerStates.Blinking:
                Blinking(SwitchTo); break;
            case PlayerStates.Boosting:
                Boosting(SwitchTo); break;
            case PlayerStates.LimitRemover:
                LimitRemover(SwitchTo); break;
            case PlayerStates.TempInvonrability:
                StartCoroutine(TempInvonrability(SwitchTo)); break;
        }
    }


    //this would also reduce the need of a vfx manager, at least remove the rpcs from that scripts so that its only called locally, and we dont need to rpc everytime we call a particle :)

    
    
    private void IdleDriving(bool switchTo)
    {
      //vfxCon.SetVFX()
        //Idle noise = switchTo,
    }

    [SerializeField] TrailRenderer[] trails = new TrailRenderer[2];
    private void Driving(bool switchTo)
    {
      
        //Driving noise = switchTo,
        //those tire particles = switchTo,
        //if(on) maybe make the sound and velocity of the particles based of verleyPhysics
        //maybe even the direction of the particles -verlocity? i dont think thats how tires work so idk

    }

    private void Reversing(bool switchTo)
    {
        //reversing lights = switchTo, i heard tristan talking about it to a designer, and how he was gonna code it, he probs didnt think about multiplayer so just in case
        //tire particles = switchTo,
        //reverse the direction of particles possible, and maybe reduce the smokeparticle effect so it doesnt cover the screen
    }

    private void Braking(bool switchTo)
    {
        //tireParticles = switchTo;
        //bunch up tire smoke?
        //make screech noise and make tire smoke greyer?
    }

    [SerializeField] AudioSource m_DriftSound;
    private void Drifting(bool switchTo)
    {
        //vfxCon.SetVFX(VFXContainer.VFXTypes.spinSmoke, switchTo);
        vfxCon.SetVFX(VFXContainer.VFXTypes.Drifting, switchTo);
        m_DriftSound.volume = Convert.ToInt32(switchTo);

        //tireParticles = switchTo;
        //drifiting noise = switchTo;
    }

    private void DroppingPortal(bool switchTo)
    {
        //droppingPortalNoise = switchTo; idk if we want a noise to play for other people to inform them, if not ignore this line
        //DroppingPortalVFX = switchTo; like maybe a pink glow on the back of the car or something
    }

    private void Blinking(bool switchTo)
    {
        //blinkVFX = switchTo;
        //blinkNoise = switchTo; same thing with the droppingPortalNoise
        vfxCon.SetVFX(VFXContainer.VFXTypes.electricBall, switchTo); 
    }

    private void Rewinding(bool switchTo)
    {
        colForFakeCollision.enabled = !switchTo;
        vfxCon.SetVFX(VFXContainer.VFXTypes.Ghosting, switchTo);
        //rewindHologramVFX = switchTo;
    }
    public Class carClasses;
    private void Boosting(bool switchTo)
    {
            vfxCon.SetVFX(VFXContainer.VFXTypes.Boosting, switchTo);
    }
    private void LimitRemover(bool switchTo)
    {
            vfxCon.SetVFX(VFXContainer.VFXTypes.SpeedLimitRemover, switchTo);
    }
    private IEnumerator TempInvonrability(bool switchTo)
    {
        if(switchTo == false) { yield break; }
        colForFakeCollision.enabled = false;
        vfxCon.SetVFX(VFXContainer.VFXTypes.Ghosting, true);
        yield return new WaitForSeconds(2f);
        vfxCon.SetVFX(VFXContainer.VFXTypes.Ghosting, false);
        colForFakeCollision.enabled = true;
        if(CurrentPowerState == PlayerStates.TempInvonrability) 
        {
            ChangeCurrentState(PlayerStates.IdlePower, true); 
        }
        //this one might fuck everything up, cause they should have temp invincibilty once getting outta rewind
        //but its a state machine so i cant have two going at once, maybe i will make this a coroutine that lasts 2 seconds
        //that is called when rewinding = false;
    }
}
