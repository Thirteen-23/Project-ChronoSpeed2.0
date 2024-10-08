using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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

    private void FixedUpdate()
    {
        for (int i = sparks.Length; i > contactAmount; i--)
        {
            sparks[i - 1].Stop();
        }
        contactAmount = 0;
    }

}