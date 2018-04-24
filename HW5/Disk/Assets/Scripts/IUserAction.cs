
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { ROUND_START, ROUND_END, RUNNING, END, START }

// GUI通往SceneController的接口
public interface IUserAction
{
    GameState getGameState();
    void setGameState(GameState gs);
    int GetScore();
    void shoot(Vector3 pos);
    void setRound(int r);
    void clearScore();
}