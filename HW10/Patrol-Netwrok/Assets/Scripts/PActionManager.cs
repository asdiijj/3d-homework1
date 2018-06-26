using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]

/*
 * the ActionManager of Patrol
 */

public class PActionManager : SSActionManager, ISSActionCallback, Observer
{
    public enum ActionState : int { IDLE, WALKLEFT, WALKFORWARD, WALKRIGHT, WALKBACK }

    private Animator animator;
    private SSAction action;
    private ActionState actionState;
    private const float walkSpeed = 1f;
    private const float runSpeed = 3f;

    // Use this for initialization
    new void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        Subject publisher = Publisher.GetInstance();
        publisher.Add(this);
        actionState = ActionState.IDLE;
        idle();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public void SSEventAction(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objParam = null)
    {
        actionState = actionState> ActionState.WALKBACK ? ActionState.IDLE : (ActionState)((int)actionState + 1);
        // change the current state
        switch (actionState)
        {
            case ActionState.WALKLEFT:
                walkLeft();
                break;
            case ActionState.WALKRIGHT:
                walkRight();
                break;
            case ActionState.WALKFORWARD:
                walkForward();
                break;
            case ActionState.WALKBACK:
                walkBack();
                break;
            default:
                idle();
                break;
        }
    }

    public void idle()
    {
        action = IdleAction.GetIdleAction(Random.Range(1, 1.5f), animator);
        this.RunAction(this.gameObject, action, this);
    }
    public void walkLeft()
    {
        Vector3 target = Vector3.left * Random.Range(3, 5) + this.transform.position;
        action = WalkAction.GetWalkAction(walkSpeed, target, animator);
        this.RunAction(this.gameObject, action, this);
    }
    public void walkRight()
    {
        Vector3 target = Vector3.right * Random.Range(3, 5) + this.transform.position;
        action = WalkAction.GetWalkAction(walkSpeed, target, animator);
        this.RunAction(this.gameObject, action, this);
    }
    public void walkForward()
    {
        Vector3 target = Vector3.forward * Random.Range(3, 5) + this.transform.position;
        action = WalkAction.GetWalkAction(walkSpeed, target, animator);
        this.RunAction(this.gameObject, action, this);
    }
    public void walkBack()
    {
        Vector3 target = Vector3.back * Random.Range(3, 5) + this.transform.position;
        action = WalkAction.GetWalkAction(walkSpeed, target, animator);
        this.RunAction(this.gameObject, action, this);
    }

    public void turn()
    {
        action.destroy = true;
        switch (actionState)
        {
            case ActionState.WALKLEFT:
                actionState = ActionState.WALKRIGHT;
                walkRight();
                break;
            case ActionState.WALKRIGHT:
                actionState = ActionState.WALKLEFT;
                walkLeft();
                break;
            case ActionState.WALKFORWARD:
                actionState = ActionState.WALKBACK;
                walkBack();
                break;
            case ActionState.WALKBACK:
                actionState = ActionState.WALKFORWARD;
                walkForward();
                break;
        }
    }

    public void getTarget(GameObject target)
    {
        action.destroy = true;
        action = CatchAction.GetCatchAction(runSpeed, target.transform, animator);
        this.RunAction(this.gameObject, action, this);
    }
    public void loseTarget()
    {
        action.destroy = true;
        idle();
    }

    public void stop()
    {
        action.destroy = true;
        action = IdleAction.GetIdleAction(-1f, animator);
        this.RunAction(this.gameObject, action, this);
    }

    // the actor run out of the patrol-area
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            turn();
        }
    }
    // the patrol crash the wall
    private void OnCollisionEnter(Collision collision)
    {
        Transform parent = collision.gameObject.transform.parent;
        if (parent != null && parent.CompareTag("Wall"))
        {
            turn();
        }
    }
    
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
        else if (state == ActorState.GET)
        {

        }
        else
        {
            stop();
        }
    }
}
