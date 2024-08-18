using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum types
{
    braking,
    accerating
}
public class Tracking_Acceleration_Braking : MonoBehaviour
{
    public AI m_AIControl; 
    
    public types postsForAI;
    public float speed_Check;

    [Header("Exiting Corner values")]
    public float m_AIAccerationValueChange;
    public int distanceOffset_AccerationChange;
   

    [Header("Entering Corner values")]
    public float m_AISlowDownValueChange;
    public int distanceOffset_BrakeChange;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Awake()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
    

    }

    private void OnTriggerEnter(Collider other)
    {
        m_AIControl = other.gameObject.GetComponentInParent<AI>();
        
        if (postsForAI == types.braking)
        {
            if (other.gameObject.CompareTag("AI"))
            {
               // Debug.Log("tag");
                if (speed_Check < m_AIControl.speed_Reader)
                { 
                    m_AIControl.acceration_Value = m_AISlowDownValueChange;
                    m_AIControl.distanceOffset = distanceOffset_BrakeChange;
                }
                else if(speed_Check > m_AIControl.speed_Reader) 
                {
                   
                    m_AIControl.distanceOffset = distanceOffset_BrakeChange - 1;
                }
            }
        }

        if (postsForAI == types.accerating)
        {
           // Debug.Log("tag");
            if (speed_Check < m_AIControl.speed_Reader)
            {
                m_AIControl.acceration_Value = m_AIAccerationValueChange;
                m_AIControl.distanceOffset = distanceOffset_AccerationChange;
            }
            else if (speed_Check > m_AIControl.speed_Reader)
            {

                m_AIControl.distanceOffset = distanceOffset_AccerationChange - 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("AI"))
        {
           // Debug.Log(" not tagged");

           
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("AI"))
        {
           // Debug.Log(" tag stayed");

        }
    }
}
