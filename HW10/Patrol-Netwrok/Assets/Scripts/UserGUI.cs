
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserGUI : NetworkBehaviour
{
    //private IUserAction action;
    public ActorController actorAction;

    void Start()
    {
        // action = Director.getInstance().currnetSceneController as IUserAction;
    }

    private void OnGUI()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width * 0.6f, Screen.height * 0.1f,
            Screen.width * 0.2f, Screen.height * 0.15f), "Score: " + actorAction.GetScore().ToString());
        
        if (actorAction.waitingInfo != "" && !isServer)
        {
            if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f,
                Screen.width * 0.4f, Screen.height * 0.15f),
                "WINNER\n" + actorAction.waitingInfo + "\n\nWaiting for Restart..."))
            {
            }
        }
    }
}