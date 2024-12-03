using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
public enum AIMouth
{
    racing,
    slowing_Down,
    speeding_Up,
    reversing
}


public class AI : MonoBehaviour
{
    public enum aI_Difficulty
    {
        raceStart,
        easy,
        normal,
        hard
    }
    public enum AI_Brain
    {
        driving,
        HitWall,

    }
    public Class classforAI;
    public AIMouth aiSpeaking = AIMouth.racing;
    public aI_Difficulty difficultness;
    [SerializeField] Rigidbody rb;
    [Header("Adjust the sensor for AI distance")]
    Ray[] rays = new Ray[10];
    Ray frontRay;
    Ray leftRay;
    Ray rightRay;
    Vector3 direction = Vector3.forward;
    [SerializeField] float range;
    AI_Controls carAI;
    [SerializeField] GameObject m_AICarBody;
    [SerializeField] GameObject m_AICarBodyDetection;
    [SerializeField] float steer_Value;
    [SerializeField] float adjustRayLeft;
    [SerializeField] float adjustRayRight;
    public float forceTurn = 25000f;
    [HideInInspector] public float acceration_Value;
    public float speed_Reader;
    public float speed_Limiter = 200f;

    //checking waypoints
    [Header("Waypoints system")]
    public TrackWayPoints[] waypoints = new TrackWayPoints[0];
    public List<Transform> nodes => waypoints[currentWaypointchange].trackNodes;
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public float steeringForce;
    public Transform currentWaypoint;
    public int currentWaypointIndex;
    [SerializeField] float minimumWayPointApproachThreshold;
    [SerializeField] float maximumWayPointApproachThreshold;
    public int numberOfLaps;

    public Tracking_Manager_Script valueBeingRead;
    [SerializeField] GameObject bridge;


    // Start is called before the first frame update
    void Start()
    {
        //nodes = waypoints.trackNodes;
        carAI = m_AICarBody.GetComponent<AI_Controls>();
        rb = m_AICarBody.GetComponentInChildren<Rigidbody>();
        rewindback = GetComponent<AIRewind>();
        difficultness = aI_Difficulty.raceStart;
        carAI.acceration_Value = 0f;
        currentWaypoint = waypoints[currentWaypointchange].transform;
        // difficultness = (aI_Difficulty)Random.Range(1, 3);
        //AIChange();

        //bridge = GameObject.Find("Checkpoints");
        //valueBeingRead = FindObjectOfType<Tracking_Manager_Script>();
        //nodes = waypoints.trackNodes;

    }

    void Awake()
    {
        //waypoints 
        //waypoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponent<TrackWayPoints>();
        //nodes = waypoints.trackNodes;

    }
    // Update is called once per frame
    void Update()
    {



    }

    int change;
    public int currentWaypointchange;
    int waypointLength;
    public void AIChange()
    {
        waypointLength = waypoints.Length - 1;

        switch (classforAI)
        {
            case Class.Light:
                //currentWaypointchange = Random.Range(0,3);
                //currentWaypointchange = Random.Range(0, 2);
                if (difficultness == aI_Difficulty.easy)
                {
                    currentWaypointchange = 0;
                }
                else if (difficultness == aI_Difficulty.normal)
                {
                    currentWaypointchange = 1;
                }
                else if (difficultness == aI_Difficulty.hard)
                {
                    currentWaypointchange = 2;
                }
                break;

            case Class.Medium:
               //  currentWaypointchange = Random.Range(3, 5);
                //currentWaypointchange = Random.Range(3, 6);
                if (difficultness == aI_Difficulty.easy)
                {
                    currentWaypointchange = 3;
                }
                else if (difficultness == aI_Difficulty.normal)
                {
                    currentWaypointchange = 4;
                }
                else if (difficultness == aI_Difficulty.hard)
                {
                    currentWaypointchange = 5;
                }
                break;

            case Class.Heavy:
               // currentWaypointchange = Random.Range(6, 9);
                //  currentWaypointchange = Random.Range(6, 8);
                if (difficultness == aI_Difficulty.easy)
                {
                    currentWaypointchange = 6;
                }
                else if (difficultness == aI_Difficulty.normal)
                {
                    currentWaypointchange = 7;
                }
                else if (difficultness == aI_Difficulty.hard)
                {
                    currentWaypointchange = 8;
                }
                break;

        }

    }
    private void FixedUpdate()
    {
        carAI.acceration_Value = acceration_Value;
        speed_Reader = carAI.currentSpeed;
        //Sensor();
        //CalculateDistanceOfWaypoints();
        changingDistanceOffset();
        CheckForUpdatedWaypoints();
        AISteer();
        AIState();
        Debug.Log(waypoints[currentWaypointchange]);
      

    }
    private void definingRays()
    {
        frontRay = new Ray(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(direction * range));
        leftRay = new Ray(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(new Vector3(adjustRayLeft, 0, 1) * range));
        rightRay = new Ray(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(new Vector3(adjustRayRight, 0, 1) * range));

    }
    private void Sensor()
    {
        definingRays();

        // FrontRaySensor();
        //LeftRaySensor();
        //RightRaySensor();



    }

