using UnityEngine;
using System.Collections;

public class OneShotThenLoopController : MonoBehaviour
{
    [Header("Sources (assign in Inspector)")]
    public AudioSource introSource;   // loop = false
    public AudioSource loopSource;    // loop = true

    [Header("Clips (optional if prewired on the sources)")]
    public AudioClip introClip;
    public AudioClip loopClip;

    [Header("Defaults")]
    public bool restartIfAlreadyPlaying = true;  // if PlayChain() is called while playing
    public float defaultFadeOutTime = 0.25f;     // used by StopChain() without args

    Coroutine chainCo;
    Coroutine fadeCo;

    // --- PUBLIC API ---

    /// <summary>Start the intro then the loop. Safe to call multiple times.</summary>
    public void PlayChain()
    {
        if (!EnsureSetup()) return;

        // If we're already playing:
        if (IsAnyPlaying())
        {
            if (!restartIfAlreadyPlaying) return;
            StopEverythingImmediate();
        }

        chainCo = StartCoroutine(ChainRoutine());
    }

    /// <summary>Stop both intro and loop immediately (no fade).</summary>
    public void StopChainImmediate()
    {
        StopEverythingImmediate();
    }

    /// <summary>Stop both intro and loop with a fade (seconds). If time <= 0, immediate.</summary>
    public void StopChain(float fadeOutTime = -1f)
    {
        if (fadeOutTime < 0f) fadeOutTime = defaultFadeOutTime;

        if (!IsAnyPlaying())
            return;

        if (fadeCo != null) StopCoroutine(fadeCo);
        if (chainCo != null) StopCoroutine(chainCo);

        if (fadeOutTime <= 0f)
        {
            StopEverythingImmediate();
        }
        else
        {
            fadeCo = StartCoroutine(FadeOutAndStop(fadeOutTime));
        }
    }

    /// <summary>Only stop the looping bed (leave intro alone if you want).</summary>
    public void StopLoopOnly(float fadeOutTime = -1f)
    {
        if (fadeOutTime < 0f) fadeOutTime = defaultFadeOutTime;

        if (loopSource && loopSource.isPlaying)
        {
            if (fadeOutTime <= 0f) { loopSource.Stop(); loopSource.volume = 1f; }
            else StartCoroutine(FadeOutOne(loopSource, fadeOutTime, stopOnEnd:true));
        }
    }

    // --- INTERNALS ---

    IEnumerator ChainRoutine()
    {
        // Prepare sources
        introSource.loop = false;
        loopSource.loop = true;

        // Assign clips if provided here (okay if already set on sources)
        if (introClip) introSource.clip = introClip;
        if (loopClip)  loopSource.clip  = loopClip;

        // Safety checks
        if (!introSource.clip && !loopSource.clip)
        {
            Debug.LogWarning("[OneShotThenLoop] No clips assigned.");
            yield break;
        }

        // Reset volumes (in case we faded previously)
        introSource.volume = 1f;
        loopSource.volume = 1f;

        // Play intro if available, otherwise go straight to loop
        if (introSource.clip)
        {
            introSource.Play();
            while (introSource.isPlaying) yield return null;
        }

        if (loopSource.clip)
        {
            loopSource.Play();
        }

        chainCo = null;
    }

    IEnumerator FadeOutAndStop(float time)
    {
        // Fade both (only those that are playing)
        IEnumerator f1 = (introSource && introSource.isPlaying) ? FadeOutOne(introSource, time, stopOnEnd:true) : null;
        IEnumerator f2 = (loopSource  && loopSource.isPlaying)  ? FadeOutOne(loopSource,  time, stopOnEnd:true) : null;

        // Run in parallel
        if (f1 != null) StartCoroutine(f1);
        if (f2 != null) StartCoroutine(f2);

        // Wait until both are done
        float t = 0f;
        while (t < time) { t += Time.unscaledDeltaTime; yield return null; }

        // Cleanup
        if (chainCo != null) { StopCoroutine(chainCo); chainCo = null; }
        fadeCo = null;
    }

    IEnumerator FadeOutOne(AudioSource src, float time, bool stopOnEnd)
    {
        if (!src) yield break;
        float start = src.volume;
        float t = 0f;
        while (t < time && src) // guard if destroyed
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Clamp01(t / time);
            src.volume = start * k;
            yield return null;
        }
        if (src)
        {
            if (stopOnEnd) src.Stop();
            src.volume = 1f; // reset for next play
        }
    }

    void StopEverythingImmediate()
    {
        if (fadeCo != null) { StopCoroutine(fadeCo); fadeCo = null; }
        if (chainCo != null) { StopCoroutine(chainCo); chainCo = null; }

        if (introSource) { introSource.Stop(); introSource.volume = 1f; }
        if (loopSource)  { loopSource.Stop();  loopSource.volume  = 1f; }
    }

    bool IsAnyPlaying()
    {
        return (introSource && introSource.isPlaying) || (loopSource && loopSource.isPlaying);
    }

    bool EnsureSetup()
    {
        if (!introSource || !loopSource)
        {
            Debug.LogWarning("[OneShotThenLoop] Assign both AudioSources.");
            return false;
        }
        // Either clips on sources or provided here:
        bool hasIntro = introSource.clip || introClip;
        bool hasLoop  = loopSource.clip  || loopClip;
        if (!hasIntro && !hasLoop)
        {
            Debug.LogWarning("[OneShotThenLoop] No clips found on sources or fields.");
            return false;
        }
        return true;
    }

    void OnDisable()
    {
        // Optional: stop if object is disabled (prevents orphaned loops)
        StopChainImmediate();
    }
}
