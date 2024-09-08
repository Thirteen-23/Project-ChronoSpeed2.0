using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OnPlayerSpawn : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            Debug.Log(SceneManager.GetActiveScene());
            //Camera Work
            MainCamera mCam = FindAnyObjectByType<MainCamera>();
            Rigidbody bodyRB = GetComponentInChildren<Rigidbody>();
            mCam.rb = bodyRB;
            mCam.player = bodyRB.transform;
            
        }
        else
        {
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<Car_Movement>().enabled = false;  
        }
        base.OnNetworkSpawn();
    }
}
