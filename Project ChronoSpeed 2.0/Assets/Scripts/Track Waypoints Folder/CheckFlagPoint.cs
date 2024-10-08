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
        GameObject parentObj = other.GetComponentInParent<Rigidbody>().gameObject;
        if (parentObj.CompareTag("Player") || parentObj.CompareTag("OtherPlayer") || parentObj.CompareTag("AI") || parentObj.CompareTag("AIBody"))
        {
            if (isFirstCheckpoint)
                lapMan.CarHitFirstCheckPoint(parentObj, transform);
            else
                lapMan.CarHitCheckPoint(parentObj, transform);
        }
    }
}
