using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public List<Collider> checkPointHit = new List<Collider>();
    public int checkpointCount = 0;
    public int totalCheckpoints;
    public int lapCompeted = 0;

    [Range(0, 10)] public int distanceOffset;
    public Rigidbody rb;
    public TrackWayPoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    public Transform currentWaypoint;
    public int currentWaypointIndex;

    public GameObject[] presentCars; 
    // Start is called before the first frame update
    void Start()
    {
        nodes = waypoints.trackNodes;
    }
    private void Awake()
    {
        if (rb == true)
        {
            rb = GetComponent<Rigidbody>();
        }
        if(rb == null)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateDistanceOfWaypoints();
    }

    public int currentNode;
    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = rb.transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 difference = nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if (currentDistance < distance)
            {
                if ((i + distanceOffset) >= nodes.Count)
                {
                    currentWaypoint = nodes[i];
                    distance = currentDistance;
                   
                }
                else
                {
                    currentWaypoint = nodes[i + distanceOffset];
                    distance = currentDistance;
                }
                currentNode = i;
            }


        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!checkPointHit.Contains(other))
        {
            checkpointCount++;
            checkPointHit.Add(other);

        }
        if (checkpointCount == totalCheckpoints)
        {
            lapCompeted++;
            checkpointCount = 0;
            checkPointHit.Clear();

        }
    }
}
