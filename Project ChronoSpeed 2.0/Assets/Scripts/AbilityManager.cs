using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public float cooldownTime;
    public float activeTime;
    public float m_2ndCooldownTime;
    public float m_2ndActiveTime;
    public bool abilityUsed = false;
    public bool m_2ndAbilityUsed = false;
    abilityState state = abilityState.ready;
    m_2ndAbilityState m_2NdState = m_2ndAbilityState.ready;
    // Update is called once per frame
    void Update()
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

                }
                else
                {
                    state = abilityState.ready;
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

                }
                else
                {
                    m_2NdState = m_2ndAbilityState.ready;
                }
                break;
        }



    }

    public void ButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            abilityUsed = true;
            Debug.Log("pressed");
        }
        if (context.performed)
        {
            abilityUsed = true;
            Debug.Log("holding");
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
            Debug.Log("pressed");
        }
        if (context.performed)
        {
            m_2ndAbilityUsed = true;
            Debug.Log("holding");
        }
        if (context.canceled)
        {
            m_2ndAbilityUsed = false;
        }
    }
}
