using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb;
    public Vector3 offset;
    public float speed;

    void Start()
    {

    }

    void FixedUpdate()
    {
        CameraUpdate();
    }



    private void CameraUpdate()
    {
        Vector3 playerForward = (rb.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position, player.position + player.TransformVector(offset) + playerForward * (-5f), speed * Time.deltaTime);
        transform.LookAt(player);
    }


}
