using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ability : ScriptableObject
{
    public new string name;
    public float activeTime;
    public float cooldownTime;
    public Gamepad left;

    public virtual void Activate(GameObject parent) {}
    public virtual void BeginCooldown(GameObject parent) {}


   
}
