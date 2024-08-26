using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tracking_Manager_Script : MonoBehaviour
{
    CheckFlagPoint m_Sensors; 
    public List<Transform> checkpointNodes = new List<Transform>();
    
    public GameObject[] m_listOfCars = new GameObject[0];
    public List<GameObject> assigningNodes = new List<GameObject>();
    public float changingSpeedToAccerate;
    public float changingSpeedToSlowDown;


    public Color wayPointColour;
    [Range(0, 1)] public float sphereRadius;
    public List<Transform> trackCheckpoints = new List<Transform>();
    void Start()
    {
        //children = new Transform[transform.childCount];
        //foreach (Transform child in transform)
        //{
        //    children[i++] = child.transform;

        //}
        

        
        
        Transform[] paths = GetComponentsInChildren<Transform>();
        checkpointNodes = new List<Transform>(); 
        for(int i = 1; i < paths.Length; i++)
        {
            checkpointNodes.Add(paths[i]);
            
        }

        foreach(Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            assigningNodes.Add(child.gameObject);
        }
       // CheckpointPass();
        m_Sensors = GetComponentInChildren<CheckFlagPoint>(); 
    }

    // Update is called once per frame
    void Update()
    {
        m_listOfCars = m_listOfCars.OrderBy(gameObject => gameObject.GetComponent<AI>().currentWaypointIndex).ToArray();
       

        // listOfCars = m_Sensors.listOfCars;

    }

    private void CheckpointPass()
    {
       //for(int i = 1; i < tester.Capacity ; i++)
       // {
       //     tester[i].AddComponent<CheckFlagPoint>();
       // }

       //foreach(Transform child in gameObject.transform)
       // {
       //     child.AddComponent<CheckFlagPoint>();
       // }
        int childCount = gameObject.transform.childCount;
        for(int x = 0; x < childCount; x++)
        {
            gameObject.transform.GetChild(x).AddComponent<CheckFlagPoint>();
        }

    }
  

}
