using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamCycle : MonoBehaviour
{
    public GameObject[] objects; // Array to hold the GameObjects to cycle through
    private int currentIndex = 0; // Index of the currently active object
    public GameObject lookBackCam;
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
       /* if (Input.GetKeyDown(KeyCode.C))
        {
            CycleObjects();
        }*/
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

    public void SwapView(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CycleObjects();
        }
    }

    public void lookback(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            objects[currentIndex].SetActive(false);
         
        }
        if (context.performed)
        {
            lookBackCam.SetActive(true);

        }
        if (context.canceled)
        {
            lookBackCam.SetActive(false);
            objects[currentIndex].SetActive(true);
        }
    }
  
}
