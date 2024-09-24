using UnityEngine;

public class Mirage : MonoBehaviour
{
    public Blink blParent;


    [SerializeField] GameObject disapateAnimAndAudio;
    private void OnTriggerEnter(Collider other)
    {
        //This means i can spawn it server side and it wont do nothin
        if (blParent == null)
            return;

        if (other.CompareTag("RigidBodyObj") || other.CompareTag("CarBody") || other.CompareTag("ground"))
            return;

        blParent.BreakMirage();
        Disapate();
        Destroy(gameObject);
    }

    public void Disapate()
    {
        Destroy(Instantiate(disapateAnimAndAudio, transform.position, transform.rotation), 1);
    }

    private void Update()
    {
        transform.position = transform.position + transform.forward * Time.deltaTime;
    }
}
