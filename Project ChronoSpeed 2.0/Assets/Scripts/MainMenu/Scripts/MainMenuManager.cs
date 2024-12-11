using Cinemachine;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] float CameraSpeed;
    [SerializeField] CinemachineDollyCart cart;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] GameObject pivotPoint;

    private UIManager uiManager;

    private float targetPosition;
    float time;
    private Vector3 startRotation;
    [SerializeField] public Vector3 targetRotation;

    private int targetUI;

    private void Awake()
    {
        uiManager = GetComponentInChildren<UIManager>();
    }

    private void Update()
    {
        if (pivotPoint.transform.eulerAngles != targetRotation)
        {
            time += Time.deltaTime;
            if(time > 0.5f) time = 0.5f;
            float yLerp = Mathf.LerpAngle(startRotation.y, targetRotation.y, time / 0.5f);
            Vector3 Lerped = new Vector3(0, yLerp, 0);
            pivotPoint.transform.eulerAngles = Lerped;
        }
    }
    public void OpenMainMenuCG()
    {
        for(int i = 1; i < (int)UIManager.MainMenuStates.Count; i++)
        {
            targetUI = i;
            SwitchUI(0);
        }
        targetUI = 0;
        SwitchUI(1);
    }
    public void OpenJoinCG()
    {
        targetUI = 0;
        SwitchUI(0);

        targetUI = 1;
        SwitchUI(1);
    }

    public void OpenOptionsCG()
    {
        targetUI = 0;
        SwitchUI(0);

        targetUI = 2;
        SwitchUI(1);
    }

    public void OpenCarSelectCG()
    {
        //targetUI = 0;
        //SwitchUI(0);

        //targetUI = 3;
        //SwitchUI(3);
        //this gets called with a delay inbetween in SwitchCameraArea
    }

    public void SwitchCameraArea(int i)
    {
        SwitchUI(0);
        if(i == 1)
        {
            targetUI = 0;
            targetPosition = 0;
        }
        else if(i == 2)
        {
            targetUI = 3;
            targetPosition = 61.5f;
        }
        else if(i == 3)
        {
            targetUI = 4;
            targetPosition = 100;
        }
        StartCoroutine(MoveCamera());
    }

    public void PivotCars(int angleY)
    {
        startRotation = pivotPoint.transform.eulerAngles;
        targetRotation = new Vector3(0, angleY, 0);
        time = 0;
    }
    void SwitchUI(int alpha)
    {
        uiManager.SwitchUI(targetUI, alpha);
    }
    private IEnumerator MoveCamera()
    {
        bool isInFrontOfPoint = cart.m_Position > targetPosition;

        while(cart.m_Position != targetPosition)
        {
            if(isInFrontOfPoint)
            {
                if(cart.m_Position < targetPosition)
                {
                    cart.m_Position = targetPosition;
                    cart.m_Speed = 0;
                    continue;
                }
            }
            else
            {
                if(cart.m_Position > targetPosition)
                {
                    cart.m_Position = targetPosition;
                    cart.m_Speed = 0;
                    continue;
                }
            }

            cart.m_Speed = isInFrontOfPoint ? -CameraSpeed : CameraSpeed;
            yield return new WaitForFixedUpdate();
        }

        SwitchUI(1);            
    }

    
    
    public void StartHost()
    {
        ServerManager.Singleton.StartHost();
    }
    public void JoinHost()
    {
        ServerManager.Singleton.StartClient(ipInput.text);
    }

    public void fullscreen()
    {
        // toogle Fullscreen
        Screen.fullScreen = !Screen.fullScreen;
    }

}
