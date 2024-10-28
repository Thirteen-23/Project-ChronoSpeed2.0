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
        if (other.CompareTag("CarBody"))
        {
            //This is the main player, noone else has a rigidbody client side
            rb.useGravity = true;
            Vector3 vel = other.GetComponentInParent<VerleyVelocity>().Velocity;

            //if more than half speed, double the hit force cause itd be fun (apparently this is to much so i had to remove it : (
            Vector3 newVelocity = vel;
            newVelocity.y = 5;
            rb.AddForceAtPosition(newVelocity, GetComponent<Collider>().ClosestPointOnBounds(other.transform.position), ForceMode.Impulse);

            Destroy(gameObject, 5);
        }
    }
}
