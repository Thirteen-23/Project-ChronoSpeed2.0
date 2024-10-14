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

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.isTrigger)
            return;

        if (collision.transform.CompareTag("CarBody") || collision.transform.CompareTag("AIBody"))
        {
            Class theirClass = collision.transform.GetComponentInParent<Car_Movement>().carClasses;
            currentCollidingCars.Add(collision.transform, new CollisionRequiredInfo(theirClass, GetInverseMass(theirClass),collision.transform.GetComponent<Rigidbody>()));
            GetOuttaThereStep(collision);
        }
        

    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.isTrigger)
            return;
        GetOuttaThereStep(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.isTrigger)
            return;

        if (collision.transform.CompareTag("CarBody") || collision.transform.CompareTag("AIBody"))
            currentCollidingCars.Remove(collision.transform);
    }

    void GetOuttaThereStep(Collision collision)
    {
        CollisionRequiredInfo dontNeedThis;
        if (!currentCollidingCars.TryGetValue(collision.transform, out dontNeedThis))
            return;

        float otherMass = currentCollidingCars[collision.transform].theirMass;

        //Do depen later, need to make game rn : (
        Vector3 collisionDirection = (transform.root.position - collision.contacts[0].point).normalized;

        Vector3 relativeVelocity = myRB.velocity - currentCollidingCars[collision.transform].theirRB.velocity;
        Vector3 normal = transform.position - collision.transform.position;

        normal.y = 0;
        normal.Normalize();

        float dot = Vector3.Dot(relativeVelocity, normal);
        dot *= myInvMass + otherMass;
        normal *= dot;
        Vector3 velChange = normal * myInvMass;

        myRB.AddForce(velChange, ForceMode.VelocityChange);
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
