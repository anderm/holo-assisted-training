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

    public abstract IEnumerator CheckAndAdvanceScene(Action<bool> callback = null);

    public abstract void InitScene();

    public abstract GameObject GetCurrentInteractiveObject();
}

public class PlacementSceneMode : SceneMode
{
    public GameObject EngineModel { get; set; }
    public GameObject ToolsKitGameObject { get; set; }

    public PlacementSceneMode(GameObject engineModel, GameObject toolsKitGameObject)
    {
        this.EngineModel = engineModel;
        this.ToolsKitGameObject = toolsKitGameObject;

        this.SceneModeType = SceneModeType.Placement;
    }

    public override IEnumerator CheckAndAdvanceScene(Action<bool> callback = null)
    {
        switch (this.StepNumber)
        {
            case 0:
                TextToSpeechManager.Instance.SpeakText("Good! Now tap again to place the engine.");
                this.EngineModel.SetActive(true);
                this.EngineModel.transform.eulerAngles = new Vector3(0, 90f, 0);
                this.EngineModel.SendMessage("OnSelect");
                
                this.StepNumber++;
                yield return true;

                break;
            case 1:
                TextToSpeechManager.Instance.SpeakText("Good job. Now tap anywhere to pick up your toolbox.");
                this.StepNumber++;

                yield return true;
                break;

            case 2:
                TextToSpeechManager.Instance.SpeakText("Nice. You're ready to place your toolbox now. Tap again wherever you'd like.");
                this.StepNumber++;

                this.ToolsKitGameObject.SetActive(true);
                this.ToolsKitGameObject.SendMessage("OnSelect");

                yield return true;
                break;
            case 3:
                this.StepNumber++;
                this.ToolsKitGameObject.GetComponent<BoxCollider>().enabled = false;
                callback(true);
                yield return true;
                break;
        }

        yield return false;
    }

    public override void InitScene()
    {
        TextToSpeechManager.Instance.SpeakText("Welcome " + SceneManager.Instance.FirstName + ". Please tap to place the engine you'll be working on.");
        this.EngineModel.SetActive(false);
        this.ToolsKitGameObject.SetActive(false);
        this.StepNumber = 0;
    }

    public override GameObject GetCurrentInteractiveObject()
    {
        // No need for this method in placement mode.
        return null;
    }
}