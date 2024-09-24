using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    // getting values from car
    public Transform player;
    public Rigidbody rb;

    // value to change the position of the camera
    public Vector3 offset;

    // used for looking behind the player
    public Vector3 reversOffset;
    public float speed;
    public Car_Movement carValues; 
   
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
       
    }



    private void CameraUpdate()
    {
        Vector3 playerForward = (rb.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(player.position, player.position + player.TransformVector(offset) + playerForward /** (-1f)*/, speed * Time.deltaTime);
        transform.LookAt(player);
        
        if(carValues.ture == true)
        {
            transform.position = Vector3.Lerp(player.position, player.position + player.TransformVector(reversOffset) + playerForward /** (-1f)*/, speed * Time.deltaTime);
            transform.LookAt(player);
        }

    }

   

}
