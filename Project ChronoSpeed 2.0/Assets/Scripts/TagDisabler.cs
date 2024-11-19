using UnityEngine;

public class TagDisabler : MonoBehaviour
{
    public string targetTag = "Lights";
    public string triggerTag = "LightsTrigger";
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            ToggleChildObjectsInHierarchy();
        }
    }

    private void ToggleChildObjectsInHierarchy()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(targetTag))
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }
    }
}