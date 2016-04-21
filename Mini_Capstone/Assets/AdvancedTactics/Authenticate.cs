using UnityEngine;
using System.Collections;

public class Authenticate : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        // Select the Google Play Games platform as our social platform implementation
        GooglePlayGames.PlayGamesPlatform.Activate();
    }

    // Update is called once per frame
    void Update()
    {

        if (!Social.localUser.authenticated)
        {
            // Authenticate
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    string token = GooglePlayGames.PlayGamesPlatform.Instance.GetToken();
                    Debug.Log(token);
                }
                else {
                    Debug.Log("Authentication failed.");
                }
            });
        }
    }
}
