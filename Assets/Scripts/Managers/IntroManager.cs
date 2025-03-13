using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{    
    [SerializeField] Image _overlay;
    
    MenuManager _menuManager;

    void Start()
    {
        _menuManager = MenuManager.Instance;

        StartCoroutine(FadeOutOverlay());        
    }

    IEnumerator FadeOutOverlay()
    {
        if (_overlay == null) yield break;

        yield return new WaitForSecondsRealtime(Constants.Durations.IntroOverlayFadeDelay);

        Color initialColor = _overlay.color;
        float initialAlpha = initialColor.a;
        float targetAlpha = 0f;

        // Interpolate over time
        float elapsed = 0f;

        // Previous implementation works in Windows build but not in WebGL build:
        //while (elapsed < duration)
        //{
        //    elapsed += Time.deltaTime;
        //    float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / duration);
        //    SetAlpha(graphic, newAlpha);
        //    yield return null;
        //}

        float startTime = Time.unscaledTime; // Use unscaled time to avoid issues with timeScale

        while (elapsed < Constants.Durations.IntroOverlayFadeout)
        {
            elapsed = Time.unscaledTime - startTime;
            float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / Constants.Durations.IntroOverlayFadeout);
            Utils.SetGraphicAlpha(_overlay, newAlpha);
            yield return null;
        }
        
        Utils.SetGraphicAlpha(_overlay, targetAlpha); // Ensure the final alpha is set

        StartCoroutine(_menuManager.FadeInCanvas());
    }
}
