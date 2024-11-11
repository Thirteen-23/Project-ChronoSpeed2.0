
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;


public class AIRewind : MonoBehaviour
{
    [SerializeField] float rewindTime = 5;

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
    public void Rewind()
    {
        isRecording = false;
        if(curRewindCor == null) curRewindCor = StartCoroutine(RewindCor());
    }


    void OnRelease()
    {
        if (isRecording) return;
        
        carRigidbody.velocity = storedData[0].Velocity / 2;
        storedData.Clear();
        currentRewindIteration = 0;

        isRecording = true;
        curRewindCor = null;
    }


    IEnumerator RewindCor()
    {
        currentRewindIteration = storedData.Count - 1;

        while (currentRewindIteration > 0)
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

}
