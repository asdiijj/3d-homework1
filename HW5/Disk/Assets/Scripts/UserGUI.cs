using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    bool isFirst = true;

    void Start()
    {
        action = Director.getInstance().currnetSceneController as IUserAction;
    }

    private void OnGUI()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 pos = Input.mousePosition;
            action.shoot(pos);
        }

        GUI.Label(new Rect(1000, 0, 400, 400), action.GetScore().ToString());

        // to start
        if (isFirst && GUI.Button(new Rect(700, 100, 100, 60), "Round: 1"))
        {
            isFirst = false;
            action.setGameState(GameState.ROUND_START);
            action.setRound(1);
        }
        else if (isFirst && GUI.Button(new Rect(700, 200, 100, 60), "Round: 3"))
        {
            isFirst = false;
            action.setGameState(GameState.ROUND_START);
            action.setRound(3);
        }
        else if (isFirst && GUI.Button(new Rect(700, 300, 100, 60), "Round: 5"))
        {
            isFirst = false;
            action.setGameState(GameState.ROUND_START);
            action.setRound(5);
        }

        if (!isFirst && action.getGameState() == GameState.ROUND_END && GUI.Button(new Rect(700, 100, 90, 90), "Next Round"))
        {
            action.setGameState(GameState.ROUND_START);
        }

        if (action.getGameState() == GameState.END)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(700, 300, 400, 400), "Your Score: " + action.GetScore().ToString());
            
            if (GUI.Button(new Rect(700, 100, 100, 60), "restart"))
            {
                isFirst = true;
                action.clearScore();
                action.setGameState(GameState.START);
            }
            
        }
    }


}