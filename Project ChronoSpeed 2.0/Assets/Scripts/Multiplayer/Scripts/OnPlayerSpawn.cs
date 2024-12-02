using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnPlayerSpawn : NetworkBehaviour
{
    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject CameraJunk;
    public override void OnNetworkSpawn()
    {
        var input = GetComponent<PlayerInput>();
        var lapMan = GetComponent<LapManager>();
        var carMove = GetComponent<Car_Movement>();

        if (IsOwner)
        {
            Rigidbody bodyRB = GetComponentInChildren<Rigidbody>();

            input.enabled = false;
        }
        else
        {
            Destroy(input);
            Destroy(carMove);
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<TimeRecording>());
            gameObject.tag = "OtherPlayer";

            foreach (Transform child in transform)
            {
                if (child.CompareTag("lights"))
                {
                    Destroy(child.gameObject);
                }
            }

            Destroy(CameraJunk);
            Destroy(Canvas);
        }
        if(!IsServer)
            Destroy(lapMan);
        
        base.OnNetworkSpawn();
    }
}
