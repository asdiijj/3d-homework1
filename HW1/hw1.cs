using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    // member
    // 0为无，1为圆，2为叉
    private int[,] board = new int[3, 3];
    private int turn = 1;

    // Use this for initialization
    void Start () {
        Reset();
	}

    void Reset()
    {
        turn = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                board[i, j] = 0;
    }

    // 0为胜负未分，1为圆赢，2为叉赢，3为平局
    int Judge()
    {
        // 判断斜方向
        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[1, 1] != 0)
            return board[1, 1];
        else if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[1, 1] != 0)
            return board[1, 1];

        // 判断横向
        for (int i = 0; i < 3; i++)
            if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != 0)
                return board[i, 0];

        // 判断纵向
        for (int i = 0; i < 3; i++)
            if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != 0)
                return board[0, i];

        // 判断平局
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i, j] == 0)
                    return 0;

        // 胜负未分
        return 3;
    }

    void OnGUI()
    {
        // 外观设置
        GUI.skin.button.fontSize = 40;
        GUI.skin.button.fontStyle = FontStyle.Bold;
        GUIStyle font = new GUIStyle();
        font.fontSize = 25;
        font.fontStyle = FontStyle.Bold;
        font.normal.textColor = new Color(255, 255, 255);
        
        // 重置棋盘
        if (GUI.Button(new Rect(100, 400, 50, 50), "R"))
            Reset();

        // 判断胜负
        if (Judge() == 1)
        {
            GUI.Label(new Rect(120, 360, 100, 50), "O wins!", font);
        }
        else if (Judge() == 2)
        {
            GUI.Label(new Rect(120, 360, 100, 50), "X wins!", font);
        }
        else if(Judge() == 3)
        {
            GUI.Label(new Rect(120, 360, 100, 50), "Draw!", font);
        }

        // 渲染棋盘
        int length = 80;
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if (board[i, j] == 1)
                    GUI.Button(new Rect(100 + i * length, 100 + j * length, length, length), "O");
                else if (board[i, j] == 2)
                    GUI.Button(new Rect(100 + i * length, 100 + j * length, length, length), "X");
                if(GUI.Button(new Rect(100 + i * length, 100 + j * length, length, length), ""))
                {
                    if(Judge() == 0)
                    {
                        board[i, j] = turn;
                        if (turn == 1)
                            turn = 2;
                        else
                            turn = 1;
                    }
                }
            }
        }
    }

}
