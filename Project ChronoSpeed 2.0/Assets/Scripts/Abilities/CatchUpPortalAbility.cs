using System.Collections.Generic;
using UnityEngine;

public class CatchUpPortalAbility : MonoBehaviour
{
    [SerializeField] CatchUpPortalAbility linkedPortal;
    List<GameObject> incomingPlayers = new List<GameObject>();

    [Header("PORTAL FUNCTIONALITY \nUse Km/h, Portal uses functionality of whichever is more than 0")]
    [SerializeField] float IncreaseSpeedBy = 0;
    bool speedIncreasePortal = false;
    [SerializeField] float SetSpeedTo = 0;
    bool speedSetPortal = false;

    private void Start()
    {
        if (linkedPortal == null)
            return;
        speedIncreasePortal = linkedPortal.IncreaseSpeedBy > 0;
        speedSetPortal = linkedPortal.SetSpeedTo > 0;

        //Conversion from gabes km/h meter to unity forces
        IncreaseSpeedBy = linkedPortal.IncreaseSpeedBy / 3.6f;
        SetSpeedTo = linkedPortal.SetSpeedTo / 3.6f;
    }

    public void AddIncomingPlayer(GameObject player)
    {
        if(!incomingPlayers.Contains(player)) incomingPlayers.Add(player);
    }    
    private void OnTriggerEnter(Collider other)
    {
        if (linkedPortal == null)
            return;

        if (!other.CompareTag("CarBody") && other.isTrigger)
            return;
        //Parent object in car contains rigidbody, so find it to find top parent in hierachy
        GameObject player = other.GetComponentInParent<Rigidbody>().gameObject;

        player.transform.rotation = linkedPortal.transform.rotation;

        player.transform.position = linkedPortal.transform.position;
        float offset = other.bounds.extents.z;
        player.transform.position += player.transform.forward * offset;

        Rigidbody carRB = other.GetComponentInParent<Rigidbody>();
        Vector3 newDirection = linkedPortal.transform.forward;
        if (speedIncreasePortal)
            carRB.velocity = newDirection * (Mathf.Abs(carRB.velocity.magnitude) + IncreaseSpeedBy);
        else if (speedSetPortal)
            carRB.velocity = newDirection * SetSpeedTo;
        else
            carRB.velocity = newDirection * Mathf.Abs(carRB.velocity.magnitude);
    }
}
