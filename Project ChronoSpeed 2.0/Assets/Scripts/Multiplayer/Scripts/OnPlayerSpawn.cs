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
        var carvalues = GetComponent<Car_Movement>();
        if (IsOwner)
        {
            MainCamera mCam = FindAnyObjectByType<MainCamera>();
            Rigidbody bodyRB = GetComponentInChildren<Rigidbody>();
            GameObject cameraGameObject = GameObject.FindGameObjectWithTag("CameraFollow"); 
            
            input.enabled = false;
            mCam.rb = bodyRB;
            mCam.player = cameraGameObject.transform;
            mCam.carValues = carvalues;
           //bodyRB.transform;
        }
        else
        {
            Destroy(input);
            Destroy(carvalues);
            gameObject.tag = "OtherPlayer";
        }

        Debug.Log(IsServer);
        if(!IsServer)
            Destroy(lapMan);
        
        base.OnNetworkSpawn();
    }
}
