using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] List<Ability> storeAbilities;
    public Ability m_1stAbility;
    public Ability m_2stAbility;

    enum abilityState
    {
        ready,
        active,
        cooldown
    }

    enum m_2ndAbilityState
    {
        ready,
        active,
        cooldown
    }

    enum ResourceState
    {
        charging,
        ready,
        active
    }
    [Header("abiliy values for cooldowns")]
    public float cooldownTime;
    public float activeTime;
    public float m_2ndCooldownTime;
    public float m_2ndActiveTime;
    public bool abilityUsed = false;
    public bool m_2ndAbilityUsed = false;
    abilityState state = abilityState.ready;
    m_2ndAbilityState m_2NdState = m_2ndAbilityState.ready;
    // Update is called once per frame
    // public Image m_1stAbilityImage;
    // public Image m_2ndAbilityImage;
    //  public Color readyColor;
    // public Color activeColor;
    // public Color cooldownColor;

    [Header("Resource Meter Values")]
    ResourceState resourceState = ResourceState.charging;
    public Class m_CarClass;
    public Car_Movement accessCarValues;
    // public Slider maBar;

    // public float currentResourceValue;
    // public int minResourceValue;
    // public int maxResourceValue;

    [Header(" Resource Meter For All Abilities")]
    // boost bar Values
    // public Slider boostBar;
    public float currentBoostBarValue;
    public int minBoostBarValue;
    public int maxBoostBarValue;
    public Image boostBarBar;
    public AudioSource nitroBoostSound;

    // Blink bar and Portal Bar Values
    // public Slider abilityBar;
    public float currentAbilityValue;
    public int minAbilityValue;
    public int maxAbilityValue;
    public Image abilityBarBar;

    // checking for heavy car drifting 
    public bool m_ResoureceGatherCheck = false;
    public float m_ResourceMultiplerForSpeed;

    //check for Utopia Car speed for resource increase
    public float m_SpeedThreshholdForResource;
    public float m_ResourceMultiplerForDrifting = 5;
    [Header("Ability Costs Values")]
    public float ability1CostValue;
    public float portalDropCostValue;
    public float blinkCostValue;

    private PortalSpawn tempPortSpawnRef;
    private Blink tempBlinkRef;
    public AudioSource blinkSound;
    public AudioSource chargingSound; 
    private void Awake()
    {
        accessCarValues = GetComponent<Car_Movement>();
        tempPortSpawnRef = GetComponent<PortalSpawn>();
        tempBlinkRef = GetComponent<Blink>();
        //nitroBoostSound = GameObject.Find("NitroBoostSound").GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {

        //  m_1stAbilityImage.color = readyColor;
        //  m_2ndAbilityImage.color = readyColor;

        // boostBar = GameObject.Find("BoostBar").GetComponentInChildren<Slider>();
        // abilityBar = GameObject.Find("AblityBar").GetComponentInChildren<Slider>();

    }

    void FixedUpdate()
    {
        AllMaBars();

        switch (resourceState)
        {
            case ResourceState.charging:
                if (ability1CostValue > currentBoostBarValue)
                {
                   // nitroBoostSound.Stop();
                }
                else
                {
                    resourceState = ResourceState.ready;
                }
                break;
            case ResourceState.ready:
                {

                    if (abilityUsed)
                    {
                        if (currentBoostBarValue >= ability1CostValue)
                        {
                            nitroBoostSound.Play();
                            currentBoostBarValue = currentBoostBarValue - ability1CostValue;
                            m_1stAbility.Activate(gameObject);
                            activeTime = m_1stAbility.activeTime;
                            resourceState = ResourceState.active;
                        }
                    }
                }
                break;
            case ResourceState.active:
                {
                    if (activeTime > 0)
                    {

                        activeTime -= Time.deltaTime;
                    }

                    else
                    {
                        m_1stAbility.BeginCooldown(gameObject);
                        if (ability1CostValue <= currentBoostBarValue)
                        {
                            resourceState = ResourceState.ready;
                        }
                        else
                        {
                            resourceState = ResourceState.charging;
                        }
                    }
                    break;

                }
        }
        #region current ResourceGather
        /*
        m_CarClass = accessCarValues.carClasses;
        maBar.minValue = minResourceValue;
        maBar.maxValue = maxResourceValue;
        maBar.value = currentResourceValue;

        switch (m_CarClass)
        {
            case Class.Light:

                if (accessCarValues.currentSpeed > m_SpeedThreshholdForResource)
                {
                    if (currentResourceValue < maxResourceValue)

                    {
                        currentResourceValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                    }

                }
                else if (currentResourceValue == maxResourceValue)
                {
                    currentResourceValue = maxResourceValue;
                }
                break;
            case Class.Medium:
                m_ResoureceGatherCheck = accessCarValues.mediumCar;
                if (accessCarValues.currentSpeed > m_SpeedThreshholdForResource)
                {
                    if (currentResourceValue < maxResourceValue)

                    {
                        currentResourceValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                    }

                }
                else if (m_ResoureceGatherCheck == true)
                {
                    if (currentResourceValue < maxResourceValue)

                    {
                        currentResourceValue += Time.deltaTime * m_ResourceMultiplerForDrifting;
                    }
                    else if (currentResourceValue == maxResourceValue)
                    {
                        currentResourceValue = maxResourceValue;
                    }
                }
                else if (currentResourceValue == maxResourceValue)
                {
                    currentResourceValue = maxResourceValue;
                }
                break;
            case Class.Heavy:
                m_ResoureceGatherCheck = accessCarValues.heavyCar;
                if (m_ResoureceGatherCheck == true)
                {
                    if (currentResourceValue < maxResourceValue)

                    {
                        currentResourceValue += Time.deltaTime * m_ResourceMultiplerForDrifting;
                    }
                    else if (currentResourceValue == maxResourceValue)
                    {
                        currentResourceValue = maxResourceValue;
                    }
                }
                break;
        }


        // abilityCoolDownAbility();
        //AIUsingAbilities();
        switch (resourceState)
        {
            case ResourceState.charging:
                if (ability1CostValue > currentResourceValue)
                {

                }
                else
                {
                    resourceState = ResourceState.ready;
                }
                break;
            case ResourceState.ready:
                {

                    if (abilityUsed == true && ability1CostValue < currentResourceValue)
                    {
                        currentResourceValue = currentResourceValue - ability1CostValue;
                        m_1stAbility.Activate(gameObject);
                        activeTime = m_1stAbility.activeTime;
                        resourceState = ResourceState.active;
                    }
                }
                break;
            case ResourceState.active:
                {
                    if (activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }

                    else
                    {
                        m_1stAbility.BeginCooldown(gameObject);
                        if (ability1CostValue <= currentResourceValue)
                        {
                            resourceState = ResourceState.ready;
                        }
                        else
                        {
                            resourceState = ResourceState.charging;
                        }
                    }
                    break;

                }
        }
        */
        #endregion
    }


    private void abilityCoolDownAbility()
    {
        /*   switch (state)
           {
               case abilityState.ready:
                   if (abilityUsed == true)
                   {
                       m_1stAbility.Activate(gameObject);
                       state = abilityState.active;
                       activeTime = m_1stAbility.activeTime;
                   }
                   break;
               case abilityState.active:
                   if (activeTime > 0)
                   {
                       activeTime -= Time.deltaTime;
                       m_1stAbilityImage.color = activeColor;
                   }

                   else
                   {
                       m_1stAbility.BeginCooldown(gameObject);

                       state = abilityState.cooldown;
                       cooldownTime = m_1stAbility.cooldownTime;
                   }

                   break;
               case abilityState.cooldown:
                   if (cooldownTime > 0)
                   {
                       cooldownTime -= Time.deltaTime;
                       m_1stAbilityImage.color = cooldownColor;
                   }
                   else
                   {
                       state = abilityState.ready;
                       m_1stAbilityImage.color = readyColor;
                   }
                   break;
           }

           switch (m_2NdState)
           {
               case m_2ndAbilityState.ready:
                   if (m_2ndAbilityUsed == true)
                   {
                       m_2stAbility.Activate(gameObject);
                       m_2NdState = m_2ndAbilityState.active;
                       m_2ndActiveTime = m_2stAbility.activeTime;
                   }
                   break;
               case m_2ndAbilityState.active:
                   if (m_2ndActiveTime > 0)
                   {
                       m_2ndActiveTime -= Time.deltaTime;
                       m_2ndAbilityImage.color = activeColor;
                   }

                   else
                   {
                       m_2stAbility.BeginCooldown(gameObject);

                       m_2NdState = m_2ndAbilityState.cooldown;
                       m_2ndCooldownTime = m_2stAbility.cooldownTime;
                   }

                   break;
               case m_2ndAbilityState.cooldown:
                   if (m_2ndCooldownTime > 0)
                   {
                       m_2ndCooldownTime -= Time.deltaTime;
                       m_2ndAbilityImage.color = cooldownColor;
                   }
                   else
                   {
                       m_2NdState = m_2ndAbilityState.ready;
                       m_2ndAbilityImage.color = readyColor;
                   }
                   break;
           }
        */
    }
    public void ButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityUsed = true;
        }
        if (context.performed)
        {
            abilityUsed = true;
        }
        if (context.canceled)
        {
            abilityUsed = false;
        }
    }

    bool abilityWorking1 = false;
    public void PortalDropAbilityUse(InputAction.CallbackContext context)
    {
        if (context.canceled && abilityWorking1)
        {
            abilityWorking1 = false;
            tempPortSpawnRef.PortalDrop(context);
        }


        else if (context.performed && currentAbilityValue >= portalDropCostValue)
        {
            abilityWorking1 = true;
            currentAbilityValue -= portalDropCostValue;
            tempPortSpawnRef.PortalDrop(context);
        }
    }

    public void BlinkAbilityUse(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            tempBlinkRef.BlinkTo();
                chargingSound.Stop();
                blinkSound.Play();
        }
        else if(context.started)
        {
            if (currentAbilityValue >= blinkCostValue)
            {
                chargingSound.Play(); 
            }
        }
        else if (context.performed && /* currentResourceValue*/ currentAbilityValue >= blinkCostValue)
        {
            currentAbilityValue -= blinkCostValue;
            tempBlinkRef.SpawnMirage();
        }
    }


    public void AllMaBars()
    {
        m_CarClass = accessCarValues.carClasses;
        //boostBar.minValue = minBoostBarValue;
        // boostBar.maxValue = maxBoostBarValue;
        //boostBar.value = currentBoostBarValue;
        boostBarBar.fillAmount = currentBoostBarValue / maxBoostBarValue;
        abilityBarBar.fillAmount = currentAbilityValue / maxAbilityValue;
        //abilityBar.minValue = minAbilityValue;
        // abilityBar.maxValue = maxAbilityValue;
        //abilityBar.value = currentAbilityValue;
        switch (m_CarClass)
        {
            case Class.Light:
                // for light car boost value is replaced by limit remover 

                if (accessCarValues.currentSpeed > m_SpeedThreshholdForResource)
                {
                    if (currentBoostBarValue < maxBoostBarValue)

                    {
                        if (abilityWorking1 == true)
                        {


                        }
                      
                        else
                        {
                            currentBoostBarValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                            //  Boostbarbar.fillAmount = currentBoostBarValue / maxBoostBarValue; 
                            //currentBoostBarValue = currentBlinkValue = currentPDValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                        }
                    }

                    if (currentAbilityValue < maxAbilityValue)
                    {
                        currentAbilityValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                    }


                }
                else if (currentBoostBarValue == maxBoostBarValue)
                {
                    currentBoostBarValue = maxBoostBarValue;
                }
                else if (currentAbilityValue == maxAbilityValue)
                {
                    currentAbilityValue = maxAbilityValue;
                }


                break;

            case Class.Medium:
                m_ResoureceGatherCheck = accessCarValues.mediumCar;
                if (accessCarValues.currentSpeed > m_SpeedThreshholdForResource)
                {
                   
                    if (currentBoostBarValue < maxBoostBarValue)

                    {
                        currentBoostBarValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                        //currentBoostBarValue = currentBlinkValue = currentPDValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                    }

                    if (currentAbilityValue < maxAbilityValue)
                    {
                        currentAbilityValue += Time.deltaTime * m_ResourceMultiplerForSpeed;
                    }

                }

                else if (m_ResoureceGatherCheck == true)
                {
                    if (currentBoostBarValue < maxBoostBarValue)

                    {
                        currentBoostBarValue += Time.deltaTime * m_ResourceMultiplerForDrifting;
                    }

                    
                    if (currentAbilityValue < maxAbilityValue)
                    {
                        currentAbilityValue += Time.deltaTime * m_ResourceMultiplerForDrifting;
                    }

                    
                    else if (currentBoostBarValue == maxBoostBarValue)
                    {
                        currentBoostBarValue = maxBoostBarValue;
                    }
                    else if (currentAbilityValue == maxAbilityValue)
                    {
                        currentAbilityValue = maxAbilityValue;
                    }

                }
                break;

            case Class.Heavy:
                m_ResoureceGatherCheck = accessCarValues.heavyCar;
                if (m_ResoureceGatherCheck == true)
                {
                   
                        if (currentBoostBarValue < maxBoostBarValue)

                        {
                            currentBoostBarValue += Time.deltaTime * m_ResourceMultiplerForDrifting;
                        }

                        if (currentAbilityValue < maxAbilityValue)
                        {
                            currentAbilityValue += Time.deltaTime * m_ResourceMultiplerForDrifting;
                        }
                    
                  
                    else if (currentBoostBarValue == maxBoostBarValue)
                    {
                        currentBoostBarValue = maxBoostBarValue;
                    }
                    else if (currentAbilityValue == maxAbilityValue)
                    {
                        currentAbilityValue = maxAbilityValue;
                    }

                }
                break;
        }


    }


}
