### README

- 下载patrol，Assets/game 配置场景开始游戏

- [视频连接](http://v.youku.com/v_show/id_XMzU5OTUyNDM2OA==.html?spm=a2h3j.8428770.3416059.1)

- 关于此游戏

  - wasd或方向键移动；

  - 游戏场景有4个区域，每个区域有1个巡逻兵，角色进入对应区域后巡逻兵会追击角色；

  - 角色逃离巡逻兵进入下一区域后，+1分；

  - 场景会随机刷新一个白色小球，捡到后+5分，刷新下一个；

  - 巡逻兵巡逻时，随机向上下左右某个方向移动一段时间，若遇到障碍，则反方向前进。

- 以下为个人的学习过程与实验报告。

---

### 实验报告
> 具体实现方法请查看代码源文件，此处只贴主要部分。

#### 场景
设置了几个Tigger来进行判断操作，比如ATrigger是一个area，用来判断actor进入了该区域，如图：
![](/HW6/Assets/trigger.png)

#### 动画

- 以下为操纵角色actor的Animator，其中 isLive == false 且触发trigger "die" 时，进入death状态并触发死亡动画；由变量speed来控制idle（站立）或run（跑动）。
![](/HW6/Assets/actorAnimator.png)

- 以下为巡逻兵patrol的Animator，由变量speed来控制idle（站立）或run（巡逻或追击）。
![](/HW6/Assets/patrolAnimator.png)

#### 观察者模式

观察者模式的基本结构如下图所示，按照图中结构构建函数即可：
![](/HW6/Assets/publisher.png)

首先实现发布者与订阅者的基本类：
```Csharp
public enum ActorState { ENTER, DEAD, GET }

public interface Observer
{
    void Notified(ActorState state, int i, GameObject obj);
}

public class Publisher : Subject
{
    private delegate void ActionUpdate(ActorState state, int i, GameObject obj);
    private ActionUpdate updateList;

    private static Subject _instance;
    public static Subject GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Publisher();
        }
        return _instance;
    }

    public void Notify(ActorState state, int i, GameObject obj)
    {
        if (updateList != null)
        {
            updateList(state, i, obj);
        }
    }
    public void Add(Observer observer)
    {
        updateList += observer.Notified;
    }
    public void Delete(Observer observer)
    {
        updateList -= observer.Notified;
    }
}
```

可以看到，发送的信息有三种，ENTER即actor进入巡逻范围，DEAD即actor被巡逻兵捉住，GET即actor捡到加分小球；

Actor是发布信息者，3种信息都由ActorManager触发相应条件后发出，函数如下：
```Csharp
// the actor enter the area
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Area"))
        {
            Subject publisher = Publisher.GetInstance();
            publisher.Notify(ActorState.ENTER, other.gameObject.name[other.gameObject.name.Length - 2] - '0', this.gameObject);
        }
        if (other.gameObject.CompareTag("Ball"))
        {
            Subject publisher = Publisher.GetInstance();
            publisher.Notify(ActorState.GET, 0, this.gameObject);
            Destroy(other.gameObject);
        }
    }

    // the actor is caught
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Patrol") && animator.GetBool("isLive"))
        {
            animator.SetBool("isLive", false);
            animator.SetTrigger("die");

            Subject publisher = Publisher.GetInstance();
            publisher.Notify(ActorState.DEAD, 0, null);
        }
    }
```

巡逻兵patrol作为订阅者，接收ENTER信息，意味着actor进入了巡逻区域，patrol要进行追击：
```Csharp
public void Notified(ActorState state, int i, GameObject obj)
   {
       if (state == ActorState.ENTER)
       {
           if (i == this.gameObject.name[this.gameObject.name.Length - 1] - '0')
           {
               getTarget(obj);
           }
           else
           {
               loseTarget();
           }
       }
   }
```

场记SceneController也是作为订阅者，接收ENTER和GET，负责计分与其它处理；以及DEAD(else)，进行输掉游戏的处理：
```Csharp
public void Notified(ActorState state, int i, GameObject obj)
    {
        if (state == ActorState.ENTER)
        {
            recorder.Increase(1);
        }
        else if (state == ActorState.GET)
        {
            recorder.Increase(5);
            CreateBall();
        }
        else
        {
            gameState = GameState.END;
        }
    }
```

#### 运动
虽然设置了刚体，但没有使用，本游戏用运动学来设置角色移动。代码较多，请移步源代码查看。
