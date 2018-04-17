
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 场景控制器，游戏主界面
public class SceneController : MonoBehaviour, ISceneController, IUserAction
{
    // 指定记分员
    public Recorder recorder { get; set; }

    // 动作管理器
    public CCActionManager actionManager { get; set; }

    // 工厂
    public DiskFactory factory { get; set; }

    // 用户设置回合数，每进行一回合减1
    public int round;

    // 每回合发射的飞碟数量
    private int diskCount;

    // 飞碟发射时间间隔
    private float gap;
    private float random_gap;

    // 每回合发射的飞碟列表
    public Queue<GameObject> diskQueue = new Queue<GameObject>();

    // 游戏状态
    private GameState gameState = GameState.START;

    void Awake()
    {
        Director director = Director.getInstance();
        director.currnetSceneController = this;
        director.currnetSceneController.LoadResources();

        this.gameObject.AddComponent<DiskFactory>();
        factory = Singleton<DiskFactory>.Instance;

        this.gameObject.AddComponent<Recorder>();
        recorder = Singleton<Recorder>.Instance;

        diskCount = 10;
        round = 0;
        gap = 0;
    }

    // 进入下一回合
    private void NextRound()
    {
        for (int i = 0; i < diskCount; ++i)
        {
            diskQueue.Enqueue(factory.GetDisk());
        }
        actionManager.StartToThrow(diskQueue);
        actionManager.diskCount = 10;
    }

    private void Update()
    {
        // 如果游戏进行中，而碟子为空，则本回合结束
        if (actionManager.diskCount == 0 && gameState == GameState.RUNNING)
        {
            gameState = GameState.ROUND_END;
        }

        // 新回合开始
        if (actionManager.diskCount == 0 && gameState == GameState.ROUND_START)
        {
            --round;
            if (round > -1)
            {
                NextRound();
                gameState = GameState.RUNNING;
            }
            else
            {
                gameState = GameState.END;
            }
        }

        if (gap > random_gap)
        {
            ThrowDisk();
            gap = 0;
        }
        else
        {
            gap += Time.deltaTime;
        }


    }

    public void setGameState(GameState gs)
    {
        gameState = gs;
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void setRound(int r)
    {
        round = r;
    }

    public int getRound()
    {
        return round;
    }

    void ThrowDisk()
    {
        if (diskQueue.Count != 0)
        {
            GameObject disk = diskQueue.Dequeue();

            Vector3 position = new Vector3(0, 0, 0);
            float y = UnityEngine.Random.Range(0f, 4f);
            position = new Vector3(-disk.GetComponent<DiskData>().direction.x * 7, y, 0);
            disk.transform.position = position;

            disk.SetActive(true);

            random_gap = UnityEngine.Random.Range(0, 1.5f);
        }
    }

    // 得到分数
    public int GetScore()
    {
        return recorder.score;
    }

    // 加载一些资源
    public void LoadResources()
    {

    }

    public void shoot(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.collider.gameObject.GetComponent<DiskData>() != null)
            {
                recorder.Record(hit.collider.gameObject);
                hit.collider.gameObject.transform.position = new Vector3(0, -5, 0);
            }
        }
    }

}