### README

- 下载Disk，Assets/a1 配置场景开始游戏

- [视频连接](http://v.youku.com/v_show/id_XMzU2MTM3Mzc3Ng==.html?spm=a2h3j.8428770.3416059.1)

- 关于此游戏

  - 修改了上一版本重开游戏不重置分数的BUG

  - disk有三种颜色、大小、分数、速度：
    - 蓝-大-低分-慢-3倍出现概率
    - 红-中-中等-中速-2倍出现概率
    - 黑-小-高分-快-1倍出现概率  

  - 物理运动下设置了飞碟会碰撞，会更加有趣

  - 可以在MainCamare设置运动模式脚本；若两个都不选则不会有飞碟射出，若两个都选则默认运动学模式（CCAction）。

- 以下为个人的学习过程与笔记，供参考。

---

### 物理引擎 - 刚体（Rigidbody）
> 给游戏对象加上组件Rigidbody，可以赋予对象物理引擎，不再需要使用公式来描述运动轨迹。

- 加入组件：
```java
disk.AddComponent<Rigidbody>();
```

- 取消物理引擎，只需要设置运动学就可以：
```java
gameobject.GetComponent<Rigidbody>().isKinematic = true;
```

- 本游戏中要射出飞碟，如下设置即可：
```java
rigidbody.velocity = speed * direction;
```

- 另外，如果需要在Update()中修改rigidbody，则需要改用FixedUpdate()。（注：本程序中不需要用到）

---

### 适配器模式（Adapter Pattern）
> 适配器模式（Adapter Pattern）是作为两个不兼容的接口之间的桥梁。这种类型的设计模式属于结构型模式，它结合了两个独立接口的功能。

具体实现：

- 本次作业要求增加一个物理运动，并要求可以切换。而一个动作的实现与ActionManager有关，因此首先声明一个ActionManager的接口类：
```java
// IActionManager.cs
public interface IActionManager
{
    // ActionManager实体类需要实现此接口，用以抛出飞碟
    void StartToThrow(Queue<GameObject> diskQueue);
    int getDiskCount();
    void setDiskCount(int count);
}
```

- 没有声明Action的接口，因为一个Action只有对应于它的ActionManager会使用，不需要实现一个共同接口，只需要继承与SSAction即可。

- 为了实现新增的物理运动，需要一些代码的改变，比如DiskFactory新增：
```java
// 在加工Disk的时候，放入刚体组件
if (disk.GetComponent<Rigidbody>() == null)
{
    disk.AddComponent<Rigidbody>();
}
```

- 比如SceneController新增actionMode来记录当前使用的运动模式：
```java
// 动作模式
public ActionMode actionMode { get; set; }
// Awake() 新增
void Awake() { actionMode = ActionMode.NONE; }
// Update() 新增
private void Update() {
    // 若未设置运动模式
    if (actionMode == ActionMode.NONE || actionManager == null) {
        return;
    }
}
// ActionMode 枚举
public enum ActionMode { NONE, KINEMATIC, PHYSICS}
```

- CCAction里要将新加入的刚体设为运动学
```java
gameobject.GetComponent<Rigidbody>().isKinematic = true;
```

- PhysicsActionManager类的实现与CCActionManager类的基本一样，只是将其中CCAction改为PhysicsAction而已。并且新增在Start()里设置场记的动作模式：
```java
public new void Start() {
    sceneController.actionMode = ActionMode.KINEMATIC;
    // or ActionMode.PHYSICS
}
```

- 最后就是最主要的PhysicsAction.cs的实现了：
```java
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 实际使用的飞行动作 - 物理运动版本
public class PhysicsAction : SSAction
{
    // 水平飞行速度
    float speed;

    // 初始飞行方向
    Vector3 direction;

    // 此版本新增成员，刚体
    Rigidbody rigidbody;

    public override void Start()
    {
        enable = true;
        speed = gameobject.GetComponent<DiskData>().speed;
        direction = gameobject.GetComponent<DiskData>().direction;

        rigidbody = gameobject.GetComponent<Rigidbody>();
        if (rigidbody)
        {
            // 设置刚体
            rigidbody.velocity = speed * direction;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    // 不需要改变rigidbody的值，无需使用FixedUpdate
    public override void Update()
    {
        // 因为设置了刚体，不需要在这里实现运动轨迹
        if (gameobject.activeSelf)
        {
            // 飞碟落地
            if (transform.position.y < -4)
            {
                destroy = true;
                enable = false;
                callback.SSActionEvent(this);
            }
        }
    }

    public static PhysicsAction GetSSAction()
    {
        PhysicsAction action = ScriptableObject.CreateInstance<PhysicsAction>();
        return action;
    }
}
```
