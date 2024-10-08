using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackerForAI : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] AI car;

    [Header("AI Brain")]
    public TextMeshProUGUI m_AIBrain;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(car.aiSpeaking  == AIMouth.slowing_Down)
        {
            m_AIBrain.text = "slowing "; 
        }
        if (car.aiSpeaking == AIMouth.speeding_Up)
        {
            m_AIBrain.text = "speeding ";
        }
        if (car.aiSpeaking == AIMouth.racing)
        {
            m_AIBrain.text = "racing ";
        }
        if (car.aiSpeaking == AIMouth.reversing)
        {
            m_AIBrain.text = "reversing ";
        }
    }
}
