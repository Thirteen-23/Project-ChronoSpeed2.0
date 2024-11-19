using UnityEngine;
using UnityEngine.VFX;

public class Mirage : MonoBehaviour
{
    public Blink blParent;


    [SerializeField] GameObject disapateAnimAndAudio;
    private void OnTriggerEnter(Collider other)
    {
        //This means i can spawn it server side and it wont do nothin
        if (blParent == null)
            return;

        if (!other.CompareTag("walls"))
            if(!other.CompareTag("Tarmac"))
                if (!other.CompareTag("SideWalk"))
                    return;

        blParent.BreakMirage();
        Disapate();
    }

    public void Disapate()
    {
        Destroy(Instantiate(disapateAnimAndAudio, transform.position, transform.rotation), 1);
    }

    private void Update()
    {
        transform.localPosition += new Vector3(0,0,1) * 10 * Time.deltaTime;
    }

    private void OnTransformParentChanged()
    {
        GameObject elecArc = Instantiate(transform.parent.GetComponentInChildren<VFXContainer>().elecArcPrefab, Vector3.zero, Quaternion.identity);
        transform.parent.GetComponentInChildren<VFXContainer>().electricArc = elecArc.GetComponent<VisualEffect>();

        blParent = GetComponentInParent<Blink>();
        blParent.SetMirage(this);
    }
}
