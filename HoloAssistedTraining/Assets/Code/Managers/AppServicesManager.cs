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
    public string TABLE_HIGHSCORE = "HighScore";
    public string TABLE_USER = "UserList";

    public MobileServiceClient Client;
    public MobileServiceTable<UserProfile> User;
    public MobileServiceTable<HighScore> Highscore;

    void Awake()
    {
        Client = new MobileServiceClient(AppUrl);
        User = Client.GetTable<UserProfile>(TABLE_USER);
        Highscore = Client.GetTable<HighScore>(TABLE_HIGHSCORE);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
