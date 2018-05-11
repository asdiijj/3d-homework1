using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneController
{
    void LoadResources();
}

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
