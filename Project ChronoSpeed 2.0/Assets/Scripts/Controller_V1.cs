using CarVariables;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
namespace CarVariables
{
    [System.Serializable]
    public class CarInfo
    {

       
      
    }
}
public class Controller_V1 : MonoBehaviour
{

    
    //keeping track of how many laps in the race. 
    public int numberOfLaps;
    CarNewInputSystem input;
    enum Class
    {
        Light,
        Medium,
        Heavy
    }
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
    public GameObject[] wheelmeshes = new GameObject[4];

    public WheelCollider[] wheels4 = new WheelCollider[4];
    public Rigidbody bodyOfCar;
    [SerializeField] CarInfo carInfo;
    [SerializeField] Class carClasses;
    [SerializeField] TransmissionTypes transmission;
    [SerializeField] DifferentialTypes drive;
   
    public Transform centerMass;

    [SerializeField] float downForceValue;
    float currentBreakForce, handbraking;
    bool resetPosition = false;

    [Header("Speed and Power of the Car")]
    [SerializeField] float horsePower;
    public AnimationCurve enginePower;
    public float maxSpeed;
    private float totalPowerInCar;
    [SerializeField] float currentSpeed;
    // dampening for smoother acceration input for keyboard 
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
    // make the steering smoother when useing a  keyboard 
    private float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    private float brakes_value;
    private float brakeDampening;

    public float turnSpeed;
    [SerializeField] AnimationCurve steeringCurve;

    //drafting values
    Ray draftingRay;
    Vector3 direction = Vector3.forward;
    [SerializeField] float m_RayRange;
    [SerializeField] float draftingMultiplierValue;

    public Transform spawnpointerBehind;
    public Transform spawnpointer;
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;
    private float currentVelocity;
    private float driftVelocity; 
    // Start is called before the first frame update 
    private void Awake()
    {

        input = new CarNewInputSystem();

    }

    private void OnEnable()
    {
        input.Enable();
        input.Movement.Acceleration.performed += ApplyingThrottleInput;
        input.Movement.Acceleration.canceled += ReleaseThrottleInput;
        input.Movement.Steering.performed += ApplySteeringInput;
        input.Movement.Steering.canceled += ReleaseSteeringInput;
        // input.Movement.braking.performed += BrakingInput;
        // input.Movement.braking.canceled += ReleaseBrakingInput;

    }
    private void OnDisable()
    {
        input.Disable();

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

    // Update is called once per frame
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
                    wheels4[i].motorTorque = (totalPowerInCar * 5) / 4;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = (totalPowerInCar * 5) / 2;
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

        steering_Value = context.ReadValue<float>();
        //print(steering_Value);
    }

    public void ReleaseSteeringInput(InputAction.CallbackContext context)
    {

        steering_Value = 0;
        //print(steering_Value);
    }

    public void ApplyingThrottleInput(InputAction.CallbackContext context)
    {
        acceration_Value = context.ReadValue<float>();

        //print(acceration_Value + "accerating");
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
            {  
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
            shiftUp = false;
        }
        else if (context.canceled)
        {
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
            shiftDown = false;
        }
        else if (context.canceled)
        {
        }
    }
    public void Handbraking(InputAction.CallbackContext context)
    {
            handbraking = context.ReadValue<float>();
        if(context.started)
        {
            if(handbraking == 1)
            {
                ifHandBraking = true;
            }
        }
           if(context.performed)
        {
            ifHandBraking = true;
        }
           else if(context.canceled)
        {
            ifHandBraking = false;
        }
    }

    private void Shifting()
    {
        if (transmission == TransmissionTypes.Manual)
        {
            Mathf.Clamp(shift_Value, 0, gearSpeedBox.Length - 1);
            if ((shiftUp == true && shift_Value > currentShift_Value) && (gearNum < gearSpeedBox.Length - 1))
            {

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

                        if (slip[i] > amountOfSlipToShift)
                        {
                            return;
                        }
                        else if (gearNum < gearSpeedBox.Length - 1 && slip[i] < amountOfSlipToShift)
                        {
                            gearNum++;
                            //exhaust_Shift.Play();
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
        WheelHit hit;
        if (wheels4[0].GetGroundHit(out hit))
        {
            if (hit.collider == false)
            {
                for (int i = 0; i < wheels4.Length; i++)
                {
                    downForceValue = 0;
                    bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);
                }
            }
        }
        else
        {
            bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);
        }
    }

    private void Drafting()
    {
        draftingRay = new Ray(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(direction * m_RayRange));

        if (Physics.Raycast(draftingRay, out RaycastHit hit, m_RayRange))
        {
            if (hit.collider.CompareTag("AI") || hit.collider.CompareTag("Player"))
            {
                bodyOfCar.AddForce(bodyOfCar.transform.forward * (1000f * draftingMultiplierValue));

            }

        }
    }


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
        if (ifHandBraking)
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
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.2f;

            // extra grip for front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;

            }

            bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * 25000);
        }
        // executed when handbrake is held
        else
        {
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
            Mathf.Lerp((forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue), (currentSpeed * handBrakefrictionMulitplier / 300) + 1, Time.deltaTime * 2f);

            for (int i = 0; i < 4; i++)
            {
                wheels4[i].forwardFriction = forwardFriction;
                wheels4[i].sidewaysFriction = sidewaysFriction;
            }
            bodyOfCar.angularDrag = whenNotDrifting;
        }
    }

    private void CheckingforSlip()
    {
        WheelHit wheelHit;

        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].GetGroundHit(out wheelHit);
            slip[i] = wheelHit.forwardSlip;
        }


    }

    void HandleDriftV2() 
    {
        if (ifHandBraking || handbraking > 0)
        {
            bodyOfCar.constraints = RigidbodyConstraints.FreezeRotationX;

            float newZ = Mathf.SmoothDamp(bodyOfCar.velocity.z, 0, ref currentVelocity, 1f);

            bodyOfCar.velocity = bodyOfCar.transform.forward * newZ;
           
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].brakeTorque = rearBrakeForce;
                    ApplyFriction(wheels4[i]);
                }


        }
        else
        {
            bodyOfCar.constraints = RigidbodyConstraints.None;
            for (int i = 2; i < wheels4.Length; i++)
            {
                wheels4[i].brakeTorque = 0;
                ResetFriction(wheels4[i]);
            }

        }
    }

    void ApplyFriction(WheelCollider wheels)
    {
        if(wheels.GetGroundHit(out var hit))
        {
            wheels.forwardFriction = UpdateFriction(wheels.forwardFriction);
            wheels.sidewaysFriction = UpdateFriction(wheels.sidewaysFriction);
        }
    }

    void ResetFriction(WheelCollider wheels)
    {
        for (int i = 2; i > wheels4.Length; i++)
        {
            wheels.forwardFriction = wheels4[0].forwardFriction;
            wheels.sidewaysFriction = wheels4[0].sidewaysFriction;
        }
    }

    WheelFrictionCurve UpdateFriction(WheelFrictionCurve friction)
    {
        friction.stiffness = ifHandBraking ? Mathf.SmoothDamp(friction.stiffness, 0.5f, ref driftVelocity, Time.deltaTime * 2f) : 1f;

        return friction; 
    }
}


