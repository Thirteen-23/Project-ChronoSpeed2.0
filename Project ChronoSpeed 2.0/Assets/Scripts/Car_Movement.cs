using UnityEngine;
using UnityEngine.InputSystem;
public enum Class
{
    Light,
    Medium,
    Heavy
}
public class Car_Movement : MonoBehaviour
{
    ///keeping track of how many laps in the race. 

    CarNewInputSystem input;
  
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
    public Class carClasses;
    [SerializeField] TransmissionTypes transmission;
    [SerializeField] DifferentialTypes drive;
    public Rigidbody bodyOfCar;
    [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];
    [SerializeField] Transform centerMass;

    [SerializeField] float downForceValue;
    //[SerializeField] float currentDownforceValue; 
    float currentBreakForce, handbraking;
    bool resetPosition = false;

    [Header("Speed and Power of the Car")]
    [SerializeField] float horsePower;
    public AnimationCurve enginePower;
    public float maxSpeed;
    private float totalPowerInCar;
    public float currentSpeed;
    /// dampening for smoother acceration input for keyboard 
    public float acceration_Value;
    [SerializeField] float AccerationDamping;

    [Header("GearBox System")]
    [SerializeField] public int idleRPM;
    [SerializeField] float maxRPM;
    [SerializeField] float minRPM;
    [SerializeField] int maxRPMForCar;
    public float engineRPM;
    [SerializeField] float finalDriveRatio;
    [SerializeField] float[] gearSpeedBox = new float[0];
    public int gearNum;
    private float m_RPMOfWheels;
    [SerializeField] float smoothTime;
    [SerializeField] Vector2[] keyRPMSet = new Vector2[0];
    [SerializeField] ParticleSystem exhaust_Shift;
    [SerializeField] float[] slip = new float[4];
    public float amountOfSlipToShift;

