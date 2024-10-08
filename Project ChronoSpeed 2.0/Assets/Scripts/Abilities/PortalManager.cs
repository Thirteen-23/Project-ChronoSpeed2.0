using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private GameObject portalStandIn;
    [SerializeField] private GameObject portal;

    List<GameObject> portalStandInList = new List<GameObject>();
    public void SpawnPortalStandIn(Vector3 position, quaternion rotation)
    {
        portalStandInList.Add(Instantiate(portalStandIn, position, rotation));
    }

    public void SpawnPortal(Vector3 firstPortalPos, Vector3 secondPortalPos, quaternion firstPortalRotation, quaternion secondPortalRotation, float portalLast)
    {
        for(int i = 0; i < portalStandInList.Count; i++)
        {
            if (portalStandInList[i].transform.position == firstPortalPos)
            {
                if (portalStandInList[i].transform.rotation == firstPortalRotation)
                {
                    Destroy(portalStandInList[i]);
                    portalStandInList.RemoveAt(i);
                    break;
                }
            }
        }

        PortalAbility firstPort = Instantiate(portal, firstPortalPos, firstPortalRotation).GetComponent<PortalAbility>();
        PortalAbility secondPort = Instantiate(portal, secondPortalPos, secondPortalRotation).GetComponent<PortalAbility>();
        
        firstPort.LinkPortal(secondPort);
        secondPort.LinkPortal(firstPort);

        Destroy(firstPort.gameObject, portalLast);
        Destroy(secondPort.gameObject, portalLast);
    }
}
