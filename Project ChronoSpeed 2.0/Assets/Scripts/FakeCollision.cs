using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FakeCollision : MonoBehaviour
{
    Class myClass;
    float myInvMass = 0;
    Rigidbody myRB;
    public struct CollisionRequiredInfo
    {
        public Class theirClass;
        public float theirMass;
        public Rigidbody theirRB;
        public CollisionRequiredInfo(Class c, float m, Rigidbody rb)
        {
            theirClass = c;
            theirMass = m;
            theirRB = rb;
        }
    }
    private void Awake()
    {
        myClass = GetComponentInParent<Car_Movement>().carClasses;
        myRB = GetComponentInParent<Rigidbody>();
        myInvMass = GetInverseMass(myClass);
    }
    Dictionary<Transform, CollisionRequiredInfo> currentCollidingCars;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody") || other.transform.CompareTag("AIBody"))
        {
            Class theirClass = other.transform.GetComponentInParent<Car_Movement>().carClasses;
            currentCollidingCars.Add(other.transform, new CollisionRequiredInfo(theirClass, GetInverseMass(theirClass), other.transform.GetComponent<Rigidbody>()));
            GetOuttaThereStep(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
            return;
        GetOuttaThereStep(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody") || other.transform.CompareTag("AIBody"))
            currentCollidingCars.Remove(other.transform);
    }
    private void OnTriggerEnter(Collision collision)
    {
        Debug.Log(transform.name);

        
        

    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    void GetOuttaThereStep(Collider other)
    {
        CollisionRequiredInfo dontNeedThis;
        Debug.Log(!currentCollidingCars.TryGetValue(other.transform, out dontNeedThis));

        if (!currentCollidingCars.TryGetValue(other.transform, out dontNeedThis))
            return;

        float otherMass = currentCollidingCars[other.transform].theirMass;

        //Do depen later, need to make game rn : (
        Vector3 collisionDirection = (transform.root.position - other.ClosestPoint(transform.root.position)).normalized;

        //Vector3 relativeVelocity = myRB.velocity - currentCollidingCars[other.transform].theirRB.velocity;
        //Vector3 normal = transform.position - other.transform.position;

        //normal.y = 0;
        //normal.Normalize();

        //float dot = Vector3.Dot(relativeVelocity, normal);
        //dot *= myInvMass + otherMass;
        //normal *= dot;
        //Vector3 velChange = normal * myInvMass;

        transform.root.position += collisionDirection * (myInvMass + otherMass) * myInvMass;

        //Debug.Log(velChange);

        //myRB.AddForce(velChange, ForceMode.VelocityChange);
    }

    float GetInverseMass(Class c)
    {
        if (c == Class.Heavy)
            return 0.1f;
        else if (c == Class.Medium)
            return 0.5f;
        else if (c == Class.Light)
            return 0.9f;
        else
            Debug.Log(transform.root.name + " myClass didnt work properly, isnt one of the three types");

        return 0;
    }
}
