using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class TimeRecording : MonoBehaviour
{
    [SerializeField] float rewindTime = 5;
    [SerializeField] float cooldown = 5;
    [SerializeField] VolumeProfile rewindProfile;

    struct RecordedData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
    }
    List<RecordedData> storedData = new List<RecordedData>();

    Rigidbody carRigidbody;
    Coroutine curRewindCor;

    int currentRewindIteration;
    bool isRecording = true;
    bool isUsable = true;
    Volume mainCamVol;
    

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        mainCamVol = Camera.main.GetComponent<Volume>();
    }
    private void FixedUpdate()
    {
        if(isRecording)
        {
            storedData.Add(new RecordedData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = carRigidbody.velocity
            });

            if (storedData.Count > 50 * rewindTime) storedData.RemoveAt(0);
        }
    }

    public void Rewind(CallbackContext callbackContext)
    {
        if (!isUsable) return;
        if(callbackContext.performed)
        {
            isRecording = false;
            curRewindCor = StartCoroutine(Rewind());
        }
        else if(callbackContext.canceled)
        {
            OnRelease();
        }
    }

    void OnRelease()
    {
        if (isRecording) return;
        isRecording = true;
        if (currentRewindIteration >= 0)
        {
            carRigidbody.velocity = storedData[currentRewindIteration].Velocity;
            storedData.RemoveRange(currentRewindIteration, storedData.Count  - currentRewindIteration);
        }
        else
        {
            carRigidbody.velocity = storedData[0].Velocity;
            storedData.Clear();
            currentRewindIteration = 0;
        }

        isUsable = false;
        StartCoroutine(Cooldown());
        StartCoroutine(DischargeVisual());
    }

    bool affectVolume = false;
    IEnumerator Rewind()
    {
        currentRewindIteration = storedData.Count - 1;

        
        affectVolume = !mainCamVol.HasInstantiatedProfile();
        Debug.Log(affectVolume);
        if(affectVolume) 
        {
            mainCamVol.profile = rewindProfile;
            mainCamVol.weight = 0;
        }
        

        while (currentRewindIteration > 0)
        {
            if (affectVolume)
            {
                float halfRewindTime = rewindTime / 2; //2.5f
                mainCamVol.weight += 1 / (halfRewindTime * 50);
                if (mainCamVol.weight > 1) mainCamVol.weight = 1;
            }
            

            if (isRecording && curRewindCor != null)
            {
                StopCoroutine(curRewindCor);
            }

            RecordedData _tempRD = storedData[currentRewindIteration];
            transform.position = _tempRD.Position;
            transform.rotation = _tempRD.Rotation;

            currentRewindIteration -= 2;
            yield return new WaitForFixedUpdate();
        }
        OnRelease();
    }

    public IEnumerator DischargeVisual()
    {
        affectVolume = mainCamVol.profile == rewindProfile;
        if(!affectVolume) { yield break; }
        while (mainCamVol.weight >  0)
        {
            //Maybe increase sound for somethin
            mainCamVol.weight -= Time.deltaTime * 2;
            yield return null;
        }
        mainCamVol.weight = 0;
        mainCamVol.profile = null;

    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isUsable = true;
    }
}