    private void LeftRaySensor()
    {
        // raycast Left if comes in contact
        if (Physics.Raycast(leftRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("AI"))
            {
                rb.AddForce(rb.transform.right * forceTurn);
            }
            else if (hit.collider.CompareTag("walls"))
            {
                rb.AddForce(rb.transform.right * forceTurn);
            }
        }
        else
        {
            carAI.steering_Value = 0;
        }
    }
    private void RightRaySensor()
    {
        // raycast Right if comes in contact
        if (Physics.Raycast(rightRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("AI"))
            {
                rb.AddForce(-rb.transform.right * forceTurn);
            }
            else if (hit.collider.CompareTag("walls"))
            {
                rb.AddForce(-rb.transform.right * forceTurn);
            }
        }
    }
    private void FrontRaySensor()
    {
        // raycast front if comes in contact
        if (Physics.Raycast(frontRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("AI"))
            {
                rb.AddForce(-rb.transform.forward * forceTurn);
            }
            else if (hit.collider.CompareTag("walls"))
            {
                rb.AddForce(-rb.transform.forward * forceTurn);
            }
        }
    }

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
                currentWaypoint = nodes[i + distanceOffset];
                distance = currentDistance;

            }


        }
    }

    [SerializeField] float dists;
    [SerializeField] Vector3 difference;
    private void CheckForUpdatedWaypoints() //call this every update
    {
        Vector3 position = rb.transform.position;
        //These should be defined in the class not as local variables
        // int currentWaypointIndex;
        //float waypointApproachThreshold;
        carAI.maxSpeed = speed_Limiter;
        difference = nodes[currentWaypointIndex].transform.position - position;
        dists = Vector3.Distance(position, nodes[currentWaypointIndex].transform.position);
        if (difference.magnitude < minimumWayPointApproachThreshold)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= nodes.Count;
            ChangeMaxSpeed();
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


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(nodes[currentWaypointIndex].transform.position, 3);
        var collider = GetComponentInChildren<Collider>();
        if (!collider)
        {
            return;
        }
        var collisionPoint = collider.ClosestPoint(location);
        Gizmos.DrawSphere(location, 0.1f);
        Gizmos.DrawWireSphere(collisionPoint, 0.1f);
    }


    private void AISteer()
    {
        Vector3 targetPosition = nodes[(currentWaypointIndex + distanceOffset) % nodes.Count].transform.position;
        Vector3 relative = rb.transform.InverseTransformPoint(/*currentWaypoint.transform.position*/targetPosition);
        relative /= relative.magnitude;
        carAI.steering_Value = steer_Value;
        steer_Value = (relative.x / relative.magnitude) * steeringForce;
    }


    private void changingDistanceOffset()
    {
        switch (difficultness)
        {
            case aI_Difficulty.raceStart:
                acceration_Value = 0f;
                break;
            case aI_Difficulty.easy:

                acceration_Value = 1f;
                carAI.downForceValue = 700f;

                break;
            case aI_Difficulty.normal:

                acceration_Value = 1.2f;
                carAI.downForceValue = 700f;
                break;
            case aI_Difficulty.hard:

                acceration_Value = 1.5f;
                carAI.downForceValue = 1000f;

                break;



        }


        if (speed_Reader > 150 && speed_Reader < 250)
        {
            if (difficultness == aI_Difficulty.easy)
            {
                distanceOffset = 2;
                acceration_Value = 1f;
                minimumWayPointApproachThreshold = 17f;
                carAI.downForceValue = 800f;
            }
            if (difficultness == aI_Difficulty.normal)
            {
                distanceOffset = 2;
                acceration_Value = 1.2f;
                minimumWayPointApproachThreshold = 17f;
                carAI.downForceValue = 800f;
            }
            if (difficultness == aI_Difficulty.hard)
            {
                distanceOffset = 3;
                acceration_Value = 1.5f;
                steeringForce = 1.2f;
                minimumWayPointApproachThreshold = 20f;
                carAI.downForceValue = 1000;
            }
        }

        if (speed_Reader > 50 && speed_Reader < 150)
        {

            if (difficultness == aI_Difficulty.easy)
            {
                distanceOffset = 2;
                acceration_Value = 1f;
                minimumWayPointApproachThreshold = 17f;

            }
            if (difficultness == aI_Difficulty.normal)
            {
                distanceOffset = 2;
                acceration_Value = 1.2f;
                minimumWayPointApproachThreshold = 17f;
            }

            if (difficultness == aI_Difficulty.hard)
            {
                distanceOffset = 3;
                acceration_Value = 1.5f;
                minimumWayPointApproachThreshold = 25f;
            }
            /*distanceOffset = 2;
            acceration_Value = 1;*/
        }
        if (speed_Reader > 200)
        {
            if (difficultness == aI_Difficulty.normal)
            {
                distanceOffset = 2;
                minimumWayPointApproachThreshold = 17f;
                carAI.downForceValue = 900f;
            }
            if (difficultness == aI_Difficulty.hard)
            {
                distanceOffset = 4;
                steeringForce = 1f;
                minimumWayPointApproachThreshold = 20f;
                carAI.downForceValue = 1000f;
            }

        }

        if (speed_Reader < 50)
        {
            distanceOffset = 1;
            steeringForce = 1f;
            minimumWayPointApproachThreshold = 20f;
            carAI.downForceValue = 500f;
            if (Physics.Raycast(frontRay, out RaycastHit hit, range))
            {
                minimumWayPointApproachThreshold = 50f;
                if (hit.collider.CompareTag("walls"))
                {
                    steeringForce = 1.5f;
                    carAI.acceration_Value = -2f;
                    // carAI.steering_Value = -carAI.steering_Value;
                }
                else if (!hit.collider.CompareTag("walls"))
                {
                    carAI.acceration_Value = 1.0f;
                    minimumWayPointApproachThreshold = 20f;
                    steeringForce = 1f;
                }
            }
        }

    }

    private void ChangeMaxSpeed()
    {
        if (nodes[currentWaypointIndex].gameObject.CompareTag("AccerateNode"))
        {
            if(difficultness == aI_Difficulty.easy)
            {
                speed_Limiter = valueBeingRead.changingSpeedToAccerate - 50;
            }
           else if (difficultness == aI_Difficulty.normal)
            {
                speed_Limiter = valueBeingRead.changingSpeedToAccerate - 20;
            }
            else
            speed_Limiter = valueBeingRead.changingSpeedToAccerate;
        }
        else if (nodes[currentWaypointIndex].gameObject.CompareTag("SlowNode"))
        {
            if (difficultness == aI_Difficulty.easy)
            {
                speed_Limiter = valueBeingRead.changingSpeedToAccerate - 50;
            }
            else if (difficultness == aI_Difficulty.normal)
            {
                speed_Limiter = valueBeingRead.changingSpeedToAccerate - 20;
            }
            else
            speed_Limiter = valueBeingRead.changingSpeedToSlowDown;
        }
        else if(nodes[currentWaypointIndex].gameObject.CompareTag("BrakeNode"))
        {
            if (carAI.currentSpeed > valueBeingRead.changingSpeedToSlowDown)
            {
                carAI.brakes_value = 1;
            }
            else
            {
                carAI.brakes_value = 0f;
            }

        }
            return;

    }

    public float speedTimer = 5;
    public float activeTime = 5;
    public float cooldownTimer = 5;
    public AIRewind rewindback;
    private void AIState()
    {
        switch (aiSpeaking)
        {

            case AIMouth.racing:

                carAI.brakes_value = 0;
                switch (difficultness)
                {
                    case aI_Difficulty.raceStart:
                        acceration_Value = 0f;
                        break;
                    case aI_Difficulty.easy:

                        acceration_Value = 1f;


                        break;
                    case aI_Difficulty.normal:

                        acceration_Value = 1.2f;

                        break;
                    case aI_Difficulty.hard:

                        acceration_Value = 1.5f;

                        break;



                }
                break;

            case AIMouth.speeding_Up:
                var collider = GetComponentInChildren<BoxCollider>();
                if (speedTimer > 0)
                {

                    //collider.enabled = false;
                    speedTimer -= Time.deltaTime;
                    switch (difficultness)
                    {
                        case aI_Difficulty.raceStart:
                            acceration_Value = 0f;

                            break;

                        case aI_Difficulty.easy:

                            acceration_Value = 3f;


                            break;
                        case aI_Difficulty.normal:

                            acceration_Value = 3.5f;

                            break;
                        case aI_Difficulty.hard:

                            acceration_Value = 4f;


                            break;



                    }
                }

                else
                {

                    // collider.enabled = true;
                    aiSpeaking = AIMouth.racing;
                    speedTimer = activeTime;
                }
                break;

            case AIMouth.slowing_Down:
                rb.AddForce(-rb.transform.forward + rb.transform.right * forceTurn);
                if (cooldownTimer > 0)
                {
                    cooldownTimer -= Time.deltaTime;
                }
                else
                {

                    aiSpeaking = AIMouth.racing;
                    cooldownTimer = Random.Range(2, 6);
                }

                break;
            case AIMouth.reversing:

                //acceration_Value = -3f; 
                if (carAI.currentSpeed < 30f)
                {
                    rewindback.Rewind();
                    //gameObject.transform.position =Vector3.Lerp(transform.position, nodes[currentWaypointIndex - 2].transform.position, 1);
                    //gameObject.transform.LookAt(nodes[currentWaypointIndex + 1].transform.position);
                    //rb.velocity = rb.velocity / 2;
                }
                break;
        }
    }
    Vector3 location;
    private void OnTriggerEnter(Collider other)
    {

        if (difficultness == aI_Difficulty.raceStart)
        {

        }
        else
        {
            if (other.CompareTag("AIBody"))
            {
                if (aiSpeaking == AIMouth.speeding_Up)
                {
                    rb.AddForce(rb.transform.right * forceTurn);
                }

                else
                {
                    aiSpeaking = AIMouth.slowing_Down;
                }

            }
         
        }
    }
    public float rewindTimer;

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("AIBody"))
        {
            float otherCarSpeed = GetComponentInParent<AI_Controls>().currentSpeed;
            if (otherCarSpeed < carAI.currentSpeed)
            {
                carAI.brakes_value = 1;
                carAI.acceration_Value = 0;
                if (carAI.currentSpeed <= otherCarSpeed)
                {
                    carAI.brakes_value = 0;
                    carAI.currentSpeed = otherCarSpeed;
                }

            }
            aiSpeaking = AIMouth.slowing_Down;

        }
        if (other.CompareTag("walls"))
        {
            if (rewindTimer > 2)
            {
                rewindTimer = 0;
                aiSpeaking = AIMouth.reversing;
            }
            else
            {
                rewindTimer += Time.fixedDeltaTime;
            }

        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AIBody"))
        {

            //difficultness = (aI_Difficulty)Random.Range(1, 4);

            if (Random.value >= 0.5f)
            {
                aiSpeaking = AIMouth.racing;
            }
            if (Random.value >= 0.7)
            {
                aiSpeaking = AIMouth.speeding_Up;
            }
        }
        if (other.CompareTag("walls"))
        {
            aiSpeaking = AIMouth.racing;
        }

    }


}
