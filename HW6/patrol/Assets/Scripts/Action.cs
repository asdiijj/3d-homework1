using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SSActionEventType : int { Started, Competeted }

public interface ISSActionCallback
{
    void SSEventAction(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objParam = null);
}

public class SSAction : ScriptableObject
{
    public bool enable = true;
    public bool destroy = false;

    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }

    protected SSAction() { }

    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
    
    public void reset()
    {
        enable = false;
        destroy = false;
        gameobject = null;
        transform = null;
        callback = null;
    }
}

public class SSActionManager : MonoBehaviour
{
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();

    // Use this for initialization
    protected void Start()
    {

    }

    protected void Update()
    {
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }
}

public class IdleAction : SSAction
{
    private float idleTime;
    private Animator animator;

    public static IdleAction GetIdleAction(float time, Animator ani)
    {
        IdleAction currentAction = ScriptableObject.CreateInstance<IdleAction>();
        currentAction.idleTime = time;
        currentAction.animator = ani;
        return currentAction;
    }

    public override void Start()
    {
        animator.SetFloat("speed", 0);
    }

    public override void Update()
    {
        if (idleTime == -1)
        {
            return;
        }
        idleTime -= Time.deltaTime;
        if (idleTime < 0)
        {
            this.destroy = true;
            this.callback.SSEventAction(this);
        }
    }

}

public class WalkAction : SSAction
{
    private float speed;
    private Vector3 destination;
    private Animator animator;

    public static WalkAction GetWalkAction(float speed, Vector3 des, Animator ani)
    {
        WalkAction currentAction = ScriptableObject.CreateInstance<WalkAction>();
        currentAction.speed = speed;
        currentAction.destination = des;
        currentAction.animator = ani;
        return currentAction;
    }

    public override void Start()
    {
        animator.SetFloat("speed", 0.5f);
    }

    public override void Update()
    {
        Quaternion quaternion = Quaternion.LookRotation(destination - transform.position);
        if (transform.rotation != quaternion)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, speed * Time.deltaTime * 5);
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, destination, speed * Time.deltaTime);
        if (this.transform.position == destination)
        {
            this.destroy = true;
            this.callback.SSEventAction(this);
        }
    }
}

public class CatchAction : SSAction
{
    private float speed;
    private Transform target;
    private Animator animator;

    public static CatchAction GetCatchAction(float speed, Transform target, Animator ani)
    {
        CatchAction currentAction = ScriptableObject.CreateInstance<CatchAction>();
        currentAction.speed = speed;
        currentAction.target = target;
        currentAction.animator = ani;
        return currentAction;
    }

    public override void Start()
    {
        animator.SetFloat("speed", 1f);
    }

    public override void Update()
    {
        Quaternion quaternion = Quaternion.LookRotation(target.position - transform.position);
        if (transform.rotation != quaternion)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, speed * Time.deltaTime * 5);
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
        if (Vector3.Distance(this.transform.position, target.position) < 0.5f)
        {
            this.destroy = true;
            this.callback.SSEventAction(this);
        }
    }
}
