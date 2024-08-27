using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public List<Collider> checkPointHit = new List<Collider>();
    public int checkpointCount = 0;
    public int totalCheckpoints;
    public int lapCompeted = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!checkPointHit.Contains(other))
        {
            checkpointCount++;
            checkPointHit.Add(other);

        }
        if (checkpointCount == totalCheckpoints)
        {
            lapCompeted++;
            checkpointCount = 0;
            checkPointHit.Clear();

        }
    }
}
