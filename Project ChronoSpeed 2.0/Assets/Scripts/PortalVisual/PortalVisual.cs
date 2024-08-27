
using UnityEngine;


public class PortalVisual : MonoBehaviour
{
    public PortalVisual LinkedPortal;
    public MeshRenderer PortalSkin;

    Camera playerCam;
    Camera portalCam;
    RenderTexture portalTexture;
    public Transform IncomingCar; 

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = true;
        
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
        if (LinkedPortal != null)
        {
            CreatePortalTexture();
            Matrix4x4 relativeMatrix = transform.localToWorldMatrix * LinkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
            portalCam.transform.SetPositionAndRotation(relativeMatrix.GetColumn(3), relativeMatrix.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody carRB = other.GetComponentInParent<Rigidbody>();
        Debug.Log(carRB == null);
        if (LinkedPortal != null & carRB != null)
        {
            if (carRB.transform == IncomingCar)
            {
                IncomingCar = null;
                return;
            }
            Debug.Log("It continued");
            Matrix4x4 relativeMatrix = LinkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix;

            Matrix4x4 carPosition = relativeMatrix * carRB.transform.localToWorldMatrix;
            carRB.transform.SetPositionAndRotation(carPosition.GetColumn(3), carPosition.rotation);

            Vector4 vel = carRB.velocity;
            Debug.Log(vel.w);
            Vector4 velChange = relativeMatrix * vel;
            carRB.velocity = velChange;

            LinkedPortal.IncomingCar = carRB.transform;
        }
    }

}
