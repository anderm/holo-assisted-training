using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity3dAzure.AppServices;
using UnityEngine;
using Assets.Code.Models;

public class AppServicesManager : Singleton<AppServicesManager>
{
    /// <summary>
    /// App Services config
    /// </summary>
    public string AppUrl;
    public string HighscoreTable = "HighScore";
    public string UserProfilesTable = "UserList";

    public MobileServiceClient Client;
    public MobileServiceTable<UserProfile> User;
    public MobileServiceTable<HighScore> Highscore;

    void Awake()
    {
        Client = new MobileServiceClient(AppUrl);
        User = Client.GetTable<UserProfile>(UserProfilesTable);
        Highscore = Client.GetTable<HighScore>(HighscoreTable);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
