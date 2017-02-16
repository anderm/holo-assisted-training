﻿using UnityEngine;
using System.Collections;
using System;
using HoloToolkit.Unity;

public class AssistedSceneMode : SceneMode
{
    public GameObject Part1Placeholder { get; set; }
    public GameObject Part2Placeholder { get; set; }
    public GameObject Part3Placeholder { get; set; }
    public GameObject PistonPlaceholder { get; set; }

    public GameObject Part1Object { get; set; }
    public GameObject Part2Object { get; set; }
    public GameObject Part3Object { get; set; }
    public GameObject PistonObject { get; set; }

    public AssistedSceneMode(GameObject part1Placeholder, GameObject part2Placeholder, GameObject part3Placeholder, GameObject pistonPlaceholder,
        GameObject part1Object, GameObject part2Object, GameObject part3Object, GameObject pistonObject, Action<bool> sceneFinishedCallback) : base(sceneFinishedCallback)
    {
        this.Part1Object = part1Object;
        this.Part1Placeholder = part1Placeholder;

        this.Part2Object = part2Object;
        this.Part2Placeholder = part2Placeholder;

        this.Part3Object = part3Object;
        this.Part3Placeholder = part3Placeholder;

        this.PistonObject = pistonObject;
        this.PistonPlaceholder = pistonPlaceholder;

        this.SceneModeType = SceneModeType.Assisted;
    }

    public override IEnumerator CheckAndAdvanceScene()
    {
        switch (this.StepNumber)
        {
            case 0:
                {
                  if(this.Part1Object.GetComponent<SnapToPosition>().isSnapped)
                    {
                        this.StepNumber++;
                        this.Part2Object.SendMessage("OnStartHighlight");
                        yield return true;
                    }
                    break;
                }
            case 1:
                {

                    if (this.Part2Object.GetComponent<SnapToPosition>().isSnapped)
                    {
                        this.StepNumber++;
                        this.Part3Object.SendMessage("OnStartHighlight");
                        yield return true;
                    }
                    break;
                }

            case 2:
                {
                    if (this.Part3Object.GetComponent<SnapToPosition>().isSnapped)
                    {
                        this.StepNumber++;
                        this.PistonObject.SendMessage("OnStartHighlight");
                        this.PistonObject.GetComponent<Rigidbody>().isKinematic = true;
                        yield return true;
                    }
                    break;
                }

            case 3:
                {
                    if (this.PistonObject.GetComponent<SnapToPosition>().isSnapped)
                    {
                        this.PistonObject.GetComponent<SnapToPosition>().Animate = true;
                        
                        var remainingTime = UIManager.Instance.Complete();
                        var finalScore = ScoreManager.Instance.OnCalculateFinalScore(remainingTime);
                        TextToSpeechManager.Instance.SpeakText("Great job, you assembled the engine! Your final score is " + finalScore +". Press reset button to try again.");

                        this.SceneFinishedCallback(true);
                        yield return true;
                    }
                    break;
                }
        }

        yield return false;
    }

    public override void InitScene()
    {
        TextToSpeechManager.Instance.SpeakText("That's it, you're ready to start assembling the engine now. Please follow the visual cues.");
        this.Part1Object.SendMessage("OnStartHighlight");

        // Start the timer
        UIManager.Instance.SetTotalSecondsAndReset(60);

        this.StepNumber = 0;
    }

    public override GameObject GetCurrentInteractiveObject()
    {
        if (this.Part1Object.GetComponent<SnapToPosition>() != null && this.Part1Object.GetComponent<SnapToPosition>().isInteracting)
            return Part1Object;

        if (this.Part2Object.GetComponent<SnapToPosition>() != null && this.Part2Object.GetComponent<SnapToPosition>().isInteracting)
            return Part2Object;

        if (this.Part3Object.GetComponent<SnapToPosition>() != null && this.Part3Object.GetComponent<SnapToPosition>().isInteracting)
            return Part3Object;

        if (this.PistonObject.GetComponent<SnapToPosition>() != null && this.PistonObject.GetComponent<SnapToPosition>().isInteracting)
            return PistonObject;

        return null;
    }
}