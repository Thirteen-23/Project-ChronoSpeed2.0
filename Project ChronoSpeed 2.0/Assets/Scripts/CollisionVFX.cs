using UnityEngine;

public class CollisionVFX : MonoBehaviour
{

    [SerializeField] ParticleSystem[] sparks;
    private int contactAmount = 0;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("walls") || other.CompareTag("AIBody") || other.CompareTag("CarBody") || other.CompareTag("RigidBodyObj"))
        {
            if (contactAmount >= sparks.Length)
                return;

            Vector3 contactPosition = other.ClosestPoint(transform.position);
            if (!sparks[contactAmount].isPlaying)
                sparks[contactAmount].Play();
            sparks[contactAmount].transform.position = contactPosition;
            sparks[contactAmount].transform.localRotation = Quaternion.Euler(0, Mathf.Sign(GetComponentInParent<Rigidbody>().velocity.magnitude) == 1 ? 180 : 0, 0);
            contactAmount++;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("walls") || collision.transform.CompareTag("AIBody") || collision.transform.CompareTag("CarBody") || collision.transform.CompareTag("RigidBodyObj"))
        {
            if (contactAmount >= sparks.Length)
                return;

            Vector3 contactPosition = collision.contacts[0].point;
            if (!sparks[contactAmount].isPlaying)
                sparks[contactAmount].Play();
            sparks[contactAmount].transform.position = contactPosition;
            sparks[contactAmount].transform.localRotation = Quaternion.Euler(0, Mathf.Sign(GetComponentInParent<Rigidbody>().velocity.magnitude) == 1 ? 180 : 0, 0);
            contactAmount++;
        }
    }

    private void FixedUpdate()
    {
        for (int i = sparks.Length; i > contactAmount; i--)
        {
            sparks[i - 1].Stop();
        }
        contactAmount = 0;
    }

}