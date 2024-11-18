using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFlagPoint : MonoBehaviour
{
    Tracking_Manager_Script lapMan;
    public bool isFirstCheckpoint;

    private void Awake()
    {
        lapMan = GetComponentInParent<Tracking_Manager_Script>();
        isFirstCheckpoint = gameObject.CompareTag("Finish/StartingLine");
    }
    private void OnTriggerEnter(Collider other)
    {
        FakeCollision parentObj = other.GetComponent<FakeCollision>();
        if (parentObj != null)
        {
            if (isFirstCheckpoint)
                lapMan.CarHitFirstCheckPoint(parentObj.myTransform.gameObject, transform);
            else
                lapMan.CarHitCheckPoint(parentObj.myTransform.gameObject, transform);
        }
    }
}
