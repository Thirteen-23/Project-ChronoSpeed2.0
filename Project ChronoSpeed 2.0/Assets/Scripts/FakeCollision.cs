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
        if(other.transform.CompareTag("CarBody") || other.transform.CompareTag("AIBody"))
        {
            Debug.Log(other.name);
            CollideLol(GetComponentInChildren<BoxCollider>(), other);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody") || other.transform.CompareTag("AIBody")) ;
            //currentCollidingCars.Remove(other.transform);
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

    void CollideLol(Collider me, Collider other)
    {
        //https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Physics.ComputePenetration.html

        float MinX1 = -me.bounds.extents.x;
        float MaxX1 = me.bounds.extents.x;
        float MinY1 = -me.bounds.extents.z;
        float MaxY1 = me.bounds.extents.z;

        float MinX2 = -other.bounds.extents.x;
        float MaxX2 = other.bounds.extents.x;
        float MinY2 = -other.bounds.extents.z;
        float MaxY2 = other.bounds.extents.z;

        float xOverlap1 = MaxX1 - MinX2;
        float xOverlap2 = MaxX2 - MinX1;
        float yOverlap1 = MaxY1 - MinY2;
        float yOverlap2 = MaxY2 - MinY1;

        float distance = 0;
        Vector2 direction = new Vector2(0,0);
        switch (FindSmallestOfFour(xOverlap1, xOverlap2, yOverlap1, yOverlap2))
        {
            case 0:
                distance += xOverlap1;
                direction = new Vector2(1,0);
                break;
            case 1:
                distance += xOverlap2;
                direction = new Vector2(-1,0);
                break;
            case 2:
                distance += yOverlap1;
                direction = new Vector2(0,1);
                break;
            case 3:
                distance += yOverlap2;
                direction = new Vector2(0,-1);
                break;
        }
        Vector3 offset = new Vector3((distance * direction).x, 0, (distance * direction).y);
        transform.root.position += offset;

        Debug.Log(offset);
    }

    int FindSmallestOfFour(float a, float b, float c, float d)
    {
        if (a < b && a < c && a < d) return 0;
        if (b < c && b < d) return 1;
        if (c < d) return 2;
        return 3;
    }
}
