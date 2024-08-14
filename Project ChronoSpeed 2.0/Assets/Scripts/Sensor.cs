using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField]
    public float range; 
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.forward;
        Ray theSensor = new Ray(transform.position, transform.TransformDirection(direction * range));
        Debug.DrawRay(transform.position, transform.TransformDirection(direction * range));

        if (Physics.Raycast(theSensor, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                Debug.Log("Hit the enivroment");

               
            }

        }
    }
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
            //Rigidbody _rb;
            //_rb = gameObject.GetComponent<Rigidbody>();

            //_rb.AddForce(0, 100, 0);
            
        }
    }

    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
        //    Rigidbody _rb;
        //    _rb = gameObject.GetComponent<Rigidbody>();

        //    _rb.AddForce(0, 0, 0);
        }
    }

}

