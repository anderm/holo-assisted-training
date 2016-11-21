using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Code.WebApi
{
    public class WebApiHelper : Singleton<WebApiHelper>
    {
        private string speakerRecognitionUrl = "https://api.projectoxford.ai/spid/v1.0/identificationProfiles";
        private string subscriptionKey = "3c9cbc7f85c6419b9a597cf6b659838d";

        IEnumerator RequestTokenCoRoutine()
        {
            //Debug.Log("START REQUEST");
            WWWForm form = new WWWForm();
            form.AddField("grant_type", "refresh_token");
            form.AddField("refresh_token", "AQAGXcLA7ePUII37EcAkJrCnrLuRzk4yVXIZ-LTei2pLZcvvtPhiTGERJOpeRZDS0I7-dLEM6b187H5yw7YhWUKg-IPQaeamkX0qDCaeoN1Mt9pAICiCKbJs8NeOyX0N0Jg");

            UnityWebRequest www = UnityWebRequest.Post(speakerRecognitionUrl, form);
            www.SetRequestHeader("Authorization",
                "Basic MjdkZjYyZjJkYzBiNGJhM2FhNjJmMzBhZjgzNThkN2I6ZDFhZjk0MWE2NzYyNDJiZjg5OWY0MDUzMmJhNGVmM2E=");

            yield return www.Send();
            if (www.isError)
            {
                Debug.Log(www.error);
            }
        }

    }


}
