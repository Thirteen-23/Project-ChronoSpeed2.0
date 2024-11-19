using UnityEngine;

public class TagDisabler : MonoBehaviour
{
    public string targetTag = "Lights";
    public string triggerTag = "LightsTrigger";

    GameObject[] lights;

    private void Start()
    {
        lights = GameObject.FindGameObjectsWithTag(targetTag);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Utopia"))
        {
            SwitchHeadlightState(false);
        }
        else if(other.CompareTag("Dystopia"))
        {
            SwitchHeadlightState(true);
        }
    }

    private void SwitchHeadlightState(bool switchTo)
    {
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(switchTo);
        }
    }
}