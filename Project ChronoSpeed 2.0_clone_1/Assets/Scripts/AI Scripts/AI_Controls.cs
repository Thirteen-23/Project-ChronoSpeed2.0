using UnityEngine;

public class AI_Controls : MonoBehaviour
{
    enum DifferentialTypes
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive

    }
    enum TransmissionTypes
    {
        Automatic,
        Manual
    }
    [SerializeField] TransmissionTypes transmission;
    [SerializeField] DifferentialTypes drive;
    [SerializeField] Rigidbody bodyOfCar;
    [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];
    [SerializeField] Transform centerMass;
    //[SerializeField] AnimationCurve gearRatio;

    float currentBreakForce, handbraking;

    [Header("Speed and Power of the Car")]
    [SerializeField] public AnimationCurve enginePower;
    public float maxSpeed;
    [SerializeField] float totalPowerInCar;
    public float currentSpeed;
    // dampening for smoother acceration input for keyboard 
    public float acceration_Value;
    [SerializeField] float AccerationDamping;
    public float downForceValue;

    [Header("GearBox System")]
    [SerializeField] public int idleRPM;
    [SerializeField] float maxRPM;
    [SerializeField] float minRPM;
    [SerializeField] int maxRPMForCar;
    [SerializeField] public float engineRPM;
    [SerializeField] float finalDriveRatio;
    [SerializeField] float[] gearSpeedBox = new float[0];
    [SerializeField] public int gearNum;
    private float m_RPMOfWheels;
    [SerializeField] float smoothTime;
    [SerializeField] Vector2[] keyRPMSet = new Vector2[0];

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] bool isBreaking;
    public bool ifHandBraking;
    WheelFrictionCurve sidewaysFriction, forwardFriction;
    public float handBrakefrictionMulitplier = 2f;
    float handbrakefriction;
    float tempo;
    [SerializeField] float whenNotDrifting;
    [SerializeField] float whenDrifting;


    [Header("Handling & Brakes")]
    [SerializeField] float allBrakeForce;
    [SerializeField] float frontBrakeForce;
    [SerializeField] float rearBrakeForce;
    public float steering_Value;
    // make the steering smoother when useing a  keyboard 
    [SerializeField] float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    [SerializeField] float brakes_value;
    [SerializeField] float brakeDampening;
    private float turnSpeed;
    public AnimationCurve steeringCurve;

    //drafting values
    Ray draftingRay;
    Vector3 direction = Vector3.forward;
    [SerializeField] float m_RayRange;

    private void Awake()
    {

    }

    void Start()
    {
        bodyOfCar.centerOfMass = centerMass.localPosition;
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;

    }


    private void FixedUpdate()
    {

        HandlingMotor();
        HandlingSteering();
        AnimatedWheels();
        DampeningSystem();
        calculatingEnginePower();
        Shifting();
        SetEngineRPMAndTorque();
        ApplyingDownForce();
        Drafting();
        AdjustTractionForDrifting();
    }

    private float SmoothTransition(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * smoothTransitionSpeed);
    }

    private void DampeningSystem()
    {
        AccerationDamping = SmoothTransition(acceration_Value, AccerationDamping);
        steeringDamping = SmoothTransition(steering_Value, steeringDamping);
        brakeDampening = SmoothTransition(brakes_value, brakeDampening);
    }

    private void HandlingMotor()
    {
        // calculation of kilometers / hour
        currentSpeed = bodyOfCar.velocity.magnitude * 3.6f;
        EngineRPMSystem();
        // code for restricting the car to max speed set. 
        if (currentSpeed < maxSpeed)
        {
            #region New Driving system 
            if (drive == DifferentialTypes.AllWheelDrive)
            {
                for (int i = 0; i < wheels4.Length; i++)
                {
                    // wheels torque equal to engine Rpm * gearbox * final drive ratio and input from player
                    wheels4[i].motorTorque = totalPowerInCar / 4;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar / 2;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar / 2;
                }
            }

            #endregion


        }

        else
        {
            if (drive == DifferentialTypes.AllWheelDrive)
            {
                for (int i = 0; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = acceration_Value * 0;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = acceration_Value * 0;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = acceration_Value * 0;
                }
            }

        }
        if (brakes_value > 0.7f)
        {
            isBreaking = true;
        }
        currentBreakForce = isBreaking ? (allBrakeForce * brakeDampening) : 0f;
        //currentBreakForce =  allBrakeForce * brakeDampening;
        handbraking = ifHandBraking ? rearBrakeForce : 0f;
        ApplyBraking();
        ApplyHandBraking();
    }

    private void ApplyBraking()
    {
        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce * Time.deltaTime;
        }




    }

    private void ApplyHandBraking()
    {
        for (int i = 2; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = handbraking;
        }
    }


    private void HandlingSteering()
    {
        for (int i = 0; i < wheels4.Length - 2; i++)
        {
            turnSpeed = steeringDamping * steeringCurve.Evaluate(currentSpeed);
            wheels4[i].steerAngle = turnSpeed;
        }

    }

    void AnimatedWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotations = Quaternion.identity;

        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].GetWorldPose(out wheelPosition, out wheelRotations);
            wheelmeshes[i].transform.position = wheelPosition;
            wheelmeshes[i].transform.rotation = wheelRotations;
        }

    }

    private void calculatingEnginePower()
    {
        EngineRPMSystem();

        //totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * m_PlayerAcceration;
        totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * AccerationDamping;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, idleRPM + (Mathf.Abs(m_RPMOfWheels) * finalDriveRatio * (gearSpeedBox[gearNum])), ref velocity, smoothTime);
    }


    private void EngineRPMSystem()
    {
        float sum = 0;
        int rR = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels4[i].rpm;
            rR++;
        }
        m_RPMOfWheels = (rR != 0) ? sum / rR : 0;

    }
    private void Shifting()
    {
       
        if (transmission == TransmissionTypes.Automatic)
        {
            if (engineRPM >= maxRPM)
            {
                if (gearNum < gearSpeedBox.Length - 1)
                {
                    gearNum++;
                }
            }
            if (engineRPM <= minRPM)
            {
                if (gearNum > 0)
                {
                    gearNum--;
                }
                else
                {
                    gearNum = 0;
                }
            }
        }
    }

    private void SetEngineRPMAndTorque()
    {
        for (int i = 0; i < keyRPMSet.Length; i++)
        {
            enginePower.AddKey(keyRPMSet[i].x, keyRPMSet[i].y);

        }

    }

    private void ApplyingDownForce()
    {

        bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);

    }

    private void Drafting()
    {

        draftingRay = new Ray(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(direction * m_RayRange));
        Debug.DrawRay(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(direction * m_RayRange));

        if (Physics.Raycast(draftingRay, out RaycastHit hit, m_RayRange))
        {
            if (hit.collider.CompareTag("AI") || hit.collider.CompareTag("Player"))
            {
                Debug.Log("Im behind");
                bodyOfCar.AddForce(bodyOfCar.transform.forward * (1000f * draftingMultiplierValue));

            }

        }
    }

    private float driftFactor;
    private float draftingMultiplierValue;

    private void AdjustTractionForDrifting()
    {
        // time it takes to go from drive to drift

        float driftSmoothFactor = 0.7f * Time.deltaTime;
        if (ifHandBraking || handbraking > 0)
        {
            bodyOfCar.angularDrag = whenDrifting;
            sidewaysFriction = wheels4[0].sidewaysFriction;
            forwardFriction = wheels4[0].forwardFriction;

            float velocity = 0;

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
            Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakefrictionMulitplier, ref velocity, driftSmoothFactor);


            for (int i = 0; i < 4; i++)
            {

                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;
            }
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1f;

            // extra grip for front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;

            }

            bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * 10000);
        }
        // executed when handbrake is held
        else
        {
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = ((currentSpeed * handBrakefrictionMulitplier / 300) + 1);

            for (int i = 0; i < 4; i++)
            {
                wheels4[i].forwardFriction = forwardFriction;
                wheels4[i].sidewaysFriction = sidewaysFriction;
            }
            bodyOfCar.angularDrag = whenNotDrifting;
        }
    }

}