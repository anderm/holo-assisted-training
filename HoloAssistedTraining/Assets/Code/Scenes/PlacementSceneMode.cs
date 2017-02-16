using System;
using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using Assets.Code.Managers;

namespace Assets.Code.Scenes
{
    public class PlacementSceneMode : SceneMode
    {
        public GameObject EngineModel { get; set; }
        public GameObject ToolsKitGameObject { get; set; }

        public PlacementSceneMode(GameObject engineModel, GameObject toolsKitGameObject, Action<bool> sceneFinishedCallback) : base(sceneFinishedCallback)
        {
            this.EngineModel = engineModel;
            this.ToolsKitGameObject = toolsKitGameObject;

            this.SceneModeType = SceneModeType.Placement;
        }

        public override IEnumerator CheckAndAdvanceScene()
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
                    this.ToolsKitGameObject.GetComponent<BoxCollider>().enabled = false;
                    this.SceneFinishedCallback(true);
                    yield return true;
                    break;
            }

            yield return false;
        }

        public override void InitScene()
        {
            TextToSpeechManager.Instance.SpeakText("Welcome " + LoginManager.Instance.FirstName + ". Please tap to place the engine you'll be working on.");
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
}
