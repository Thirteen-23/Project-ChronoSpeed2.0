using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tracking_Manager_Script : MonoBehaviour
{
    CheckFlagPoint m_Sensors;
    public List<Transform> checkpointNodes = new List<Transform>();


    public List<GameObject> assigningNodes = new List<GameObject>();
    public float changingSpeedToAccerate;
    public float changingSpeedToSlowDown;


    public Color wayPointColour;
    [Range(0, 1)] public float sphereRadius;
    public List<Transform> trackCheckpoints = new List<Transform>();

    public GameObject[] m_listOfCars;
    public List<GameObject> presentGameCars;
   
    //public Car_Movement rR;
    private void Awake()
    {
       // rR = GameObject.FindGameObjectWithTag("Player").GetComponent<Car_Movement>();
        presentGameCars = new List<GameObject>();
        foreach (GameObject r in m_listOfCars)
        {
            presentGameCars.Add(r);
        }
        //presentGameCars.Add(rR.gameObject);

    }
    void Start()
    {

        Transform[] paths = GetComponentsInChildren<Transform>();
        checkpointNodes = new List<Transform>();
        for (int i = 1; i < paths.Length; i++)
        {
            checkpointNodes.Add(paths[i]);

        }

        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            assigningNodes.Add(child.gameObject);
        }
        // CheckpointPass();
      //  m_Sensors = GetComponentInChildren<CheckFlagPoint>();
    }
    
   
    public TextMeshProUGUI textOfItems;
    GameObject playerT;
    int positionForPlayer;
    // Update is called once per frame
    void Update()
    {
        m_listOfCars = m_listOfCars.OrderBy(gameObject => -gameObject.GetComponent<LapManager>().currentNode).ToArray();
        m_listOfCars = m_listOfCars.OrderBy(gameObject => -gameObject.GetComponent<LapManager>().lapCompeted).ToArray();
        presentGameCars = new List<GameObject>();
        foreach (GameObject r in m_listOfCars)
        {
            presentGameCars.Add(r);
        }
        playerT = GameObject.FindGameObjectWithTag("Player");
        positionForPlayer = presentGameCars.IndexOf(playerT);
        textOfItems.text = (positionForPlayer + 1) + "/" + presentGameCars.Count;


    }
}
