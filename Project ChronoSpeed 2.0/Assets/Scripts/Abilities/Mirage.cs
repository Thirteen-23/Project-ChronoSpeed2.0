using Unity.Netcode;
using UnityEngine;

public class Mirage : NetworkBehaviour
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
    }

    public void Disapate()
    {
        Destroy(Instantiate(disapateAnimAndAudio, transform.position, transform.rotation), 1);
    }

    private void Update()
    {
        transform.localPosition += new Vector3(0,0,1) * 10 * Time.deltaTime;
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            transform.parent = player.transform;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity); 
            
            blParent = player.GetComponent<Blink>();
            player.GetComponent<Blink>().SetMirage(this);
        }
    }
}
