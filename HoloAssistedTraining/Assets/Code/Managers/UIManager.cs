using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Progress Bar")]
    public Image progressBar;
    public uint totalSteps = 10;
    private uint _currentStep;
    public uint currentStep
    {
        get
        {
            return _currentStep;
        }
        set
        {
            _currentStep = value;
            UpdateProgressBar();
        }
    }

    [Header("Clock Timer")]
    public Image clockImage;
    public Image clockImageBackground;
    public uint totalSeconds; // the time to complete the task where bonus points will be awarded
    private float _elapsedTime;
    public float elapsedTime
    {
        get
        {
            return _elapsedTime;
        }
        set
        {
            _elapsedTime = value;
            UpdateClockFillSegment();
        }
    }
    private bool stopTheClock; // stop counting time elapsed when finished

    // Use this for initialization
    void Start()
    {
        // Set the scene
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        // Recording total time taken to complete. If this is not needed then add the conditional (elapsedTime < totalSeconds) to stop checking.
        if (!stopTheClock)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private void UpdateProgressBar()
    {
        float zeroToOne = (float)currentStep / (float)totalSteps;
        RectTransform bar = progressBar.GetComponent<RectTransform>();
        if (zeroToOne < 0)
        {
            zeroToOne = 0;
            Debug.LogWarning("Progress less than 0.0");
        }
        else if (zeroToOne > 1)
        {
            zeroToOne = 1;
            Debug.LogWarning("Progress greater than 1.0");
        }
        Debug.Log(string.Format("Progress {0} value: {1}", currentStep, zeroToOne));
        bar.localScale = new Vector3(zeroToOne, 1, 1);
    }

    private void UpdateClockFillSegment()
    {
        // countdown from 1.0 to 0.0
        float zeroToOne = 1 - elapsedTime / (float)totalSeconds;
        if (zeroToOne < 0)
        {
            zeroToOne = 0;
            return; //Debug.LogWarning("Clock fill segment less than 0.0");
        }
        else if (zeroToOne > 1)
        {
            zeroToOne = 1;
            return; //Debug.LogWarning("Clock fill segment greater than 1.0");
        }
        //Debug.Log(string.Format("Time {0}/{1} value: {2}", elapsedTime, totalSeconds, zeroToOne));
        clockImage.fillAmount = zeroToOne;
        // change background color depending on time remaining
        if (zeroToOne < 0.2)
        {
            clockImageBackground.GetComponent<Image>().color = Color.red;
        }
        else if (zeroToOne < 0.5)
        {
            clockImageBackground.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            clockImageBackground.GetComponent<Image>().color = Color.green;
        }
    }

    private void Reset(bool isRepeat = false)
    {
        currentStep = 0;
        elapsedTime = 0.0f;
        stopTheClock = false;

    }

    private bool Next()
    {
        if (currentStep > totalSteps)
        {
            return false;
        }

        currentStep = currentStep + 1;
        if (currentStep < totalSteps)
        {
            return true;
        }
        else if (currentStep == totalSteps)
        {
            stopTheClock = true;
        }
        return (currentStep <= totalSteps) ? true : false; // whether step should score
    }

    /// <summary>
    /// Button Handlers
    /// </summary>

    public void OnResetTapped()
    {
        Reset(true);
        SendMessageUpwards("OnReset");
    }

    public void OnReset()
    {
        Reset(true);
    }

    public void OnNext(uint attempts)
    {
        if (Next())
        {
            SendMessageUpwards("OnScoreAttempts", attempts);
        }
        // calculate final score when finished
        if (currentStep == totalSteps)
        {
            float timeRemaining = totalSeconds - elapsedTime;
            SendMessageUpwards("OnCalculateFinalScore", timeRemaining);
        }
    }

    // Note: using int arg as uint didn't appear in Unity property inspector
    public void OnNextTapped(int attempts)
    {
        OnNext((uint)attempts); // convert back to uint
    }

}
