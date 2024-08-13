using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PortalVisual : MonoBehaviour
{
    public PortalVisual LinkedPortal;
    public MeshRenderer PortalSkin;

    Camera playerCam;
    Camera portalCam;
    RenderTexture portalTexture;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = true;
        CreatePortalTexture();
    }

    void CreatePortalTexture()
    {
        if (portalTexture == null || portalTexture.width != Screen.width || portalTexture.height != Screen.height)
        {
            if(portalTexture != null)
            {
                portalTexture.Release();
            }
            portalTexture = new RenderTexture(Screen.width, Screen.height, 0);

            portalCam.targetTexture = portalTexture;

            LinkedPortal.PortalSkin.material.SetTexture("_PortalTexture", portalTexture);
        }
    }

    private void Update()
    {
        Matrix4x4 relativeMatrix = transform.localToWorldMatrix * LinkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(relativeMatrix.GetColumn(3), relativeMatrix.rotation);
    }
}
