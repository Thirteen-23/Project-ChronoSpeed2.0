using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FakeCollision : MonoBehaviour
{
    Class myClass;
    float myMass = 0;
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
        myMass = GetMass(myClass);
    }
    Dictionary<Transform, CollisionRequiredInfo> currentCollidingCars;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody") || other.transform.CompareTag("OtherPlayer") || other.transform.CompareTag("AIBody"))
        {
            //Class theirClass = other.transform.GetComponentInParent<Car_Movement>().carClasses;
            //currentCollidingCars.Add(other.transform, new CollisionRequiredInfo(theirClass, GetInverseMass(theirClass), other.transform.GetComponent<Rigidbody>()));

            //GetOuttaThereStep(other);
            CollideLol(GetComponentInChildren<BoxCollider>(), other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
            return;
        //GetOuttaThereStep(other);
        if(other.transform.CompareTag("CarBody") || other.transform.CompareTag("OtherPlayer") || other.transform.CompareTag("AIBody"))
        {
            Debug.Log(other.name);
            CollideLol(GetComponentInChildren<BoxCollider>(), other);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody") || other.transform.CompareTag("OtherPlayer") || other.transform.CompareTag("AIBody"));
            //currentCollidingCars.Remove(other.transform);
    }

    

    void CollideLol(Collider me, Collider other)
    {
        Vector3 dir;
        float dist;

        Physics.ComputePenetration(me, transform.position, transform.rotation, other, other.transform.position, other.transform.rotation, out dir, out dist);
        dir.y = 0; dir.Normalize();

        transform.root.position += dir * dist;
        Debug.Log("Direction: " + dir + ", Distance: " + dist + "TotalAmount: " + (dir * dist));
        //GetOuttaThereStep(other);
    }

    void GetOuttaThereStep(Collider other)
    {
        CollisionRequiredInfo dontNeedThis;
        Debug.Log(!currentCollidingCars.TryGetValue(other.transform, out dontNeedThis));

        if (!currentCollidingCars.TryGetValue(other.transform, out dontNeedThis))
            return;

        float otherMass = currentCollidingCars[other.transform].theirMass;

        Vector3 relativeVelocity = myRB.velocity - currentCollidingCars[other.transform].theirRB.velocity;
        Vector3 normal = transform.position - other.transform.position;

        normal.y = 0;
        normal.Normalize();

        float dot = Vector3.Dot(relativeVelocity, normal);
        dot *= myMass + otherMass;
        normal *= dot;
        Vector3 velChange = normal / myMass;

        myRB.AddForce(velChange, ForceMode.VelocityChange);
    }

    float GetMass(Class c)
    {
        if (c == Class.Heavy)
            return 90f;
        else if (c == Class.Medium)
            return 60f;
        else if (c == Class.Light)
            return 30f;
        else
            Debug.Log(transform.root.name + " myClass didnt work properly, isnt one of the three types");

        return 0;
    }
}
