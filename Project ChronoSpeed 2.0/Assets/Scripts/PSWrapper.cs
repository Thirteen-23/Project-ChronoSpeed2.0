using Unity.Netcode;
using UnityEngine.InputSystem;

public class PSWrapper : NetworkBehaviour
{
    private PauseScreen m_PauseScreen;

    private void Start()
    {
        if(IsOwner)
        {
            m_PauseScreen = FindAnyObjectByType<PauseScreen>();
            m_PauseScreen.mainPlayerInput = GetComponent<PlayerInput>();
        }
            
    }
    public void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if(context.performed)
            m_PauseScreen.OpenPauseMenu();
    }

}
