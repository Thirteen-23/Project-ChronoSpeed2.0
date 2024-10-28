using UnityEngine;

public class PropHit : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarBody") || other.CompareTag("AIBody") || other.CompareTag("Player") || other.CompareTag("OtherPlayer"))
        {
            //This is the main player, noone else has a rigidbody client side
            rb.isKinematic = false;
            Rigidbody tempRB = other.GetComponentInParent<Rigidbody>();

            if (tempRB == null)
            {
                Vector3 newVelocity = other.GetComponentInParent<VerleyVelocity>().Velocity;
                
                rb.AddForceAtPosition(newVelocity, GetComponent<Collider>().ClosestPoint(other.transform.position), ForceMode.Impulse);
            }
            else
                rb.AddForceAtPosition(tempRB.velocity, GetComponent<Collider>().ClosestPoint(other.transform.position), ForceMode.Impulse);

            Destroy(gameObject, 5);
        }
    }
}
