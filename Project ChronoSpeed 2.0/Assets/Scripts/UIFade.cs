using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public float duration = 1.0f; // Duration of the fade in seconds
    public bool fadeIn = true; // Whether to fade in (true) or fade out (false)
    
    private float timer = 0.0f;
    private Image image;

    void Start()
    {
        // Get the Image component
        image = GetComponent<Image>();

        // Set the initial alpha based on fade type
        if (image != null)
        {
            if (fadeIn)
            {
                // If fading in, start with alpha at 0
                Color color = image.color;
                color.a = 0f;
                image.color = color;
            }
            else
            {
                // If fading out, start with alpha at 1
                Color color = image.color;
                color.a = 1f;
                image.color = color;
            }
        }
    }

    void Update()
    {
        // Increment timer based on time passed
        timer += Time.deltaTime;

        // Calculate the progress of the fade
        float progress = Mathf.Clamp01(timer / duration);

        // Calculate target alpha based on fade type
        float targetAlpha = fadeIn ? 1f : 0f;

        // Interpolate alpha based on progress
        float currentAlpha = Mathf.Lerp(image.color.a, targetAlpha, progress);

        // Update the image's alpha value
        Color color = image.color;
        color.a = currentAlpha;
        image.color = color;

        // If fade is complete, disable the script to stop updating
        if ((fadeIn && currentAlpha >= 1.0f) || (!fadeIn && currentAlpha <= 0.0f))
        {
            enabled = false;
        }
    }
}