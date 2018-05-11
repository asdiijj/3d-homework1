
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, END, RESTART }

// GUI通往SceneController的接口
public interface IUserAction
{
    GameState getGameState();
    void setGameState(GameState gs);
    int GetScore();
}

public class UserGUI : MonoBehaviour
{
    private IUserAction action;

    void Start()
    {
        action = Director.getInstance().currnetSceneController as IUserAction;
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f,
            Screen.width * 0.2f, Screen.height * 0.15f), "Score: " + action.GetScore().ToString());

        if (action.getGameState() == GameState.END)
        {
            GUI.color = Color.white;
            if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f,
                Screen.width * 0.2f, Screen.height * 0.15f), "ReStart"))
            {
                action.setGameState(GameState.RESTART);
            }
        }
    }


}