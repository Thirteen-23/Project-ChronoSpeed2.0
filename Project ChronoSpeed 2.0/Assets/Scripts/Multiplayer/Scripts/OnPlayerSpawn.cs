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
            GameObject cameraGameObject = GameObject.FindGameObjectWithTag("CameraFollow"); 
            
            input.enabled = false;
            mCam.rb = bodyRB;
            mCam.player = cameraGameObject.transform; 
           //bodyRB.transform;
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
