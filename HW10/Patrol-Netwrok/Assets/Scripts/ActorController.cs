using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]

public class ActorController : NetworkBehaviour
{
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    private Vector3 velocity;
    private float rotateSpeed = 15f;
    private float runSpeed = 5f;

    private int score;

    [SyncVar]
    public string waitingInfo;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        score = 0;
        waitingInfo = "";
    }
    
    public override void OnStartLocalPlayer()
    {
        CmdSetActorOnServer();
    }

    [Command]
    void CmdSetActorOnServer()
    {
        ISceneController scene = Director.getInstance().currnetSceneController;
        this.name = "player" + (scene.GetPlayerNum() + 1).ToString();
        scene.AddActorObj(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // if the actor is dead
        if (!animator.GetBool("isLive"))
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        animator.SetFloat("speed", Mathf.Max(Mathf.Abs(x), Mathf.Abs(z)));
        animator.speed = 1 + animator.GetFloat("speed") / 3;

        velocity = new Vector3(x, 0, z);
        if (x != 0 || z != 0)
        {
            Quaternion quaternion = Quaternion.LookRotation(velocity);
            if (transform.rotation != quaternion)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, Time.fixedDeltaTime * rotateSpeed);
            }
        }
        this.transform.position += velocity * Time.fixedDeltaTime * runSpeed;
    }

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
            AddScore();
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

    public void AddScore()
    {
        score++;
    }

    public void ClearScore()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }
}
