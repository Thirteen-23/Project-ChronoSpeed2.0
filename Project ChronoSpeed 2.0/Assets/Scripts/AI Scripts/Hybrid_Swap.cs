using UnityEngine;
using UnityEngine.InputSystem;

public class Hybrid_Swap : MonoBehaviour
{
    public enum types
    {   
        None,
        AI,
        Player

    }
    [SerializeField] types m_ControllerModes;
    Car_Movement m_Player;
    AI m_AI;
    AI_Controls m_AIControls;
    PlayerInput m_PlayerInput; 
    // Start is called before the first frame update
    void Start()
    {
        m_Player = gameObject.GetComponent<Car_Movement>();
        m_AI = gameObject.GetComponent<AI>();
        m_AIControls = gameObject.GetComponent<AI_Controls>();
        m_PlayerInput = gameObject.GetComponent<PlayerInput>();

    }

    // Update is called once per frame
    void Update()
    {
        switch(m_ControllerModes)
        {
            case types.None:

                m_Player.enabled = false;
                m_AI.enabled = false;
                m_AIControls.enabled = false;
                m_PlayerInput.enabled = false;


                break;
                    case types.Player:

                m_Player.enabled = true;
                m_PlayerInput.enabled = true;
                m_AI.enabled = false;
                m_AIControls.enabled = false;
                break;
            case types.AI:

                m_Player.enabled = false;
                m_PlayerInput.enabled = false;
                m_AI.enabled = true;
                m_AIControls.enabled = true;
                break; 
                

        }




        if(m_ControllerModes == types.None)
        {
            m_Player.enabled = false;
            m_AI.enabled = false;
            m_AIControls.enabled = false;
            m_PlayerInput.enabled = false; 
        }
        if (m_ControllerModes == types.Player)
        {

            m_Player.enabled = true;
            m_PlayerInput.enabled = true;
            m_AI.enabled = false;
            m_AIControls.enabled = false;
        }
        if (m_ControllerModes == types.None)
        {
            m_Player.enabled = false;
            m_PlayerInput.enabled = false;
            m_AI.enabled = true;
            m_AIControls.enabled = true;
        }
    }
}
