using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PortalSpawn : MonoBehaviour
{
    [SerializeField] float maxHoldTime;
    [SerializeField] float cooldown;

    Coroutine curForceReleaseCor;
    bool useable = true;

    Vector3 startPos;
    Quaternion startRot;
    public void PortalDrop(CallbackContext callbackContext)
    {
        if (!useable) { return; }

        if (callbackContext.started)
        {
            startPos = transform.position + transform.forward * -4;
            startRot = transform.rotation;
            MultiplayerGameManager.Singleton.SpawnPortalStandInRpc(startPos, startRot);
            curForceReleaseCor = StartCoroutine(ForceRelease());
        }
        else if (callbackContext.canceled)
        {
            OnRelease();
        }
    }


    void OnRelease()
    {
        if(startPos == Vector3.zero && startRot == Quaternion.identity) { return; }
        MultiplayerGameManager.Singleton.SpawnPortalRpc(startPos, startRot, transform.position + transform.forward * -4, transform.rotation);

        startPos = Vector3.zero;
        startRot = Quaternion.identity;

        if (curForceReleaseCor != null)
        {
            StopCoroutine(ForceRelease());
            curForceReleaseCor = null;
        }

        StartCoroutine(Cooldown());
    }
    IEnumerator ForceRelease()
    {
        yield return new WaitForSeconds(maxHoldTime);
        OnRelease();
    }

    IEnumerator Cooldown()
    {
        useable = false;
        yield return new WaitForSeconds(cooldown);
        useable = true;
    }
}
