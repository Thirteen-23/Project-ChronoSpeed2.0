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
        ready,
        active,
        charging
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
    public Image m_1stAbilityImage;
    public Image m_2ndAbilityImage;
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    [Header("Resource Meter Values")]
    public Car_Movement accessCarValues;
    public Slider maBar;
    public int maxResourceValue;
    public float currentResourceValue;
    public int minResourceValue;
    public bool checkcheck = false;
    ResourceState resourceState = ResourceState.charging;
    public float ability1CostValue; 
    private void Awake()
    {
        accessCarValues = GetComponent<Car_Movement>();
    }

    private void Start()
    {
        m_1stAbilityImage.color = readyColor;
        m_2ndAbilityImage.color = readyColor;

    }

    void FixedUpdate()
    {
        checkcheck = accessCarValues.heavyCar;
        maBar.minValue = minResourceValue;
        maBar.maxValue = maxResourceValue;
        maBar.value = currentResourceValue;
        if (checkcheck == true)
        {
            if (currentResourceValue < maxResourceValue)

            {
                currentResourceValue += Time.deltaTime * 5;
            }
            else if(currentResourceValue == maxResourceValue)
            {
                currentResourceValue = maxResourceValue; 
            }
        }
        // abilityCoolDownAbility();
        //AIUsingAbilities();
        switch (resourceState)
        {
            case ResourceState.charging:
                if (ability1CostValue> currentResourceValue)
                {
                    Debug.Log("not ready Buddy"); 
                }
                else
                {
                    resourceState = ResourceState.ready;
                }
                    break;
            case ResourceState.ready:
                {
                        Debug.Log("Abiity ready");
                    if (abilityUsed == true && ability1CostValue <= currentResourceValue)
                    {
                        resourceState = ResourceState.active; 
                    }
                }
                    break;
            case ResourceState.active:
                {
                    if (currentResourceValue >= ability1CostValue)
                    {
                       
                        Debug.Log("ability used!!");
                      currentResourceValue = currentResourceValue - ability1CostValue; 

                        if(ability1CostValue <=currentResourceValue)
                        {
                            resourceState = ResourceState.ready;
                            m_1stAbility.Activate(gameObject);
                        }
                        else
                        {
                            break;
                        }
                    }
                    resourceState = ResourceState.charging;
                    
                }
                break;
        }
    }
    private void abilityCoolDownAbility()
    {
        switch (state)
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

    }
    public void ButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityUsed = true;
          //  Debug.Log("pressed");
        }
        if (context.performed)
        {
            abilityUsed = true;
           // Debug.Log("holding");
        }
        if (context.canceled)
        {
            abilityUsed = false;
        }
    }

    public void AbilityUse2nd(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_2ndAbilityUsed = true;
            //Debug.Log("pressed");
        }
        if (context.performed)
        {
            m_2ndAbilityUsed = true;
          //  Debug.Log("holding");
        }
        if (context.canceled)
        {
            m_2ndAbilityUsed = false;
        }
    }


}
