using System.Collections.Generic;
using UnityEngine;

public class PortalAbility : MonoBehaviour
{
    [SerializeField] public float timePortLasts { get; private set; } = 60f;
    PortalAbility linkedPortal;
    List<GameObject> incomingPlayers = new List<GameObject>();

    public void LinkPortal(PortalAbility portToLink)
    {
        if (linkedPortal == null) linkedPortal = portToLink;
    }
    public void AddIncomingPlayer(GameObject player)
    {
        if(!incomingPlayers.Contains(player)) incomingPlayers.Add(player);
    }    
    private void OnTriggerEnter(Collider other)
    {
        //Parent object in car contains rigidbody, so find it to find top parent in hierachy
        GameObject player = other.GetComponentInParent<Rigidbody>().gameObject;
        
        //Checks if the player just got teleported here, if so do nothing
        if (incomingPlayers.Contains(player))
        {
            incomingPlayers.Remove(player);
            return;
        }

        //Tell the other portal that you are teleporting this player
        linkedPortal.AddIncomingPlayer(player);


        Matrix4x4 relativeMatrix = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix;
        

        Matrix4x4 newCarPos = relativeMatrix * player.transform.localToWorldMatrix;
        player.transform.SetPositionAndRotation(newCarPos.GetPosition(), newCarPos.rotation);


        Vector3 oldVel = player.GetComponent<Rigidbody>().velocity;
        Vector3 newVel = relativeMatrix * new Vector4(oldVel.x, oldVel.y, oldVel.z, 1);
        player.GetComponent<Rigidbody>().velocity = newVel;

    }
}
