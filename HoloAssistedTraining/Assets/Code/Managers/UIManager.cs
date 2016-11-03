using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Progress Bar")]
    public Image progressBar;

    [Header("Clock Timer")]
    public Image clockImage;
    public Image clockImageBackground;
    [HideInInspector]
    public uint totalSeconds; // the time to complete the task where bonus points will be awarded. NB this is set dynamically by the scene
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

    /// <summary>
    /// Takes a float (0.0 to 1.0) to indicate progress
    /// </summary>
    /// <param name="value"></param>
    public void UpdateProgressBar(float value)
    {
        if(value < 0)
        {
            value = 0;
        }
        if (value > 1)
        {
            value = 1;
        }
        Debug.Log(string.Format("Progress: {0}", value));
        RectTransform bar = progressBar.GetComponent<RectTransform>();
        bar.localScale = new Vector3(value, 1, 1);
    }

    private void UpdateClockFillSegment()
    {
        // countdown from 1.0 to 0.0
        float zeroToOne = 1 - elapsedTime / (float)totalSeconds;
        if (zeroToOne < 0)
        {
            zeroToOne = 0;
            return;
        }
        else if (zeroToOne > 1)
        {
            zeroToOne = 1;
            return;
        };
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
        UpdateProgressBar(0.0f);
        elapsedTime = 0.0f;
        stopTheClock = false;
    }

    /// <summary>
    /// Button Handlers
    /// </summary>

    public void OnResetTapped()
    {
        Reset(true);
        SendMessageUpwards("OnReset");
    }

    public void SetTotalSecondsAndReset(uint seconds)
    {
        Reset();
        this.totalSeconds = seconds;
    }

    public void OnReset()
    {
        Reset(true);
        SendMessageUpwards("RestartScene");
    }

    public float Complete()
    {
        UpdateProgressBar(1.0f);
        stopTheClock = true;
        return totalSeconds - elapsedTime;
    }

}
