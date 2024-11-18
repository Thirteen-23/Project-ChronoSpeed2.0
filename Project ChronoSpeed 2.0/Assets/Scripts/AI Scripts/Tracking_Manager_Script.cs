using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    
    public class TrackedInfo 
    {

        public GameObject Car;
        public List<Transform> HitCheckpoints;
        public bool IsPlayer;
        public int ClosestNode;

        public NetworkInfo netInfo;

        [Serializable]
        public struct NetworkInfo : INetworkSerializable, IEquatable<NetworkInfo>
        {
            public int CurLap;
            public int Place;
            public double raceCompletedIn;
            
            public ulong ClientID;

            public NetworkInfo(ulong clientID)
            {
                CurLap = 0;
                Place = 0;
                raceCompletedIn = 0;
                ClientID = clientID;
            }
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref CurLap);
                serializer.SerializeValue(ref Place);
                serializer.SerializeValue(ref raceCompletedIn);
                serializer.SerializeValue(ref ClientID);
            }

            public bool Equals(NetworkInfo other)
            {
                return CurLap == other.CurLap &&
                    Place == other.Place &&
                    raceCompletedIn == other.raceCompletedIn &&
                    ClientID == other.ClientID;
            }
        }

        

        public TrackedInfo(GameObject car, bool isPlayer, ulong clientID)
        {
            Car = car;
            HitCheckpoints = new List<Transform>();
            IsPlayer = isPlayer;

            netInfo = new NetworkInfo(clientID);
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
            var curCar = TrackedCars[i];
            //all the checkpoints are children of this object, might need to change if the other managers ever need children since all on same object
            if (curCar.HitCheckpoints.Count != transform.childCount)
            {
                CarHitCheckPoint(car, checkPoint);
                return;
            }
            else
            {
                curCar.netInfo.CurLap++;
                curCar.HitCheckpoints.Clear();

                if (curCar.netInfo.CurLap >= maxLaps)
                {
                    SortTrackedCars();
                    FinishTrackedCar(curCar);
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
        TrackedCars = TrackedCars.OrderByDescending(pluh => pluh.netInfo.CurLap).ToList();

        for (int i = 0; i < TrackedCars.Count; i++)
        {
            TrackedCars[i].netInfo.Place = i + 1 + FinishedCars.Count;
        }
    }

    public void AddTrackedCar(GameObject car, bool isPlayer, ulong clientID)
    {
        TrackedCars.Add(new TrackedInfo(car, isPlayer, clientID));
    }
    public void FinishTrackedCar(TrackedInfo finishingCar)
    {
        finishingCar.netInfo.raceCompletedIn = Time.timeSinceLevelLoadAsDouble - startTime;
        FinishedCars.Add(finishingCar);
        TrackedCars.Remove(finishingCar);
    }
   
    // Update is called once per frame
    void Update()
    {
        SortTrackedCars();
    }
}
