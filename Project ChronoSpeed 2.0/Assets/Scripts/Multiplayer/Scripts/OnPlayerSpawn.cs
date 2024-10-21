using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnPlayerSpawn : NetworkBehaviour
{
    [SerializeField] Collider Trigger;
    [SerializeField] Collider NotTrigger;
    public override void OnNetworkSpawn()
    {
        var input = GetComponent<PlayerInput>();
        var lapMan = GetComponent<LapManager>();
        var carMove = GetComponent<Car_Movement>();

        if (IsOwner)
        {
            MainCamera mCam = FindAnyObjectByType<MainCamera>();
            Rigidbody bodyRB = GetComponentInChildren<Rigidbody>();

            input.enabled = false;
            mCam.rb = bodyRB;
            mCam.player = transform.Find("CameraFollow");
            mCam.carValues = carMove;

            //Destroy(Trigger);
        }
        else
        {
            Destroy(input);
            Destroy(carMove);
            Destroy(GetComponentInChildren<FakeCollision>());
            gameObject.tag = "OtherPlayer";

            //Destroy(NotTrigger);
        }

        Debug.Log(IsServer);
        if(!IsServer)
            Destroy(lapMan);
        
        base.OnNetworkSpawn();
    }
}
