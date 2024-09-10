using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Tracking_Manager_Script : MonoBehaviour
{
    CheckFlagPoint m_Sensors;
    public List<Transform> checkpointNodes = new List<Transform>();


    public List<GameObject> assigningNodes = new List<GameObject>();
    public float changingSpeedToAccerate;
    public float changingSpeedToSlowDown;

    public List<Transform> trackCheckpoints = new List<Transform>();

    public List<TrackedInfo> TrackedCars = new List<TrackedInfo>();
    public List<TrackedInfo> FinishedCars = new List<TrackedInfo>();

    public TrackWayPoints waypoints;
    private List<Transform> nodes => waypoints.trackNodes;

    public float distanceOffset = 0.5f;
    public class TrackedInfo : INetworkSerializable
    {

        public GameObject Car;
        public int CurLap;
        public int Place;
        public float raceCompletedIn;
        public int ClosestNode;
        public bool completed;

        public TrackedInfo(GameObject car)
        {
            Car = car;
            CurLap = 0;
            Place = 0;
            ClosestNode = 0;
            completed = false;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CurLap);
            serializer.SerializeValue(ref Place);
            serializer.SerializeValue(ref raceCompletedIn);
            serializer.SerializeValue(ref ClosestNode);
            serializer.SerializeValue(ref completed);
        }
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
        m_Sensors = GetComponentInChildren<CheckFlagPoint>();
    }

    private void SortTrackedCars()
    {
        //Vector3 position = curCar.position;
        //float distance = Mathf.Infinity;

        
        
        for(int a = 0; a < TrackedCars.Count; a++)
        {
            float closestDistance = Mathf.Infinity;
            for(int b = 0; b < nodes.Count; b++)
            {
                float tempDistance = Vector3.Distance(TrackedCars[a].Car.transform.position, nodes[b].position);
                if (tempDistance > closestDistance)
                    continue;
                
                closestDistance = tempDistance;
                TrackedCars[a].ClosestNode = b;
            }
        }

        TrackedCars.OrderByDescending(pluh => pluh.ClosestNode);

        //idk which is better this, 42 comparisons at worst ?(maybe i dont know i think it uses quicksort) and 1 at best? (im assuming c# makers are better then me so this one)
        TrackedCars.OrderByDescending(pluh => pluh.CurLap);
        for (int i = 0; i < TrackedCars.Count; i++)
        {
            TrackedCars[i].Place = i + 1;
        }    

        //or this, 36 comparisons at worst, 12 at best
        //int placeToGive = 1;
        //for(int a = 3; a > 0; a--)
        //{
        //    for (int i = 0; i < trackCars.Count; i++)
        //    {
        //        if (trackCars[i].CurLap != a)
        //            continue;

        //        trackCars[i].Place = placeToGive;
        //        placeToGive++;

        //        if (placeToGive == trackCars.Count + 2)
        //            return;
        //    }
        //}
        
    }

    public void AddTrackedCar(GameObject car)
    {
        TrackedCars.Add(new TrackedInfo(car));
    }
    public void FinishTrackedCar(GameObject car)
    {
        for(int i = 0;i < TrackedCars.Count; i++)
        {
            if(TrackedCars[i].Car == car)
            {
                FinishedCars.Add(TrackedCars[i]);
                TrackedCars.RemoveAt(i);
                return;
            }
        } 
            
    }
   
    // Update is called once per frame
    void Update()
    {
        //presentGameCars.OrderBy(gameObject => -gameObject.GetComponent<LapManager>().currentNode);
        //presentGameCars.OrderBy(gameObject => -gameObject.GetComponent<LapManager>().lapCompeted);
        SortTrackedCars();

    }
}
