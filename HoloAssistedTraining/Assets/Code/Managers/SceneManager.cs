﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using Assets.Code.Models;

public class SceneManager : Singleton<SceneManager> {

    public GameObject EngineObject;
    public GameObject ToolsKitGameObject;

    public GameObject Part1Placeholder;
    public GameObject Part2Placeholder;
    public GameObject Part3Placeholder;
    public GameObject PistonPlaceholder;

    public GameObject Part1Object;
    public GameObject Part2Object;
    public GameObject Part3Object;
    public GameObject PistonObject;

    private LinkedList<SceneMode> sceneStates;
    public LinkedListNode<SceneMode> currentSceneState;
    public string UserName { get; set; }

    public List<GameObject> CopiedPistons;

    // Use this for initialization
    void Start () {

        sceneStates = new LinkedList<SceneMode>();
        CopiedPistons = new List<GameObject>();

        var loginSceneMode = new LoginSceneMode();
        var placementSceneMode = new PlacementSceneMode(this.EngineObject, this.ToolsKitGameObject);
        var assistedSceneMode = new AssistedSceneMode(this.Part1Placeholder, this.Part2Placeholder, this.Part3Placeholder, this.PistonPlaceholder,
            this.Part1Object, this.Part2Object, this.Part3Object, this.PistonObject);

        // Add the scene to our progress list.
        sceneStates.AddFirst(loginSceneMode);
        sceneStates.AddLast(placementSceneMode);
        sceneStates.AddLast(assistedSceneMode);
        
        // Init first scene mode
        currentSceneState = sceneStates.First;
        currentSceneState.Value.InitScene();

        UIManager.Instance.SetTotalSecondsAndReset(60);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    ///  We placed an object. Check if we can advance the scene.
    /// </summary>
    public void OnPlaced()
    {
        if (currentSceneState == null)
        {
            return;
        }

        var finished = false;
        var advanced = currentSceneState.Value.CheckAndAdvanceScene(out finished);

        if (advanced && finished)
        {
            currentSceneState = currentSceneState.Next;

            if (currentSceneState != null)
            {
                currentSceneState.Value.InitScene();
            }
            else
            {
                var elapsedTime = UIManager.Instance.Complete();
                ScoreManager.Instance.OnCalculateFinalScore(elapsedTime);
            }
        }
    }

    public GameObject CurrentInteractiveObject()
    {
        if (currentSceneState != null)
        {
            return currentSceneState.Value.GetCurrentInteractiveObject();
        }

        return null;
    }

    public void RestartScene()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
