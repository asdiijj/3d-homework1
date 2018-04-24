using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 导演类，用于切换场景，本游戏中用处不大

public class Director : System.Object
{

    public ISceneController currnetSceneController { get; set; }

    private static Director director;

    private Director() { }

    public static Director getInstance()
    {
        if (director == null)
        {
            director = new Director();
        }
        return director;
    }
}
