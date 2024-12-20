
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerStateMachine;
using UnityEngine.InputSystem.LowLevel;
using Cinemachine;
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
    [SerializeField] float AccelerationDamping;

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
    public float steering_Value;
    /// make the steering smoother when useing a  keyboard 
    public float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    [SerializeField] float smoothTransitionSpeedForAcceleration;
    [SerializeField] float brakes_value;
    [SerializeField] float brakeDampening;
    [SerializeField] bool isReversing = false; 

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

    // drifting boost value
    [Header(" boost values when drifting ")]
    [SerializeField] float boostWhenExitingDrift = 20000f;
    public float findme;
    public bool lightCar, mediumCar, heavyCar = false;
    public float minDrag = 0;
    public float maxDrag = 4;
    public float boostWhileDrifting = 25000f;
    [SerializeField] float tt = 1;
    [SerializeField] float maxAmountOfGrip;
    [SerializeField] float minAmountOfGripAtStart;
    float driftEndingGrip;
    public bool meBoosting = false;
    public float boostValue = 3000f;

    [Header("VFX")]
    // GameObjects / particle systems
    public ParticleSystem leftWheel;
    public ParticleSystem rightWheel;
    public ParticleSystem leftWheelSmoke;
    public ParticleSystem rightWheelSmoke;
    public ParticleSystem[] nitroboostColor;
    public ParticleSystem[] exhaustVFX;




    private void Awake()
    {
        input = new CarNewInputSystem();
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    void Start()
    {
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        bodyOfCar.centerOfMass = centerMass.localPosition;
        exhaust_Shift = GetComponentInChildren<ParticleSystem>();
        m_StateMach = GetComponentInChildren<PlayerStateMachine>();
    }

    private void FixedUpdate()
    {

        GettingInput();
        HandlingMotor();
        CarBraking();
        HandlingSteering();
        AnimatedWheels();
        DampeningSystem();
        calculatingEnginePower();
        ApplyingDownForce();
        ResettingCar();
        Shifting();
        SetEngineRPMAndTorque();
        Drafting();
        AdjustTractionForRoad();
        AdjustTractionForDrifting();
      //  CheckingForSteerAngle();
        //CheckingDistanceOfWaypoints();
        NitroBoostin();
        ExtraBoostOnLowSpeed(currentSpeed, acceration_Value);
    }

    private void GettingInput()
    {
        isBraking = Input.GetKey(KeyCode.B);
        ifHandBraking = Input.GetKey(KeyCode.Space);
        resetPosition = Input.GetKey(KeyCode.R);
        if (resetPosition == true)
        {
            bodyOfCar.velocity = Vector3.zero;
        }
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    quitApplication();
        //}

    }
    // For Handling, acceration and brake
    private float SmoothTransition(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * smoothTransitionSpeed);
    }
    private float SmoothTransitionForAcceleration(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * smoothTransitionSpeedForAcceleration);
    }

    private void DampeningSystem()
    {
        AccelerationDamping = SmoothTransitionForAcceleration(acceration_Value, AccelerationDamping);

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
                    wheels4[i].motorTorque = totalPowerInCar * 4 / 4;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar * 4 / 2;
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
       
        

       
    }

    private void CarBraking()
    {
       if (brakes_value > 0.7f)
        {
            isBraking = true;
        }
      
        currentBreakForce = isBraking ? (allBrakeForce * brakeDampening) : Mathf.Lerp(currentSpeed ,0f, 1);
        //handbraking = ifHandBraking ? rearBrakeForce : 0f;

        ApplyBraking();
       
        
    }
    private void ApplyBraking()
    {
        for (int i = 0; i < wheels4.Length-2; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce * brakeDampening;
        }
        for (int i = 2; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce * brakeDampening /2;
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
        totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * AccelerationDamping;
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
        else if (context.performed)
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
        {

        }
        else if (context.performed)
        {
          
                acceration_Value = context.ReadValue<float>();
           
        }
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
        {

        }
        if (context.performed)
        {
            brakes_value = context.ReadValue<float>();
            isBraking = true;
            if(currentSpeed <0.1f)
            {
                isBraking = false;
                    }
        }
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
        }

    }

    public bool ture = false;

    public float lookBackValue;
    public void LookBehind(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            lookBackValue = context.ReadValue<float>();

        }
        if (context.performed)
        {
            lookBackValue = context.ReadValue<float>();
            if (lookBackValue > 0.1)
            {
                ture = true;
            }
        }
        if (context.canceled)
        {
            lookBackValue = 0f;
            ture = false;
        }
    }

   
    [Header("Shifting Time Values")]
    public float maxTimeToShiftGear;
    public float timerToShift;
    public float rpmLimiter = 5000; 
    private void Shifting()
    {
        float temp = acceration_Value;

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
                    for (int i = 2; i < wheels4.Length; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;
                        if (engineRPM >= maxRPM)
                        {
                            if (slip[i] > amountOfSlipToShift)
                            {
                                /*if(engineRPM > rpmLimiter)
                                {
                                    engineRPM = Mathf.Clamp(engineRPM, 0, rpmLimiter); 
                                }*/
                                return;
                            }
                            else if (gearNum < gearSpeedBox.Length - 1 && slip[i] < amountOfSlipToShift)
                            {
                                // changes to shifting 
                                if (timerToShift > 0)
                                {
                                    timerToShift -= Time.deltaTime;
                                 
                                }
                                else
                                {
                                   
                                    gearNum++;
                                    exhaust_Shift.Play();
                                    timerToShift = maxTimeToShiftGear;
                                }
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
                                if (timerToShift > 0)
                                {
                                    timerToShift -= Time.deltaTime;
                                    engineRPM = Mathf.Clamp(engineRPM, 0, maxRPM - 500);
                                }
                                else
                                {
                                    gearNum++;
                                    exhaust_Shift.Play();
                                    timerToShift = maxTimeToShiftGear;
                                }
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
        //            bodyOfCar.AddForce(-transform.up * currentDownforceValue * bodyOfCar.velocity.magnitude);

        //     }
        //     else
        //     {
        //         currentDownforceValue = temp;
        //         bodyOfCar.AddForce(-transform.up * currentDownforceValue * bodyOfCar.velocity.magnitude);
        //     }
        ray = new Ray(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(downwardsDirection * floorRange));

        if (Physics.Raycast(ray, out RaycastHit hit, floorRange))
        {
            if (hit.collider.CompareTag("ground"))
            {
                bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);

            }

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

    // values for friction
    [Header("Friction Values for Cars ")]
    public float m_TForwardFrictionValue;
    public float m_TSidewaysFrictionValue;
    [SerializeField] float m_SForwardFrictionValue;
    [SerializeField] float m_SSidewaysFrictionValue;
    [SerializeField] float m_ExtraGripOnHigherSpeed;
    [SerializeField] float m_ValueToChangeSpeedForGrip = 180f; 
    private void AdjustTractionForRoad()
    {
        #region Traction 
        // for each terrain it is on
        WheelHit checkingTerrain;
        foreach (WheelCollider wheel in wheels4)
        {



            if (wheel.GetGroundHit(out checkingTerrain))
            {
                forwardFriction = wheel.forwardFriction;
                sidewaysFriction = wheel.sidewaysFriction;
                //if (checkingTerrain.collider.CompareTag("Road") || checkingTerrain.collider.CompareTag("SideWalk"))
                if (checkingTerrain.collider.CompareTag("Tarmac") || checkingTerrain.collider.CompareTag("SideWalk"))
                {
                    switch (carClasses)
                    {
                        case Class.Light:
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;

                            wheel.forwardFriction = forwardFriction;
                            wheel.sidewaysFriction = sidewaysFriction;
                            if(currentSpeed < 80)
                            {
                                if (checkingTerrain.collider.CompareTag("Tarmac"))
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;

                                }
                                else if (checkingTerrain.collider.CompareTag("SideWalk"))
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;

                                }
                            }
                            else
                            {
                                if (checkingTerrain.collider.CompareTag("Tarmac"))
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TForwardFrictionValue;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TSidewaysFrictionValue;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;

                                }
                                else if (checkingTerrain.collider.CompareTag("SideWalk"))
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_SForwardFrictionValue;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_SSidewaysFrictionValue;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;

                                }
                            }
                           
                            break;
                        case Class.Medium:

                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;

                            wheel.forwardFriction = forwardFriction;
                            wheel.sidewaysFriction = sidewaysFriction;

                            if (checkingTerrain.collider.CompareTag("Tarmac"))
                            {
                               
                                forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TForwardFrictionValue;
                                sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TSidewaysFrictionValue;

                                
                                wheel.forwardFriction = forwardFriction;
                                wheel.sidewaysFriction = sidewaysFriction;

                            }
                            else if (checkingTerrain.collider.CompareTag("SideWalk"))
                            {
                                forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_SForwardFrictionValue;
                                sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_SSidewaysFrictionValue;

                                wheel.forwardFriction = forwardFriction;
                                wheel.sidewaysFriction = sidewaysFriction;

                            }
                            break;
                        case Class.Heavy:
                            forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                            sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;

                            wheel.forwardFriction = forwardFriction;
                            wheel.sidewaysFriction = sidewaysFriction;

                            if (checkingTerrain.collider.CompareTag("Tarmac"))
                            {
                                if (currentSpeed < m_ValueToChangeSpeedForGrip)
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TForwardFrictionValue;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TSidewaysFrictionValue;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;
                                }

                                else if (currentSpeed > m_ValueToChangeSpeedForGrip)
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TForwardFrictionValue;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TSidewaysFrictionValue + m_ExtraGripOnHigherSpeed;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;
                                }
                            }
                            else if (checkingTerrain.collider.CompareTag("SideWalk"))
                            {
                                if (currentSpeed < m_ValueToChangeSpeedForGrip)
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_SForwardFrictionValue;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_SSidewaysFrictionValue;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;
                                }
                                else if (currentSpeed > m_ValueToChangeSpeedForGrip)
                                {
                                    forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TForwardFrictionValue;
                                    sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction + m_TSidewaysFrictionValue + m_ExtraGripOnHigherSpeed;

                                    wheel.forwardFriction = forwardFriction;
                                    wheel.sidewaysFriction = sidewaysFriction;
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
                    forwardFriction = wheel.forwardFriction;
                    sidewaysFriction = wheel.sidewaysFriction;
                    forwardFriction.stiffness = 1;
                    sidewaysFriction.stiffness = 1;

                    wheel.forwardFriction = forwardFriction;
                    wheel.sidewaysFriction = sidewaysFriction;
                }
                // forwardFriction.stiffness = checkingTerrain.collider.material.staticFriction;
                // sidewaysFriction.stiffness = checkingTerrain.collider.material.staticFriction;
            }


        }
        #endregion
    }
    [SerializeField] AudioSource m_DriftingSound;
    [SerializeField] PlayerStateMachine m_StateMach;  
    private void AdjustTractionForDrifting()
    {

      

        /// time it takes to go from drive to drift
        float driftSmoothFactor = 0.7f * Time.deltaTime;
        if (ifHandBraking && currentSpeed > 40 || currentSpeed > 40 && handbraking > 0)
        {
            bodyOfCar.angularDrag = whenDrifting;
            //bodyOfCar.angularDrag = Mathf.Lerp(minDrag, maxDrag, tt * 2f );
            //bodyOfCar.angularDrag = Mathf.Clamp(bodyOfCar.angularDrag,minDrag ,maxDrag);
            sidewaysFriction = wheels4[0].sidewaysFriction;
            forwardFriction = wheels4[0].forwardFriction;

            float velocity = 0;

            driftEndingGrip = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
            Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakefrictionMulitplier, ref velocity, driftSmoothFactor);
            m_DriftingSound.volume = 1f;

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
            // bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * boostInDrifting);

            if (wheels4[0].steerAngle > 20 || wheels4[0].steerAngle < -20)
            {
                bodyOfCar.AddForce(bodyOfCar.transform.forward * boostWhileDrifting);
                m_DriftingSound.volume = 1f;
            }
           
                // bodyOfCar.AddRelativeForce(bodyOfCar.transform.forward * steeringCurve.Evaluate(180f));
                WheelHit wheelHit;

            for (int i = 2; i < wheels4.Length; i++)
            {
                wheels4[i].GetGroundHit(out wheelHit);
                slip[i] = wheelHit.sidewaysSlip /*/ wheels4[i].sidewaysFriction.extremumSlip*/;
                if (slip[i] > 0.4f || slip[i] < -0.4f)
                {
                    m_StateMach.ChangeCurrentState(PlayerStates.Driving, false);
                    m_StateMach.ChangeCurrentState(PlayerStates.StartDrifting, true);
                    m_StateMach.ChangeCurrentState(PlayerStates.Drifting, true);

                    // leftWheelSmoke.Play();
                    // var leftMain = leftWheelSmoke.emission;
                    // leftMain.rateOverTime = ((int)currentSpeed * 10 <= 2000) ? (int)currentSpeed * 10 : 200;
                    // 
                    // rightWheelSmoke.Play();
                    // var rightMain = rightWheelSmoke.emission;
                    // rightMain.rateOverTime = ((int)currentSpeed * 10 <= 2000) ? (int)currentSpeed * 10 : 200;
                }

            }
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
                m_DriftingSound.volume = 0f;
                for (int i = 0; i < 4; i++)
                {
                    wheels4[i].forwardFriction = forwardFriction;
                    wheels4[i].sidewaysFriction = sidewaysFriction;
                }
                forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, minAmountOfGripAtStart, maxAmountOfGrip);
                
            }
            else
            {
                tt += Time.deltaTime;

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
                        m_StateMach.ChangeCurrentState(PlayerStates.StartDrifting, true);
                        m_StateMach.ChangeCurrentState(PlayerStates.Drifting, true);

                        tt = 1f;
                        forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                   Mathf.Lerp(driftEndingGrip, Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 2f, 0, 4), tt * 1f);
                       

                        bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * boostWhenExitingDrift);
                        //leftTrail.emitting = true;
                        //rightTrail.emitting = true;

                       //rightWheel.Play();
                        //leftWheel.Play();
                        //leftWheelSmoke.Play();
                        //rightWheelSmoke.Play();

                        m_DriftingSound.volume = 1f;

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
                        m_StateMach.ChangeCurrentState(PlayerStates.Driving, true);
                       // m_StateMach.ChangeCurrentState(PlayerStates.Drifting, false);
                       // rightWheel.Stop();
                       // leftWheel.Stop();
                        //leftWheelSmoke.Stop();
                        // rightWheelSmoke.Stop();
                        lightCar = false;
                        mediumCar = false;
                        heavyCar = false;
                        m_DriftingSound.volume = 0f;
                    }
                }

                if (forwardFriction.extremumValue >= Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, minAmountOfGripAtStart, maxAmountOfGrip))
                {
                   // float bodyDrag = bodyOfCar.angularDrag;
                   //bodyDrag = Mathf.Lerp(bodyDrag, whenNotDrifting,  tt);
                   // bodyOfCar.angularDrag = bodyDrag;
                    tt = 1.0f;
                    return;
                }
            }
            bodyOfCar.angularDrag = whenNotDrifting;
            m_StateMach.ChangeCurrentState(PlayerStates.Driving, true);
            #endregion


        }
    }

    public void NitroBoostin()
    {
        if (meBoosting == true)
        {
            bodyOfCar.AddForce(bodyOfCar.transform.forward * boostValue);
            for (int i = 0; i < nitroboostColor.Length; i++)
            {
                nitroboostColor[i].Play();
            }
            for (int i = 0; i < exhaustVFX.Length; i++)
            {
                exhaustVFX[i].Pause();
            }

        }
        else
        {
            for (int i = 0; i < nitroboostColor.Length; i++)
            {
                nitroboostColor[i].Stop();
            }
            for (int i = 0; i < exhaustVFX.Length; i++)
            {
                exhaustVFX[i].Play();
            }
        }
    }
    //public float[] steeringAngleOfWheels = new float[4];
    //private void CheckingForSteerAngle()
    //{ 
    //    for (int i = 0; i < wheels4.Length; i++)
    //    {

    //        steeringAngleOfWheels[i] = wheels4[i].steerAngle;
    //    }
    //}
    [SerializeField] float m_AmountOfForceOfTheStart = 10000;
    [SerializeField] bool imFlying = false;
    public void ExtraBoostOnLowSpeed(float currentSpeed, float accelValue)
    {
        //ray = new Ray(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(downwardsDirection * floorRange));
        WheelHit wheelHit;
        for (int i = 2; i < wheels4.Length; i++)
        {
            wheels4[i].GetGroundHit(out wheelHit);
            if (wheels4[i].isGrounded == true )
            {
                if (currentSpeed < 60)

                {
                    if (wheelHit.collider.CompareTag("Tarmac") || wheelHit.collider.CompareTag("SideWalk"))
                    {
                        imFlying = true;
                        bodyOfCar.AddForce(bodyOfCar.transform.forward * (m_AmountOfForceOfTheStart * accelValue));
                    }
                }
                else
                {
                    imFlying = false;
                }
            }
        }
       
    }


}



