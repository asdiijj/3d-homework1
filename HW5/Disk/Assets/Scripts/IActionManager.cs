using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionManager
{
    // ActionManager实体类需要实现此接口，用以抛出飞碟
    void StartToThrow(Queue<GameObject> diskQueue);
    int getDiskCount();
    void setDiskCount(int count);
}