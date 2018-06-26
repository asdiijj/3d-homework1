### README

- 本周作业选题：将之前完成的小游戏巡逻兵Patrol做成网联版本。

- 下载项目文件夹，打开 Patrol-Network/Assets/game

- Patrol联机版：[展示视频](https://v.youku.com/v_show/id_XMzY4Nzg0MzE5Ng==.html?spm=a2h3j.8428770.3416059.1)

- 附：[Patrol旧版](http://v.youku.com/v_show/id_XMzU5OTUyNDM2OA==.html?spm=a2h3j.8428770.3416059.1)

### 关于此游戏

- 可以在SceneController设置游戏目标分数和巡逻兵（敌人）数量，其中一名玩家达到目标分数或是被巡逻兵抓住则游戏结束；

- 分数设置为0则没有目标分，结束条件为被巡逻兵抓住；

- 因为换颜色会使得Player角色不好看，因而所有玩家的Actor一致，实际玩起来时不难区分；

- 游戏结束后，由服务端玩家负责按Reset，会解除所有客户端的连接，需要由服务端重开；

---

### 实现细节

#### 场景设置相关

- 下面为场景game的结构，其中NetworkManager添加Manager和HUD两个组件；Home为Player生成的位置（有四个），加入了StartPosition组件；SceneController为服务器端负责执行的代码脚本；其余的为场景构建：

![](/HW10/Assets/1.png)

- 下面是SceneController的设置，有对应的脚本，并且Identity设置为"Server Only"，JudgeGUI脚本负责游戏胜负的判断：

![](/HW10/Assets/2.png)

- 与旧版的Patrol的SceneController相比，联网版的要将MonoBehavior改为NetworkBehaviour，随后删去Factory的相关代码，不再使用，直接在OnStartServer函数中根据设置的NumPatrols生成巡逻兵：

```CSharp
// SceneController.cs
public override void OnStartServer()
{
    // create Enemy Patrol
    float[] xPosition = { -5, 5, -5, 5 };
    float[] zPosition = { 5, 5, -5, -5 };
    if (NumPatrols > 4)
    {
        NumPatrols = 4;
    }
    for (int i = 0; i < NumPatrols; ++i)
    {
        var position = new Vector3(xPosition[i], 0, zPosition[i]);
        var enemy = (GameObject)Instantiate(PatrolPrefab, position, Quaternion.Euler(Vector3.zero));
        enemy.name = "Patrol" + (patrolObjects.Count + 1).ToString();
        patrolObjects.Add(enemy);
        NetworkServer.Spawn(enemy);
    }
}
```

- 除了代码的修改之外，还要在NetworkManager之中设置 Spawnable Prefabs，才能使所有玩家里的巡逻兵同步，如下图所示（另外，得分用的ball也要同样的设置）：

![](/HW10/Assets/3.png)

#### 预制的设置

- 接下来要实现的就是最重要的玩家角色Actor预制了（以及巡逻兵Patrol）。如下图所示，首先加入组件NetworkIdentity并设置为Local；ActorController控制Actor的移动；加入组件NetworkTransform，为了使其动作流畅，设置Rate为29，并且Interpolate Rotation为360，如果不这样设置，玩家会看到其它客户端玩家的角色转身时很卡；最后为了同步动画，加入组件NetworkAnimator并将Actor的Animator组件拖进去：

![](/HW10/Assets/4.png)

- 以上就是Actor预制的设置，它还有一个脚本UserGUI用来显示每个玩家的分数，此脚本在本地执行，并且当游戏结束时显示Winner信息：

```CSharp
// UserGUI.cs
private void OnGUI()
{
    if (!isLocalPlayer)
    {
        return;
    }
    GUI.color = Color.white;
    GUI.Label(new Rect(Screen.width * 0.6f, Screen.height * 0.1f,
        Screen.width * 0.2f, Screen.height * 0.15f), "Score: " + actorAction.GetScore().ToString());

    if (actorAction.waitingInfo != "" && !isServer)
    {
        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f,
            Screen.width * 0.4f, Screen.height * 0.15f),
            "WINNER\n" + actorAction.waitingInfo + "\n\nWaiting for Restart..."))
        {
        }
    }
}
```

- 另外，为了让在服务端执行的脚本SceneController能够监控所有玩家的分数，Player在产生时，需要在服务端执行一段代码，将GameObject传进SceneController的数组中：

```CSharp
// ActorController.cs
[Command]
void CmdSetActorOnServer()
{
    ISceneController scene = Director.getInstance().currnetSceneController;
    this.name = "player" + (scene.GetPlayerNum() + 1).ToString();
    scene.AddActorObj(this.gameObject);
}
```

- Patrol预制也是同样的设置，即加入了NetworkIdentity,NetworkAnimator,NetworkTransform组件，这里不再赘述。实现了几个Prefab之后，代码基本就完成了。

- 另外，分数的记录代码加入到ActorController.cs中，为了使得每个玩家的分数相互独立。还有其它关于游戏结束的判断、winner信息的传输以及其它相关的代码细节，请下载项目文件进行查阅，这里不再赘述。
