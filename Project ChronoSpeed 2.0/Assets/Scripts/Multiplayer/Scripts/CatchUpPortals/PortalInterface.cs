using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalInterface : MonoBehaviour
{
    private BoxCollider BC;
    private Volume postVFX;
    [SerializeField] private Animation portal;
    [SerializeField] private Animation vfx;

    private void Awake()
    {
        BC = GetComponent<BoxCollider>();
        postVFX = GetComponentInChildren<Volume>();
    }
    public void TurnOff()
    {
        if (BC.enabled == false)
            return;
        BC.enabled = false;
        postVFX.enabled = false;
        if (portal != null) portal.Play("DP_Close");
        if (vfx != null) vfx.Play("DP_Close_VFX");
    }
    public void TurnOn() 
    {
        if (BC.enabled == true)
            return;
        BC.enabled = true;
        postVFX.enabled = true;
        if (portal != null) portal.Play("DP_Open");
        if (vfx != null) vfx.Play("DP_Open_VFX");
    }
}
