using Cinemachine;
using UnityEngine;

public class PortalJank : MonoBehaviour
{
    [SerializeField] GameObject vcam;
    [SerializeField] CinemachineVirtualCamera m_VCam;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform player = other.GetComponentInParent<Rigidbody>().transform;
        Vector3 currentPosition = player.position;
        m_VCam.PreviousStateIsValid = false;

        //Vector3 newOffset = other.GetComponentInParent<Rigidbody>().transform.position - vcam.transform.position;
        player.position += new Vector3(0, 0, 200);
        CinemachineCore.Instance.OnTargetObjectWarped(player, new Vector3(0, 0, 200));
        // vcam.transform.position = other.transform.position + newOffset;

    }
}
