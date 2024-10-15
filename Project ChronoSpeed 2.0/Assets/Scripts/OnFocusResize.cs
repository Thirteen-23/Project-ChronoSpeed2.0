using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnFocusResize : MonoBehaviour
{
    private bool wasFocused = false;
    private Coroutine currentCoroutine;

    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private float resizeDuration = 0.5f;
    [SerializeField] private Vector3 focusedSize = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private Vector3 unfocusedSize = new Vector3(1f, 1f, 1f);

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject && !wasFocused)
        {
            wasFocused = true;
            StartResizing(focusedSize);
        }
        else if (EventSystem.current.currentSelectedGameObject != gameObject && wasFocused)
        {
            wasFocused = false;
            StartResizing(unfocusedSize);
        }
    }

    private void StartResizing(Vector3 targetSize)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ResizeObjects(targetSize));
    }

    private IEnumerator ResizeObjects(Vector3 targetSize)
    {
        float elapsedTime = 0f;

        Vector3[] originalSizes = new Vector3[gameObjects.Length];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                originalSizes[i] = gameObjects[i].transform.localScale;
            }
        }

        while (elapsedTime < resizeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / resizeDuration;
            float curveValue = sizeCurve.Evaluate(t);

            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i] != null)
                {
                    gameObjects[i].transform.localScale = Vector3.Lerp(originalSizes[i], targetSize, curveValue);
                }
            }

            yield return null;
        }

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                gameObjects[i].transform.localScale = targetSize;
            }
        }
    }

    private void OnDisable()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }
}