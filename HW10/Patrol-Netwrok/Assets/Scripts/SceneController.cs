using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SceneController : NetworkBehaviour, ISceneController, IUserAction, Observer
{
    //private ActorController controller;
    //private Factory factory;
    //private Recorder recorder;
    private GameState gameState = GameState.START;

    //private GameObject actorObject;
    private GameObject ballObject;

    private List<GameObject> actorObjects = new List<GameObject>();
    private List<GameObject> patrolObjects = new List<GameObject>();
    
    private string winnerInfo;

    public GameObject PatrolPrefab;

    public int NumPatrols;

    public int targetPoint;

    void Awake()
    {
        Director director = Director.getInstance();
        director.currnetSceneController = this;
    }

    // Use this for initialization
    void Start()
    {
        //controller = new ActorController();
        //factory = Singleton<Factory>.Instance;
        //recorder = new Recorder();

        Subject publisher = Publisher.GetInstance();
        publisher.Add(this);

        LoadResources();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        CreateBall();
        winnerInfo = "";

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

    public void Update()
    {
        if (gameState != GameState.END && targetPoint > 0)
        {
            for (int i = 0; i < actorObjects.Count; ++i)
            {
                int score = actorObjects[i].GetComponent<ActorController>().GetScore();
                if (score == targetPoint)
                {
                    winnerInfo = actorObjects[i].name + " : " + score.ToString();
                    gameState = GameState.END;
                    break;
                }
            }
        }
        if (gameState == GameState.RESTART)
        {
            Reset();
        }
    }

    public void SetPlayerWaitingInfo()
    {
        for (int i = 0; i < actorObjects.Count; ++i)
        {
            if (actorObjects[i].GetComponent<ActorController>().waitingInfo != winnerInfo)
            {
                actorObjects[i].GetComponent<ActorController>().waitingInfo = winnerInfo;
            }
        }
    }
    public void ResetPlayerWaitingInfo()
    {
        for (int i = 0; i < actorObjects.Count; ++i)
        {
            actorObjects[i].GetComponent<ActorController>().waitingInfo = "";
        }
    }
    public void ResetPlayerScore()
    {
        for (int i = 0; i < actorObjects.Count; ++i)
        {
            actorObjects[i].GetComponent<ActorController>().ClearScore();
        }
    }

    public void AddActorObj(GameObject obj)
    {
        actorObjects.Add(obj);
    }

    public int GetPlayerNum()
    {
        return actorObjects.Count;
    }

    public string GetWinnerInfo()
    {
        return winnerInfo;
    }

    public void LoadResources()
    {
        /*
        int EnemyNum = 1;
        Vector3 position = new Vector3(1, 0, -4);
        float[] xPosition = { -5, 5, -5, 5 };
        float[] zPosition = { 5, 5, -5, -5 };
        for (int i = 0; i < EnemyNum; i++)
        {
            position = new Vector3(xPosition[i], 0, zPosition[i]);
            GameObject patrol = factory.setObject(position, Quaternion.Euler(Vector3.zero));
            patrol.name = "Patrol" + (i + 1);
        }
        */
        //CreateBall();
    }

    private void CreateBall()
    {
        float x = 0;
        float z = 0;
        while (x > -2f && x < 2f)
        {
            x = Random.Range(-9.125f, 9.125f);
        }
        while (z > -2f && z < 2f)
        {
            z = Random.Range(-9.125f, 9.125f);
        }
        Vector3 position = new Vector3(x, 0.125f, z);
        GameObject ball = Instantiate(Resources.Load("prefabs/ball"), position, Quaternion.Euler(Vector3.zero)) as GameObject;
        ballObject = ball;
        NetworkServer.Spawn(ballObject);
    }

    public void Notified(ActorState state, int i, GameObject obj)
    {
        if (state == ActorState.ENTER)
        {
            //recorder.Increase(1);
        }
        else if (state == ActorState.GET)
        {
            //recorder.Increase(5);
            CreateBall();
        }
        else
        {
            gameState = GameState.END;
        }
    }

    public void Reset()
    {
        //recorder.Reset();
        //Destroy(actorObject);
        //factory.Clear();
        LoadResources();
        if (ballObject != null)
        {
            Destroy(ballObject);
        }
        patrolObjects.Clear();
        gameState = GameState.START;
    }

    public GameState getGameState()
    {
        return gameState;
    }
    public void setGameState(GameState gs)
    {
        gameState = gs;
    }
}
