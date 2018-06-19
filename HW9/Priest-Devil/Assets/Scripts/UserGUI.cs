using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class UserGUI : MonoBehaviour {

    private UserAction action;
    public int status = 0;
    GUIStyle style;
    GUIStyle buttonStyle;
    public NextStatus next;
    float canCount;

    void Start()
    {
        action = Director.getInstance().scene_controller as UserAction;

        style = new GUIStyle();
        style.fontSize = 40;
        style.alignment = TextAnchor.MiddleCenter;

        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;

        next = NextStatus.ON;
        canCount = 0.0f;
    }
    void OnGUI()
    {
        canCount++;
        if (status == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "You lose!", style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.restart();
            }
        }
        else if (status == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "You win!", style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.restart();
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width / 3 - 200, Screen.height / 2, 100, 50), "Next", buttonStyle))
            {
                action.setMovingObj(null);
                if (next == NextStatus.ON)
                {
                    if (canCount < 60.0f)
                    {
                        return;
                    }
                    canCount = 0;
                    action.nextOnBoat();
                    next = NextStatus.MOVE;
                }
                else if (next == NextStatus.MOVE)
                {
                    if (canCount < 75.0f)
                    {
                        return;
                    }
                    canCount = 0;
                    action.moveBoat();
                    next = NextStatus.OFF;
                }
                else if (next == NextStatus.OFF)
                {
                    if (canCount < 90.0f)
                    {
                        return;
                    }
                    canCount = 0;
                    action.nextOffBoat();
                    next = NextStatus.ON;
                }
            }
        }
    }
}
