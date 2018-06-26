
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum GameState { START, END, RESTART }

// GUI通往SceneController的接口
public interface IUserAction
{
    GameState getGameState();
    void setGameState(GameState gs);
    //int GetScore();
    string GetWinnerInfo();
    void SetPlayerWaitingInfo();
    void ResetPlayerWaitingInfo();
}

public class JudgeGUI : NetworkBehaviour
{
    private IUserAction action;

    void Start()
    {
        action = Director.getInstance().currnetSceneController as IUserAction;
    }

    private void OnGUI()
    {
        GUI.color = Color.white;

        if (action.getGameState() == GameState.END)
        {
            action.SetPlayerWaitingInfo();
            GUI.color = Color.white;
            if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f,
                Screen.width * 0.2f, Screen.height * 0.15f),
                "WINNER\n" + action.GetWinnerInfo() + "\n\nReStart"))
            {
                action.setGameState(GameState.RESTART);
                action.ResetPlayerWaitingInfo();
                NetworkServer.DisconnectAll();
            }
        }
    }
}