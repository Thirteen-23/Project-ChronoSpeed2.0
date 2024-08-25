
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class Speedo : MonoBehaviour
{


    [SerializeField] Rigidbody rb;
    [SerializeField] Car_Movement car;
    [SerializeField] Tracking_Manager_Script checkingPosition; 

    [Header("RPM UI")]
   // [SerializeField] Image RPMNeedle;
    [SerializeField] float minRPMAngle;
    [SerializeField] float maxRPMAngle;
    public Slider m_RPM;
    // public TextMeshProUGUI RpmNum;
    private float m_rpmIndicator;
    private float final_RPMIndicator;


    [Header("Speedo UI")]
    public TextMeshProUGUI speedLabel;
    public Slider m_Speedo;
    //  public TextMeshProUGUI speedoFinalSpeed;
    //  public TextMeshProUGUI speedoMiddleSpeed;
    // [SerializeField] Image speedoNeedle;
   // [SerializeField] float minSpeedoAngle;
  //  [SerializeField] float maxSpeedoAngle;
    public float maxSpeed = 0.0f;
    private float speed;
    private float finalSpeed;
    const float speedoSnap = 2f;

    [Header("Gear UI")]
    public TextMeshProUGUI currentGearLabel;
    private float currentGear = 0f;
    [SerializeField] int gear;

    
    public AbilityManager ability;
    //public TextMeshProUGUI positionBoard;
    [SerializeField] GameObject[] list;
    
    // Start is called before the first frame update
    void Start()
    {
       // rb = rb.GetComponent<Rigidbody>();
       // car = car.GetComponent<Car_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
        speedo();
        GearChangeFunction();
        //RPMBar();
       // ExactNumOfRPM();
        ScoreBoard();
        

    }
    private void ScoreBoard()
    {
        for (int i = 0; i < checkingPosition.m_listOfCars.Length; i++)
        {
            list[i] = checkingPosition.m_listOfCars[i];
        }

        for (int i = 0; i < list.Length; i++)
        {
            //positionBoard.text = list[i] + "";
        }
    }
    private void speedo()
    {
        // 3.6f conversion to KM/H
      //  speedoFinalSpeed.text = maxSpeed + "";
       // speedoMiddleSpeed.text = maxSpeed / 2 + ""; 
        speed = rb.velocity.magnitude * 3.6f;
        finalSpeed = Mathf.Lerp(finalSpeed, speed, speedoSnap * Time.deltaTime);
        if (speedLabel != null)
        {
            speedLabel.text = ((int)finalSpeed) + "km/h";
            
            ///--- code for the needle if used
            //speedoNeedle.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedoAngle, maxSpeedoAngle, speed / maxSpeed)); 
            //speedoNeedle.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedAngle, maxSpeedAngle, speed / maxSpeed));

        }
        m_Speedo.maxValue = maxSpeed;
        m_Speedo.minValue = 0;
        m_Speedo.value = finalSpeed;
    }

    private void GearChangeFunction()
    {
        currentGear = car.gearNum + 1;
        if (currentGearLabel != null)
        {
            currentGearLabel.text = ((int)currentGear + "");
        }


    }

    private void RPMBar()
    {
        float timeTime;
        m_rpmIndicator = car.engineRPM;
        Keyframe lastkey = car.enginePower[car.enginePower.length - 1];
        timeTime = lastkey.time;
        m_RPM.maxValue = timeTime;
        m_RPM.minValue = car.idleRPM;
        final_RPMIndicator = Mathf.Lerp(final_RPMIndicator, m_rpmIndicator, speedoSnap * Time.deltaTime);
        m_RPM.value = final_RPMIndicator;
        ///---rpm needle if used
       // RPMNeedle.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minRPMAngle, maxRPMAngle, final_RPMIndicator / timeTime));
    }

    private void ExactNumOfRPM()
    {
        m_RPM.value = m_rpmIndicator;
        final_RPMIndicator = Mathf.Lerp(final_RPMIndicator, m_RPM.value, speedoSnap * Time.deltaTime);



    }
   
   
    
}
