
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 实际使用的动作管理器
public class CCActionManager : SSActionManager, ISSActionCallback
{
    // 场景控制器
    public SceneController sceneController;

    // 飞碟的动作列表
    public List<CCAction> Fly;

    // 飞碟数量
    public int diskCount;

    private List<SSAction> used = new List<SSAction>();
    private List<SSAction> free = new List<SSAction>();

    // 获取动作
    SSAction GetSSAction()
    {
        SSAction action = null;
        if (free.Count > 0)
        {
            action = free[0];
            free.Remove(free[0]);
        }
        else
        {
            action = ScriptableObject.Instantiate<CCAction>(Fly[0]);
        }

        used.Add(action);
        return action;
    }

    // 删去已使用动作
    public void FreeSSAction(SSAction action)
    {
        SSAction temp = null;
        foreach (SSAction i in used)
        {
            if (action.GetInstanceID() == i.GetInstanceID())
            {
                temp = i;
            }
        }
        if (temp != null)
        {
            temp.reset();
            free.Add(temp);
            used.Remove(temp);
        }
    }

    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        if (source is CCAction)
        {
            diskCount--;
            DiskFactory factory = Singleton<DiskFactory>.Instance;
            factory.FreeDisk(source.gameobject);
            FreeSSAction(source);
        }
    }

    public new void Start()
    {
        sceneController = (SceneController)Director.getInstance().currnetSceneController;
        sceneController.actionManager = this;
        Fly.Add(CCAction.GetSSAction());
    }

    // 扔出一队列的飞碟
    public void StartToThrow(Queue<GameObject> diskQueue)
    {
        foreach (GameObject disk in diskQueue)
        {
            RunAction(disk, GetSSAction(), (ISSActionCallback)this);
        }
    }
}