using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game;

public class UserGUI : MonoBehaviour
{
    // 0:playing
    public int status = 0;

    private UserAction action;
    GUIStyle style;
    GUIStyle buttonStyle;

    // Use this for initialization
    void Start()
    {
        action = Director.GetInstance().scene_controller as UserAction;

        style = new GUIStyle();
        style.fontSize = 40;
        style.alignment = TextAnchor.MiddleCenter;

        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (status == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "You lose!", style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.Restart();
            }
        }
        else if (status == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "You win!", style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.Restart();
            }
        }
    }
}
