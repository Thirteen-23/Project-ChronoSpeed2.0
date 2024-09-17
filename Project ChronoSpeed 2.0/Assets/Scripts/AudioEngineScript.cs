using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class AudioEngineScript : MonoBehaviour
//{
   
//    Car_Movement car;
//    public AudioSource source;
//    public AudioSource source1;
//    public AudioSource source2;
//    public float minPitch = 0.6f;
//    public float maxPitch = 1f;
//    public float pitchFromCar;

//    // Start is called before the first frame update
//    void Start()
//    {
//        car = GetComponent<Car_Movement>();
//        source = GetComponentInChildren<AudioSource>();
//        source.pitch = minPitch;
//    }

//    private void Update()
//    {
//        float speedoSnap = 0.0001f;
//        float velo = 0;
//        pitchFromCar = car.engineRPM /10000f + minPitch;
     
//        if (pitchFromCar < 0)
//        {
//            source.pitch = minPitch;
//            source.volume = 1; 
//        }
//        if(pitchFromCar == maxPitch)
//        {

//        }
//        else
//        {

//            source.pitch = pitchFromCar ;
//        }


//    }


//}

[System.Serializable]
public class EngineNote
{
    public AudioSource source;
    public float minRPM;
    public float peakRPM;
    public float maxRPM;
    public float pitchReferenceRPM;

    public float SetPitchAndGetVolumeForRPM(float rpm)
    {
        source.pitch = rpm / pitchReferenceRPM;

        if (rpm < minRPM || rpm > maxRPM)
        {
            return 0f;
        }

        if (rpm < peakRPM)
        {
            return Mathf.InverseLerp(minRPM, peakRPM, rpm);
        }
        else
        {
            return Mathf.InverseLerp(maxRPM, peakRPM, rpm);
        }
    }

    public void SetVolume(float volume)
    {
        source.mute = (source.volume = volume) == 0;
    }

}

public class AudioEngineScript : MonoBehaviour
{
    public EngineNote[] engineNotes;
    public Car_Movement carValues; 
    public float rpm;
    public float masterVolume;

    static float[] workingVolumes = new float[3]; // or maximum number of engine notes you need

    private void Start()
    {
        carValues = GetComponent<Car_Movement>();
      
    }
    private void Update()
    { float maxVol = 5;
        float progress = Mathf.Sin(Time.deltaTime * 0.02f);
        rpm = Mathf.Lerp((int)carValues.engineRPM, maxVol, progress) ;
        // The total volume calculated for all engine notes won't generally sum to 1.
        // Calculate what they do sum to and then scale the individual volumes to ensure
        // consistent volume across the RPM range.
        float totalVolume = 0f;
        for (int i = 0; i < engineNotes.Length; ++i)
        {
            totalVolume += workingVolumes[i] = engineNotes[i].SetPitchAndGetVolumeForRPM(rpm);
        }

        if (totalVolume > 0f)
        {
            for (int i = 0; i < engineNotes.Length; ++i)
            {
                engineNotes[i].SetVolume(masterVolume * workingVolumes[i] / totalVolume);
            }
        }
    }

}

