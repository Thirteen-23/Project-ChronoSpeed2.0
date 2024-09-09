using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch_Manager : MonoBehaviour
{
    AI m_AI;
    AI_Controls m_AIMainControls;
    Car_Movement m_PlayerMovement;
    PlayerInput playerinputs; 
    public enum types
    {
        AI, 
        Player
    }

    [SerializeField] public types driver; 
    // Start is called before the first frame update
    void Start()
    {
        m_AI = GetComponent<AI>();
        m_AIMainControls = GetComponent<AI_Controls>();
        m_PlayerMovement = GetComponent<Car_Movement>();
        playerinputs = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if(driver == types.AI)
        {
            m_PlayerMovement.enabled = false;
            m_AI.enabled = true;
            playerinputs.enabled = false;
            m_AIMainControls.enabled = true;
        }

        else if(driver == types.Player)
        {
            m_PlayerMovement.enabled = true;
            m_AI.enabled = false;
            m_AIMainControls.enabled = false;
            m_AIMainControls.acceration_Value = m_PlayerMovement.acceration_Value;
            playerinputs.enabled = true;
        }
    }
}
