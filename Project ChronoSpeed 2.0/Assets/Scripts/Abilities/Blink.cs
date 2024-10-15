using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Blink : MonoBehaviour
{
    [SerializeField] VolumeProfile chargeProfile;
    [SerializeField] VolumeProfile warpProfile;
    Volume mainCamVol;

    VFXContainer m_VFXContainer;

    Mirage currentMirage;
    SphereCollider mirageCol;
    Coroutine chargeCoroutine;
    Coroutine warpCoroutine;
    Coroutine dischargeCoroutine;

    private void Awake()
    {
        m_VFXContainer = GetComponent<VFXContainer>();
        mainCamVol = FindAnyObjectByType<MainCamera>().GetComponent<Volume>(); ;
    }
    public void SpawnMirage()
    {
        m_VFXContainer.SetVFX(VFXManager.VFXTypes.electricBall, true);
        VFXManager.AlterVFXState(transform.root.gameObject, VFXManager.VFXTypes.electricBall, true);
        //Maybe do a little 0.5 second charge sound
        currentMirage.enabled = true;
        mirageCol.enabled = true;

        currentMirage.transform.localPosition = new Vector3(0,1, 2.5f);
        chargeCoroutine = StartCoroutine(ChargeVisual());
        //tell the server
    }

    public void BreakMirage()
    {
        m_VFXContainer.SetVFX(VFXManager.VFXTypes.electricBall, false);
        VFXManager.AlterVFXState(transform.root.gameObject, VFXManager.VFXTypes.electricBall, false);

        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        chargeCoroutine = null;

        dischargeCoroutine = StartCoroutine(DischargeVisual());
        currentMirage.enabled = false;
        mirageCol.enabled = false;

        //tell the server
    }

    public void BlinkTo()
    {
        //this means it hit something and has been turned off before it can be hit
        if (!currentMirage.enabled)
            return;

        m_VFXContainer.SetVFX(VFXManager.VFXTypes.electricBall, false);
        VFXManager.AlterVFXState(transform.root.gameObject, VFXManager.VFXTypes.electricBall, false);

        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        chargeCoroutine = null;

        warpCoroutine = StartCoroutine(WarpVisual());

        //Do sound

        transform.position = currentMirage.transform.position - new Vector3(0,1,0);
        currentMirage.transform.localPosition = new Vector3(0, 1, 2.5f);
        currentMirage.enabled = false;
        currentMirage.enabled = false;
    }


    public IEnumerator ChargeVisual()
    {
        mainCamVol.profile = chargeProfile;
        mainCamVol.weight = 0;
        while (mainCamVol.weight < 1)
        {
            //Maybe increase sound for somethin
            mainCamVol.weight += Time.deltaTime / 3f;
            yield return null;
        }
        mainCamVol.weight = 1;

        
        chargeCoroutine = null;

    }
    public IEnumerator WarpVisual()
    {
        
        mainCamVol.profile = warpProfile;
        mainCamVol.weight = 1;

        while(mainCamVol.weight > 0)
        {
            mainCamVol.weight -= Time.deltaTime * 2f;
            yield return null;
        }
        mainCamVol.weight = 0;
        warpCoroutine = null;
    }

    public IEnumerator DischargeVisual()
    {

        while (mainCamVol.weight > 0)
        {
            mainCamVol.weight -= Time.deltaTime * 2f;
            yield return null;
        }
        mainCamVol.weight = 0;
        
    }

    public void SetMirage(Mirage curMir)
    {
        currentMirage = curMir;
        mirageCol = curMir.GetComponent<SphereCollider>();

        Transform electricTarget = GetComponent<VFXContainer>().electricArc.transform.GetChild(0);
        electricTarget.parent = currentMirage.transform;

        currentMirage.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        electricTarget.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
    }
}