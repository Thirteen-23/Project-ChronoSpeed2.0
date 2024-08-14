using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public TrackWayPoints waypoints;

    public List<Transform> nodes = new List<Transform>();
    [Range (0 ,10)] public int distanceOffset;
    public Transform currentWaypoint; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        waypoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponent<TrackWayPoints>();
        nodes = waypoints.trackNodes; 
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity; 

        for(int i = 0; i< nodes.Count; i++)
        {
            Vector3 difference = nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if(currentDistance < distance)
            {
                currentWaypoint = nodes[i + distanceOffset];
                distance = currentDistance;

            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(currentWaypoint.transform.position, 3);
    }
}
