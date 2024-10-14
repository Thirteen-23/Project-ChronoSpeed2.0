using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FakeCollision : MonoBehaviour
{
    Class myClass;

    private void Awake()
    {
        myClass = GetComponentInParent<Car_Movement>().carClasses;
    }
    Dictionary<Transform, Class> currentCollidingCars;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("CarBody") || collision.transform.CompareTag("AIBody"))
        {
            currentCollidingCars.Add(collision.transform, collision.transform.GetComponentInParent<Car_Movement>().carClasses);

            GetOuttaThereStep(collision);
        }
        

    }

    private void OnCollisionStay(Collision collision)
    {
        GetOuttaThereStep(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        currentCollidingCars.Remove(collision.transform);
    }

    void GetOuttaThereStep(Collision collision)
    {

    }
}
