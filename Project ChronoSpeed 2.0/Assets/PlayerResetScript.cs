using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerResetScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unstuckText;
    [SerializeField] Car_Movement carscript;
    [SerializeField] float timer = -5;
    [SerializeField] bool checkPlayerIsStuck = false;
    
    #region Tracking stuff
    

    public Rigidbody rb;
    [SerializeField] float dists;
    [SerializeField] Vector3 difference;
    public TrackWayPoints[] waypoints = new TrackWayPoints[0];
    public List<Transform> nodes => waypoints[currentWaypointchange].trackNodes;
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public float steeringForce;
    public Transform currentWaypoint;
    public int currentWaypointIndex;
    [SerializeField] float minimumWayPointApproachThreshold;
    [SerializeField] float maximumWayPointApproachThreshold;
    public int numberOfLaps;
    public int currentWaypointchange = 5;
    #endregion
    void Start()
    {
        currentWaypoint = waypoints[currentWaypointchange].transform;
        timer = -5; 
         rb = GetComponentInParent<Rigidbody>();
        carscript = GetComponentInParent<Car_Movement>().gameObject.GetComponentInParent<Car_Movement>().gameObject.GetComponent<Car_Movement>();
        
      
    }
    private void Awake()
    {
        waypoints = FindObjectOfType<AIWayPointManager>().m_Waypoint_difficulties;
    }
    // Update is called once per frame
    void Update()
    {
        
        CheckForUpdatedWaypoints();
        WhenPlayerIsStuck();
      
    }

    private void WhenPlayerIsStuck()
    {
       
            if (carscript.currentSpeed < 5 || carscript.currentSpeed > 300)
            {
                timer += Time.deltaTime;
                if (timer >= 3)
                {
                    unstuckText.enabled = true;
                checkPlayerIsStuck = true; 
                }
            }
            else
            {
                timer = 0;
                unstuckText.enabled = false;
                checkPlayerIsStuck = false;
        }
        


    }
    public void MoveMe(InputAction.CallbackContext context)
    {
        if (checkPlayerIsStuck == true)
        {
            if (context.performed)
            {
                rb.transform.position = nodes[currentWaypointIndex].transform.position;
                rb.velocity = Vector3.zero; 
            }
        }
    }

    private void CheckForUpdatedWaypoints() //call this every update
    {
        Vector3 position = rb.transform.position;
        //These should be defined in the class not as local variables
        // int currentWaypointIndex;
        //float waypointApproachThreshold;

        difference = nodes[currentWaypointIndex].transform.position - position;
        dists = Vector3.Distance(position, nodes[currentWaypointIndex].transform.position);
        if (difference.magnitude < minimumWayPointApproachThreshold)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= nodes.Count;
        }

        if (difference.magnitude > maximumWayPointApproachThreshold)
        {
            //rb.transform.position = Vector3.Lerp(rb.transform.position, nodes[currentWaypointIndex-1].transform.position, Time.deltaTime);
            // rb.transform.position = nodes[currentWaypointIndex - 1].transform.position;
            // rb.transform.LookAt(nodes[currentWaypointIndex + 1].transform.position);


            float distance = Mathf.Infinity;

            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 difference = nodes[i].transform.position - position;
                float currentDistance = difference.magnitude;
                if (currentDistance < distance)
                {
                    currentWaypointIndex = i;
                    distance = currentDistance;

                }

            }
        }
    }
}
