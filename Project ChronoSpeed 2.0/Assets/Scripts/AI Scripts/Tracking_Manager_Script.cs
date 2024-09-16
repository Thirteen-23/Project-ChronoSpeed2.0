using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Tracking_Manager_Script : MonoBehaviour
{
    public TrackWayPoints waypoints;
    public List<Transform> nodes => waypoints.trackNodes;

    public List<TrackedInfo> TrackedCars = new List<TrackedInfo>();
    public List<TrackedInfo> FinishedCars = new List<TrackedInfo>();

    [SerializeField] private int maxLaps = 3;

    public float startTime;
    public float changingSpeedToAccerate = 300;
    public float changingSpeedToSlowDown = 200;

    [Serializable]
    public class TrackedInfo : INetworkSerializable
    {

        public GameObject Car;
        public List<Transform> HitCheckpoints;

        public int CurLap;
        public int Place;
        public double raceCompletedIn;
        public int ClosestNode;

        public TrackedInfo(GameObject car)
        {
            Car = car;
            HitCheckpoints = new List<Transform>();

            CurLap = 0;
            Place = 0;
            ClosestNode = 0;
        }
        public TrackedInfo()
        {
            Car = null;
            HitCheckpoints = new List<Transform>();

            CurLap = -1;
            Place = -1;
            raceCompletedIn = -1;
            ClosestNode = -1;

        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CurLap);
            serializer.SerializeValue(ref Place);
            serializer.SerializeValue(ref raceCompletedIn);
            serializer.SerializeValue(ref ClosestNode);
        }
    }


    public void CarHitCheckPoint(GameObject car, Transform checkPoint)
    {
        for(int i = 0; i < TrackedCars.Count; i++)
        {
            if (TrackedCars[i].Car != car) continue;

            //Has it already hit this checkpoint this lap
            if (TrackedCars[i].HitCheckpoints.Contains(checkPoint))
                break;

            TrackedCars[i].HitCheckpoints.Add(checkPoint);
        }
    }

    public void CarHitFirstCheckPoint(GameObject car, Transform checkPoint)
    {
        for (int i = 0; i < TrackedCars.Count; i++)
        {
            if (TrackedCars[i].Car != car) continue;
            
            //all the checkpoints are children of this object, might need to change if the other managers ever need children since all on same object
            if (TrackedCars[i].HitCheckpoints.Count != transform.childCount)
            {
                CarHitCheckPoint(car, checkPoint);
                return;
            }
            else
            {
                TrackedCars[i].CurLap++;
                TrackedCars[i].HitCheckpoints.Clear();

                if (TrackedCars[i].CurLap >= maxLaps)
                {
                    SortTrackedCars();
                    FinishTrackedCar(TrackedCars[i]);
                    return;
                }

                CarHitCheckPoint(car, checkPoint);
            }
        }
    }

    private void SortTrackedCars()
    {
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

        TrackedCars = TrackedCars.OrderByDescending(pluh => pluh.ClosestNode).ToList();

        //idk which is better this, 42 comparisons at worst ?(maybe i dont know i think it uses quicksort) and 1 at best? (im assuming c# makers are better then me so this one)
        TrackedCars = TrackedCars.OrderByDescending(pluh => pluh.CurLap).ToList();

        for (int i = 0; i < TrackedCars.Count; i++)
        {
            TrackedCars[i].Place = i + 1 + FinishedCars.Count;
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
    public void FinishTrackedCar(TrackedInfo finishingCar)
    {
        finishingCar.raceCompletedIn = Time.timeSinceLevelLoadAsDouble - startTime;
        FinishedCars.Add(finishingCar);
        TrackedCars.Remove(finishingCar);
    }
   
    // Update is called once per frame
    void Update()
    {
        SortTrackedCars();
    }
}
