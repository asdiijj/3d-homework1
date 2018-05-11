using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour, ISceneController, IUserAction, Observer 
{
    private ActorController controller;
    private Factory factory;
    private Recorder recorder;
    private GameState gameState = GameState.START;

    private GameObject actorObject;
    private GameObject ballObject;

    void Awake()
    {
        Director director = Director.getInstance();
        director.currnetSceneController = this;
    }

    // Use this for initialization
    void Start()
    {
        controller = new ActorController();
        factory = Singleton<Factory>.Instance;
        recorder = new Recorder();

        Subject publisher = Publisher.GetInstance();
        publisher.Add(this);

        LoadResources();
    }

    public void Update()
    {
        if (gameState == GameState.RESTART)
        {
            Reset();
        }
    }

    public void LoadResources()
    {
        Vector3 position = new Vector3(1, 0, -4);
        float[] xPosition = { -5, 5, -5, 5 };
        float[] zPosition = { 5, 5, -5, -5 };
        GameObject actor = Instantiate(Resources.Load("prefabs/actor"), position, Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject;
        actor.AddComponent<ActorController>();
        actorObject = actor.gameObject;
        for (int i = 0; i < xPosition.Length; i++)
        {
            position = new Vector3(xPosition[i], 0, zPosition[i]);
            GameObject patrol = factory.setObject(position, Quaternion.Euler(Vector3.zero));
            patrol.name = "Patrol" + (i + 1);
        }
        CreateBall();
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
    }

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

    public void Reset()
    {
        recorder.Reset();
        Destroy(actorObject);
        factory.Clear();
        LoadResources();
        if (ballObject != null)
        {
            Destroy(ballObject);
        }
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
    public int GetScore()
    {
        return recorder.GetScore();
    }
}
