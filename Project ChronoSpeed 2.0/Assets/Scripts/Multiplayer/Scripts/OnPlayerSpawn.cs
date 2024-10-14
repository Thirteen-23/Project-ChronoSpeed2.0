using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnPlayerSpawn : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        var input = GetComponent<PlayerInput>();
        var lapMan = GetComponent<LapManager>();

        if (IsOwner)
        {
            MainCamera mCam = FindAnyObjectByType<MainCamera>();
            Rigidbody bodyRB = GetComponentInChildren<Rigidbody>();

            input.enabled = false;
            mCam.rb = bodyRB;
            mCam.player = transform.Find("CameraFollow");
        }
        else
        {
            Destroy(input);
            Destroy(GetComponent<Car_Movement>());
            gameObject.tag = "OtherPlayer";
        }

        Debug.Log(IsServer);
        if(!IsServer)
            Destroy(lapMan);
        
        base.OnNetworkSpawn();
    }
}
