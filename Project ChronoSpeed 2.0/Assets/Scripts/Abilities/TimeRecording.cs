using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class TimeRecording : MonoBehaviour
{
    [SerializeField] float rewindTime = 5;
    [SerializeField] float cooldown = 5;
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

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
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
    }

    IEnumerator Rewind()
    {
        currentRewindIteration = storedData.Count - 1;

        while(currentRewindIteration > 0)
        {
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

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isUsable = true;
    }
}
