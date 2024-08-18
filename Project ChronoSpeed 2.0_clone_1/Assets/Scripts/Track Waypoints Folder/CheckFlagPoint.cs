using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFlagPoint : MonoBehaviour
{
    public List<GameObject> listOfCars = new List<GameObject>();
    [HideInInspector] public AI m_AI;
    [HideInInspector] public Car_Movement m_CarMovementAccess;
    Tracking_Manager_Script valueBeingRead;
    GameObject bridge; 
    // Start is called before the first frame update
    void Start()
    {
        bridge = GameObject.Find("Checkpoints");
        valueBeingRead = bridge.GetComponent<Tracking_Manager_Script>();
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.tag == "Player")
        {
            Debug.Log("has passed");
            listOfCars.Add(other.gameObject);
            m_CarMovementAccess = other.gameObject.GetComponentInParent<Car_Movement>();
            m_CarMovementAccess.numberOfLaps++; 
        }
        if (other.tag == "AI")
        {
            Debug.Log(" AI has passed");
            listOfCars.Add(other.gameObject);
            m_AI = other.gameObject.GetComponentInParent<AI>();
            m_AI.numberOfLaps++;
        }
    }
}
