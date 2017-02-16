using UnityEngine;
using System.Collections;
using System;
using HoloToolkit.Unity;

public enum SceneModeType
{
    Login,
    Placement,
    Assisted,
    Exam
}

public abstract class SceneMode
{
    private int stepNumber;
    private int totalSteps = 4; // TODO: set using total number of pieces to place

    public SceneModeType SceneModeType { get; set; }

    public SceneMode(Action<bool> sceneFinishedCallback)
    {
        this.SceneFinishedCallback = sceneFinishedCallback;
    }

    public int StepNumber
    {
        get
        {
            return this.stepNumber;
        }

        set
        {
            this.stepNumber = value;
            //UIManager.Instance.currentStep++;
            float progress = (float)this.stepNumber / (float)this.totalSteps; 
            UIManager.Instance.UpdateProgressBar(progress);
        }
    }
    
    public Action<bool> SceneFinishedCallback { get; set; }

    public abstract IEnumerator CheckAndAdvanceScene();

    public abstract void InitScene();

    public abstract GameObject GetCurrentInteractiveObject();
}