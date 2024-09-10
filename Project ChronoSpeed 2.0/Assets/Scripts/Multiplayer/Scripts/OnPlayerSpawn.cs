using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            mCam.player = bodyRB.transform;
        }
        else
        {
            Destroy(input);
            Destroy(GetComponent<Car_Movement>());
            gameObject.tag = "OtherPlayer";
        }
        

        if(IsServer)
            lapMan.waypoints = FindAnyObjectByType<TrackWayPoints>();
        else
            Destroy(lapMan);
        
        base.OnNetworkSpawn();
    }
}
