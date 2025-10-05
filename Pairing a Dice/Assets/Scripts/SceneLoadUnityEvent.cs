using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)] // Run very early so Awake option truly fires ASAP
public class SceneLoadUnityEvent : MonoBehaviour
{
    public enum TriggerTiming
    {
        Awake,          // As soon as this component awakens (very early in load)
        OnEnable,       // When this component is enabled
        Start,          // After all Awake calls have run
        AfterSceneLoaded, // Uses SceneManager.sceneLoaded (safe when object might enable late)
        NextFrame       // First frame after Start (after one Update loop begins)
    }

    [Header("When do you want the event to run?")]
    public TriggerTiming timing = TriggerTiming.Awake;

    [Tooltip("Optional delay (seconds) before invoking the event.")]
    public float delay = 0f;

    [Tooltip("Prevent multiple invocations from this component instance.")]
    public bool onlyOnce = true;

    [Header("Event to invoke")]
    public UnityEvent OnTriggered;

    bool _fired = false;
    bool _subscribed = false;

    void Awake()
    {
        if (timing == TriggerTiming.Awake)
            TryInvoke();
    }

    void OnEnable()
    {
        if (timing == TriggerTiming.OnEnable)
        {
            TryInvoke();
        }
        else if (timing == TriggerTiming.AfterSceneLoaded && !_subscribed)
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            _subscribed = true;
        }
    }

    void Start()
    {
        switch (timing)
        {
            case TriggerTiming.Start:
                TryInvoke();
                break;
            case TriggerTiming.NextFrame:
                StartCoroutine(InvokeAfterDelayCoroutine(Mathf.Max(0f, delay), waitOneFrame: true));
                break;
        }
    }

    void OnDisable()
    {
        if (_subscribed)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            _subscribed = false;
        }
    }

    void OnDestroy()
    {
        if (_subscribed)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            _subscribed = false;
        }
    }

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Fire once per component per scene load
        TryInvoke();
        if (onlyOnce)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            _subscribed = false;
        }
    }

    void TryInvoke()
    {
        if (onlyOnce && _fired) return;

        if (delay > 0f && timing != TriggerTiming.NextFrame)
        {
            StartCoroutine(InvokeAfterDelayCoroutine(delay, waitOneFrame: false));
        }
        else
        {
            // For zero delay, still route through coroutine if NextFrame was selected
            if (timing == TriggerTiming.NextFrame)
                StartCoroutine(InvokeAfterDelayCoroutine(0f, waitOneFrame: true));
            else
                InvokeNow();
        }
    }

    System.Collections.IEnumerator InvokeAfterDelayCoroutine(float seconds, bool waitOneFrame)
    {
        if (waitOneFrame)
            yield return null; // wait one frame (after Start/first Update)

        if (seconds > 0f)
            yield return new WaitForSeconds(seconds);

        InvokeNow();
    }

    void InvokeNow()
    {
        if (onlyOnce && _fired) return;

        _fired = true;
        try
        {
            OnTriggered?.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }
}
