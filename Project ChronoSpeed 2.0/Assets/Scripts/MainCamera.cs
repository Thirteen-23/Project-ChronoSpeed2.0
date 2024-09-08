using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb;
    public Vector3 offset;
    public float speed;
  /// <summary>
  /// Hopefully a camerashake at highspeeds
  /// </summary>
    public Transform camtransform;
    public float shakeAmount = 0.7f;
    public float descreasfactor = 1f;
    public bool itsShaking = false;
    Vector3 originalPos;
    float originalShakeDuration;
    float shakeDuration = 0f; 
    void Start()
    {
        if(camtransform == null)
        {
            camtransform = GetComponent(typeof(Transform)) as Transform;
            
        }
        originalPos = camtransform.localPosition;
        originalShakeDuration = shakeDuration;
    }

    private void OnEnable()
    {
       
    }
    void FixedUpdate()
    {
        CameraUpdate();
        if(itsShaking == true)
        {
            if(originalShakeDuration > 0)
            {
                camtransform.localPosition = Vector3.Lerp(camtransform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount, Time.deltaTime * 3);
                shakeDuration -= Time.deltaTime * descreasfactor;
            }
            else
            {
                shakeDuration = originalShakeDuration;
                camtransform.localPosition = originalPos;
                itsShaking = false; 
            }
        }
    }



    private void CameraUpdate()
    {
        Vector3 playerForward = (rb.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position, player.position + player.TransformVector(offset) + playerForward * (-1f), speed * Time.deltaTime);
        transform.LookAt(player);

       
        
    }


}
