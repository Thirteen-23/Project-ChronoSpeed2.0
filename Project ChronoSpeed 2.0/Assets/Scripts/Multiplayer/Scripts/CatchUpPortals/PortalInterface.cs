using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInterface : MonoBehaviour
{
    private BoxCollider BC;
    [SerializeField] private Animation portal;
    [SerializeField] private Animation vfx;

    private void Awake()
    {
        BC = GetComponent<BoxCollider>();
        TurnOff();
    }
    public void TurnOff()
    {
        BC.enabled = false;
        portal.Play();
        if (portal != null) portal.Play("DP_Close");
        if (vfx != null) vfx.Play("DP_Close_VFX");
    }
    public void TurnOn() 
    {
        BC.enabled = true;
        if (portal != null) portal.Play("DP_Open");
        if (vfx != null) vfx.Play("DP_Open_VFX");
    }
}
