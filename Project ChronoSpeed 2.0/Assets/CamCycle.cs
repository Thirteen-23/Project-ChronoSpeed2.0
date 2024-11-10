using UnityEngine;

public class CamCycle : MonoBehaviour
{
    public GameObject[] objects; // Array to hold the GameObjects to cycle through
    private int currentIndex = 0; // Index of the currently active object

    void Start()
    {
        // Ensure all objects are disabled except the first one
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        // Check for key press (e.g., spacebar)
        if (Input.GetKeyDown(KeyCode.C))
        {
            CycleObjects();
        }
    }

    void CycleObjects()
    {
        // Disable the currently active object
        objects[currentIndex].SetActive(false);

        // Increment the index and wrap around if necessary
        currentIndex = (currentIndex + 1) % objects.Length;

        // Enable the next object
        objects[currentIndex].SetActive(true);
    }
}