    [Header("Manual Shift")]
    private bool shiftUp = false;
    private bool shiftDown = false;
    private float shift_Value;
    private float currentShift_Value;

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] bool isBraking;
    public bool ifHandBraking;
    WheelFrictionCurve sidewaysFriction, forwardFriction;
    public float handBrakefrictionMulitplier = 2f;
    float handbrakefriction;
    float tempo;
    [SerializeField] float whenNotDrifting;
    [SerializeField] float whenDrifting;
    public float driftFactor;

    [Header("Abilities Value")]
    public bool turnOnAllTerrain = false;
    public float frictionPlusValueForAbility;

    [Header("Handling & Brakes")]
    [SerializeField] float allBrakeForce;
    [SerializeField] float frontBrakeForce;
    [SerializeField] float rearBrakeForce;
    private float steering_Value;
    /// make the steering smoother when useing a  keyboard 
    private float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    private float brakes_value;
    private float brakeDampening;

    public float turnSpeed;
    [SerializeField] AnimationCurve steeringCurve;

    ///drafting values
    Ray draftingRay;
    Vector3 direction = Vector3.forward;

    [SerializeField] float m_RayRange;
    [SerializeField] float draftingMultiplierValue;

    /// ground clearance value
    Vector3 downwardsDirection = Vector3.down;
    [SerializeField] float floorRange;

    public Transform spawnpointerBehind;
    public Transform spawnpointer;
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;
    // drifting boost value
    [SerializeField] float forceBoostForDriftingValue = 20000f;
    public float findme;
    public bool lightCar, mediumCar, heavyCar = false;
    private void Awake()
    {

        input = new CarNewInputSystem();

    }

    private void OnEnable()
    {
       // input.Enable();
        //input.Movement.Acceleration.performed += ApplyingThrottleInput;
        //input.Movement.Acceleration.canceled += ReleaseThrottleInput;
        //input.Movement.Steering.performed += ApplySteeringInput;
        //input.Movement.Steering.canceled += ReleaseSteeringInput;
        // input.Movement.braking.performed += BrakingInput;
        // input.Movement.braking.canceled += ReleaseBrakingInput;

    }
    private void OnDisable()
    {
       // input.Disable();

    }
    void Start()
    {
        // nodes = waypoints.trackNodes;
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        bodyOfCar.centerOfMass = centerMass.localPosition;
        exhaust_Shift = GetComponentInChildren<ParticleSystem>();
        leftTrail.emitting = false;
        rightTrail.emitting = false;
    }

    private void FixedUpdate()
    {

        GettingInput();
        HandlingMotor();
        HandlingSteering();
        AnimatedWheels();
        DampeningSystem();
        calculatingEnginePower();
        ApplyingDownForce();
        ResettingCar();
        Shifting();
        SetEngineRPMAndTorque();
        Drafting();
        AdjustTractionForDrifting();
        CheckingforSlip();
        //CheckingDistanceOfWaypoints();
        NitroBoostin();
    }

    private void GettingInput()
    {
        isBraking = Input.GetKey(KeyCode.B);
        ifHandBraking = Input.GetKey(KeyCode.Space);
        resetPosition = Input.GetKey(KeyCode.R);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitApplication();
        }

    }
    // For Handling, acceration and brake
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

    private void quitApplication()
    {
        Application.Quit();
    }

    private void ResettingCar()
    {
        if (resetPosition == true)
        {
            gameObject.transform.position = originalPos;
            gameObject.transform.rotation = rotations;

        }
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
                    wheels4[i].motorTorque = totalPowerInCar  * 4 / 4;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar * 4 /2;
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
            isBraking = true;
        }
        currentBreakForce = isBraking ? (allBrakeForce * brakeDampening) : 0f;
        //handbraking = ifHandBraking ? rearBrakeForce : 0f;
        ApplyBraking();
        ApplyHandBraking();

    }
    private void ApplyBraking()
    {
        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce;
        }




    }

    private void ApplyHandBraking()
    {
        //for (int i = 2; i < wheels4.Length; i++)
        //{
        //    wheels4[i].brakeTorque = handbraking;

        //}
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
        //engineRPM = Mathf.Lerp(engineRPM, idleRPM + (Mathf.Abs(m_RPMOfWheels) * finalDriveRatio * (gearSpeedBox[gearNum])), smoothTime * Time.deltaTime);
        horsePower = (totalPowerInCar * engineRPM) / 5252;
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

    public void ApplySteeringInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        else if(context.performed)
        {
                    steering_Value = context.ReadValue<Vector2>().x;
                }
        else if (context.canceled)
        {
            steering_Value = 0;
        }
    }

    public void ReleaseSteeringInput(InputAction.CallbackContext context)
    {

        steering_Value = 0;
        //print(steering_Value);
    }

    public void ApplyingThrottleInput(InputAction.CallbackContext context)
    {
        if (context.started)
            acceration_Value = context.ReadValue<float>();
        else if (context.canceled)
        {
            acceration_Value = 0;
        }
    }
    public void ReleaseThrottleInput(InputAction.CallbackContext context)
    {
        acceration_Value = 0;
        //print(acceration_Value + " not accerating");
    }

    public void BrakingInput(InputAction.CallbackContext context)
    {
        if (context.started)
            brakes_value = context.ReadValue<float>();

        else if (context.canceled)
        {
            brakes_value = 0;
        }
    }

    public void ReleaseBrakingInput(InputAction.CallbackContext context)
    {
        brakes_value = 0;
    }

    public void ShiftingUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (shift_Value <= gearSpeedBox.Length - 1 && shift_Value < gearSpeedBox.Length - 1)
            {    //Debug.Log("started");
                shiftUp = true;
                shift_Value++;
            }
            else if (shift_Value == gearSpeedBox.Length - 1)
            {
                return;
            }
        }
        else if (context.performed)
        {
            // Debug.Log("performed");
            shiftUp = false;
        }
        else if (context.canceled)
        {
            // Debug.Log("cancelled");
        }
    }

    public void ShiftingDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (shift_Value <= gearSpeedBox.Length - 1 && shift_Value > 0)
            {
                shiftDown = true;
                shift_Value--;
            }
            else if (shift_Value == 0)
            {
                shift_Value = 0;
            }
        }
        else if (context.performed)
        {
            // Debug.Log("performed");
            shiftDown = false;
        }
        else if (context.canceled)
        {
            // Debug.Log("cancelled");
        }
    }

    public void Handbraking(InputAction.CallbackContext context)
    {

        handbraking = context.ReadValue<float>();
        if (context.started)
        {
            if (handbraking == 1)
            {
                ifHandBraking = true;
            }
        }
        if (context.performed)
        {
            ifHandBraking = true;
        }
        else if (context.canceled)
        {
            ifHandBraking = false;
            handbraking = 0;
            Debug.Log("Let go");
        }

    }
    private void Shifting()
    {
        if (transmission == TransmissionTypes.Manual)
        {
            Mathf.Clamp(shift_Value, 0, gearSpeedBox.Length - 1);
            if ((shiftUp == true && shift_Value > currentShift_Value) && (gearNum < gearSpeedBox.Length - 1))
            {
                //Debug.Log(gearNum);
                //Debug.Log(gearSpeedBox[gearNum]);

                gearNum++;
                currentShift_Value = shift_Value;
                exhaust_Shift.Play();
                if (shift_Value == gearSpeedBox.Length - 1)
                {
                    shift_Value = gearSpeedBox.Length - 1;
                }
            }
            else if ((shiftDown == true && shift_Value < currentShift_Value) && (gearNum > 0))
            {

                gearNum--;

                currentShift_Value = shift_Value;

            }
        }
        if (transmission == TransmissionTypes.Automatic)
        {
            WheelHit wheelHit;
            switch (drive)
            {
                case DifferentialTypes.AllWheelDrive:
                    for (int i = 0; i < wheels4.Length; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;
                        if (engineRPM >= maxRPM)
                        {
                            if (slip[i] > amountOfSlipToShift)
                            {
                                return;
                            }
                            else if (gearNum < gearSpeedBox.Length - 1 && slip[i] < amountOfSlipToShift)
                            {
                                gearNum++;
                                exhaust_Shift.Play();
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
                    break;
                case DifferentialTypes.RearWheelDrive:
                    for (int i = 2; i < wheels4.Length; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;

                        if (engineRPM >= maxRPM)
                        {
                            if (slip[i] > amountOfSlipToShift)
                            {
                                return;
                            }
                            else if (gearNum < gearSpeedBox.Length - 1 && slip[i] < amountOfSlipToShift)
                            {
                                gearNum++;
                                exhaust_Shift.Play();
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
                    break;
                case DifferentialTypes.FrontWheelDrive:
                    for (int i = 0; i < wheels4.Length - 2; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;

                        if (engineRPM >= maxRPM && slip[i] < amountOfSlipToShift)
                        {
                            if (gearNum < gearSpeedBox.Length - 1)
                            {
                                gearNum++;
                               // exhaust_Shift.Play();
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
                    break;
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

    Ray ray;
    private void ApplyingDownForce()
    {
        // float temp = 0;

        //WheelHit hit;
        // for (int i = 0; i < wheels4.Length; i++)
        // {
        //     if (wheels4[i].GetGroundHit(out hit))
        //     {
        //         currentDownforceValue = downForceValue; 
        //         Debug.Log("touching Ground");
        //            bodyOfCar.AddForce(-transform.up * currentDownforceValue * bodyOfCar.velocity.magnitude);

        //     }
        //     else
        //     {
        //         currentDownforceValue = temp;
        //         bodyOfCar.AddForce(-transform.up * currentDownforceValue * bodyOfCar.velocity.magnitude);
        //     }
        ray = new Ray(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(downwardsDirection * floorRange));
        Debug.DrawRay(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(downwardsDirection * floorRange));

        if (Physics.Raycast(ray, out RaycastHit hit, floorRange))
        {
            if (hit.collider.CompareTag("ground"))
            {
              //  Debug.Log("on the ground");
                bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);

            }

        }
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
    [SerializeField] float tt = 1;
    [SerializeField] float maxAmountOfGrip;
    [SerializeField] float minAmountOfGripAtStart;
    float driftEndingGrip;
    public ParticleSystem leftWheel;
    public ParticleSystem rightWheel;
    public ParticleSystem leftWheelSmoke;
    public ParticleSystem rightWheelSmoke;
    private void AdjustTractionForDrifting()
    {

        #region Traction ability (now discarded)
        /*
        // for each terrain it is on
        WheelHit checkingTerrain;

        if (wheels4[0].GetGroundHit(out checkingTerrain))
        {
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;
            if (checkingTerrain.collider.name == "DystopiaGround" || checkingTerrain.collider.name == "UtopiaGround")
            {
                switch (carClasses)
                {
                    case Class.Light:
                        forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                        sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                        for (int i = 0; i < 4; i++)
                        {
                            wheels4[i].forwardFriction = forwardFriction;
                            wheels4[i].sidewaysFriction = sidewaysFriction;
                        }
                        if (checkingTerrain.collider.name == "UtopiaGround")
                        {
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + 0.2f;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + 0.2f;
                            for (int i = 0; i < 4; i++)
                            {
                                wheels4[i].forwardFriction = forwardFriction;
                                wheels4[i].sidewaysFriction = sidewaysFriction;
                            }
                        }
                        else if (checkingTerrain.collider.name == "DystopiaGround")
                        {
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction - 0.1f;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction - 0.1f;
                            for (int i = 0; i < 4; i++)
                            {
                                wheels4[i].forwardFriction = forwardFriction;
                                wheels4[i].sidewaysFriction = sidewaysFriction;
                            }
                        }
                        break;
                    case Class.Medium:

                        if (turnOnAllTerrain == true)
                        {
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + frictionPlusValueForAbility;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + frictionPlusValueForAbility;
                            for (int i = 0; i < 4; i++)
                            {
                                wheels4[i].forwardFriction = forwardFriction;
                                wheels4[i].sidewaysFriction = sidewaysFriction;
                            }
                        }
                        else
                        {

                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                            for (int i = 0; i < 4; i++)
                            {
                                wheels4[i].forwardFriction = forwardFriction;
                                wheels4[i].sidewaysFriction = sidewaysFriction;
                            }
                        }
                        break;
                    case Class.Heavy:
                        forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                        sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                        for (int i = 0; i < 4; i++)
                        {
                            wheels4[i].forwardFriction = forwardFriction;
                            wheels4[i].sidewaysFriction = sidewaysFriction;
                        }
                        if (checkingTerrain.collider.name == "DystopiaGround")
                        {
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + 0.2f;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + 0.2f;
                            for (int i = 0; i < 4; i++)
                            {
                                wheels4[i].forwardFriction = forwardFriction;
                                wheels4[i].sidewaysFriction = sidewaysFriction;
                            }
                        }
                        else if (checkingTerrain.collider.name == "UtopiaGround")
                        {
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction - 0.1f;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction - 0.1f;
                            for (int i = 0; i < 4; i++)
                            {
                                wheels4[i].forwardFriction = forwardFriction;
                                wheels4[i].sidewaysFriction = sidewaysFriction;
                            }
                        }
                        break;

                }
            }
            //if (checkingTerrain.collider.name == "DystopiaGround" && carClasses == Class.Medium)
            //{
            //    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
            //    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;
            //    for (int i = 0; i < 4; i++)
            //    {
            //        wheels4[i].forwardFriction = forwardFriction;
            //        wheels4[i].sidewaysFriction = sidewaysFriction;
            //    }
            //}
            else
            {
                forwardFriction = wheels4[0].forwardFriction;
                sidewaysFriction = wheels4[0].sidewaysFriction;
                forwardFriction.stiffness = 1;
                sidewaysFriction.stiffness = 1;
                for (int i = 0; i < 4; i++)
                {
                    wheels4[i].forwardFriction = forwardFriction;
                    wheels4[i].sidewaysFriction = sidewaysFriction;
                }
            }
            // forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
            // sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;


        }*/
        #endregion

        /// time it takes to go from drive to drift
        float driftSmoothFactor = 0.7f * Time.deltaTime;
        if (ifHandBraking && currentSpeed > 40 || currentSpeed > 40 && handbraking > 0)
        {
            bodyOfCar.angularDrag = whenDrifting;
            sidewaysFriction = wheels4[0].sidewaysFriction;
            forwardFriction = wheels4[0].forwardFriction;
          
            float velocity = 0;

            driftEndingGrip = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
            Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakefrictionMulitplier, ref velocity, driftSmoothFactor);


            for (int i = 0; i < 4; i++)
            {

                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;
            }
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.2f;

            // extra grip for front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;

            }
          
            // bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * 25000);
            // bodyOfCar.AddRelativeForce(bodyOfCar.transform.forward * steeringCurve.Evaluate(180f));

            tt = 0;
        }
        // executed when handbrake is not held
        else
        {
            #region option for drifting
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;


            if (tt > 1f)
            {

                Debug.Log("normal friction");
                for (int i = 0; i < 4; i++)
                {
                    wheels4[i].forwardFriction = forwardFriction;
                    wheels4[i].sidewaysFriction = sidewaysFriction;
                }
                forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 2f, 0, 3);
            }
            else
            {
                tt += Time.deltaTime * 2f;

                forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                    Mathf.Lerp(driftEndingGrip, Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, minAmountOfGripAtStart, maxAmountOfGrip), tt);
                //Mathf.Lerp((forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue),// (currentSpeed * handBrakefrictionMulitplier / 300) + 2.5f, Time.deltaTime * 2f);
                //Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 2f, 0, 3), tt);



                for (int i = 0; i < 4; i++)
                {
                    wheels4[i].forwardFriction = forwardFriction;
                    wheels4[i].sidewaysFriction = sidewaysFriction;
                }

                WheelHit wheelHit;

                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].GetGroundHit(out wheelHit);
                    slip[i] = wheelHit.sidewaysSlip /*/ wheels4[i].sidewaysFriction.extremumSlip*/;
                    if (slip[i] > 0.4f || slip[i] < -0.4f)
                    {
                        tt = 1f;
                        forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                   Mathf.Lerp(driftEndingGrip, Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 2f, 0, 5), tt);
                        bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * forceBoostForDriftingValue);
                        //leftTrail.emitting = true;
                        //rightTrail.emitting = true;
                        rightWheel.Play();
                        leftWheel.Play();
                        leftWheelSmoke.Play();
                        rightWheelSmoke.Play();
                        switch (carClasses)
                        {
                            case Class.Light:
                                lightCar = true;
                                break;
                            case Class.Medium:
                                mediumCar = true;
                                break;
                            case Class.Heavy:
                                heavyCar = true;
                                break;
                        }
                        for (int j = 0; j < 4; j++)
                        {
                            wheels4[j].forwardFriction = forwardFriction;
                            wheels4[j].sidewaysFriction = sidewaysFriction;
                        }

                    }
                    else
                    {
                       // leftTrail.emitting = false;
                       // rightTrail.emitting = false;
                        rightWheel.Stop();
                        leftWheel.Stop();
                        leftWheelSmoke.Stop();
                        rightWheelSmoke.Stop();
                        lightCar = false;
                        mediumCar = false;
                        heavyCar = false; 
                    }
                }

                if (forwardFriction.extremumValue >= Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, 0, 3))
                {
                    bodyOfCar.angularDrag = whenNotDrifting;
                    tt = 1.0f;
                    return;
                }
            }
            bodyOfCar.angularDrag = whenNotDrifting;
           
            #endregion


        }
    }
    public bool meBoosting = false;
    public float boostValue = 3000f;
    public ParticleSystem nitroboostColor;
    public void NitroBoostin()
    { 
        if(meBoosting == true)
        {
            Debug.Log("I am boosting?");
            bodyOfCar.AddForce(bodyOfCar.transform.forward * boostValue);
            nitroboostColor.Play();

        }
        else
        {
            nitroboostColor.Stop();
        }
    }
    private void CheckingforSlip()
    {
        WheelHit wheelHit;

        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].GetGroundHit(out wheelHit);
            slip[i] = wheelHit.sidewaysSlip /*/ wheels4[i].sidewaysFriction.extremumSlip*/;
        }
    }

}


