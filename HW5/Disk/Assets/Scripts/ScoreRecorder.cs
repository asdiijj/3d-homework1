
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 记分员
public class Recorder : MonoBehaviour
{
    public int score;

    private void Start()
    {
        score = 0;
    }

    public void Record(GameObject disk)
    {
        if (disk.GetComponent<DiskData>().color == Color.blue)
            score += 6;
        else if (disk.GetComponent<DiskData>().color == Color.red)
            score += 10;
        else
            score += 15;
    }

    public void Reset()
    {
        score = 0;
    }
}