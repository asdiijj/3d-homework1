
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recorder
{
    private int score = -1;

    public void Reset()
    {
        score = -1;
    }
    
    public void Increase(int point)
    {
        score += point;
    }
    public void Decrease(int point)
    {
        score -= point;
    }
    public int GetScore()
    {
        return score;
    }
}

