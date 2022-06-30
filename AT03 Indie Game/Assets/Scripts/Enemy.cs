using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;




public class Enemy : FiniteStateMachine, IInteractable
{
    public Bounds bounds;
    public float viewRadius;
    public Transform player;
    public EnemyIdleState idleState;
    public EnemyWanderState wanderState;
    public EnemyChaseState chaseState;
    public EnemyStunState stunState;

    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }
    public AudioSource AudioSource { get; private set; }
    public Transform Target { get; private set; }
    public bool ForceChaseTarget { get; private set; } = false;

    protected override void Awake()
    {
        idleState = new EnemyIdleState(this, idleState);
        wanderState = new EnemyWanderState(this, wanderState);
        chaseState = new EnemyChaseState(this, chaseState);
        stunState = new EnemyStunState(this, stunState);
        entryState = idleState;
        if (TryGetComponent(out NavMeshAgent agent) == true)
        {
            Agent = agent;
        }
        if (TryGetComponent(out AudioSource aSrc) == true)
        {
            AudioSource = aSrc;
        }
        if(transform.GetChild(0).TryGetComponent(out Animator anim) == true)
        {
            Anim = anim;
        }
        TargetItem.ObjectiveActivatedEvent += TriggerForceChasePlayer;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //here we can write cusotm code to be executed after the original start definition is run
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    public void Activate()
    {
        SetState(stunState);
    }

    private void TriggerForceChasePlayer()
    {
        if(ForceChaseTarget == false)
        {
            ForceChaseTarget = true;
            SetState(chaseState);
        }
    }
}
public abstract class EnemyBehaviourState : IState
{
    protected Enemy Instance { get; private set; }


    public EnemyBehaviourState(Enemy instance)
    {
        Instance = instance;

    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();

    public abstract void OnStateUpdate();

    public virtual void DrawStateGizmos()
    {

    }
}

[System.Serializable]
public class EnemyIdleState : EnemyBehaviourState
{
    [SerializeField]
    private Vector2 idleTimeRange = new Vector2(3, 10);
    [SerializeField]
    private AudioClip idleClip;

    private float timer = -1;
    private float idleTime = 0;

    public EnemyIdleState(Enemy instance, EnemyIdleState idle) : base(instance)
    {
        idleTimeRange = idle.idleTimeRange;
        idleClip = idle.idleClip;
    }

    public override void OnStateEnter()
    {

        idleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        timer = 0;
        Instance.Anim.SetBool("isMoving", false);

        Instance.AudioSource.PlayOneShot(idleClip);

        Debug.Log("Idle state entered, waiting for " + idleTime + " seconds.");
    }

    public override void OnStateExit()
    {
        timer = -1;
        idleTime = 0;
        Debug.Log("Exiting the idle state.");
    }

    public override void OnStateUpdate()
    {
        if (Instance.ForceChaseTarget == true)
        {
            Instance.SetState(Instance.chaseState);
        }
        if (Vector3.Distance(Instance.transform.position, Instance.player.position) <= Instance.viewRadius)
        {
            Instance.SetState(Instance.chaseState);
        }
        if (timer >= 0)
        {
            timer += Time.deltaTime;
            if(timer >= idleTime)
            {
                //Debug.Log("Exiting Idle State after " + idleTime + " seconds. ");
                Instance.SetState(Instance.wanderState);
            }
        }
    }
}

[System.Serializable]
public class EnemyWanderState : EnemyBehaviourState
{

    [SerializeField]
    private float wanderSpeed = 3.5f;
    [SerializeField]
    private AudioClip wanderClip;
    [SerializeField]
    private Vector3 targetPosition;

    public EnemyWanderState(Enemy instance, EnemyWanderState wander) : base(instance)
    {
        wanderSpeed = wander.wanderSpeed;
        wanderClip = wander.wanderClip;
    }

    public override void OnStateEnter()
    {
        Instance.Agent.speed = wanderSpeed;
        Instance.Agent.isStopped = false;
        Vector3 randomPosInBounds = new Vector3
            (

            Random.Range(-Instance.bounds.extents.x, Instance.bounds.extents.x),
            Instance.transform.position.y,
            Random.Range(-Instance.bounds.extents.z, Instance.bounds.extents.z)

            ) + Instance.bounds.center;
        //while(NavMesh.SamplePosition(randomPosInBounds, out NavMeshHit hit, 1f, NavMesh.AllAreas) == false)
        //{
        //    randomPosInBounds = new Vector3
        //    (
        //    Random.Range(-Instance.bounds.extents.x, Instance.bounds.extents.x),
        //    Instance.transform.position.y,
        //    Random.Range(-Instance.bounds.extents.z, Instance.bounds.extents.z)
        //    ) + Instance.bounds.center;
        //}
        targetPosition = randomPosInBounds;
        Instance.Agent.SetDestination(targetPosition);
        Instance.Anim.SetBool("isMoving", true);
        Instance.Anim.SetBool("isChasing", false);
        Instance.AudioSource.PlayOneShot(wanderClip);
        Debug.Log("Wander state entered with a target pos of " + targetPosition);
    }

    public override void OnStateExit()
    {
        Debug.Log("Wander state exited.");
    }

    public override void OnStateUpdate()
    {
        Vector3 t = targetPosition;
        t.y = 0;
        if(Vector3.Distance(Instance.transform.position, targetPosition) <= Instance.Agent.stoppingDistance)
        {
            if (Instance.ForceChaseTarget == true)
            {
                Instance.SetState(Instance.chaseState);
            }
            else
            {
                Debug.Log("Arrived at target position");
                Instance.SetState(Instance.idleState);
            }

        }
        else
        {
            Debug.Log("moving to target pos");
        }

        if (Vector3.Distance(Instance.transform.position, Instance.player.position) <= Instance.viewRadius)
        {
            Instance.SetState(Instance.chaseState);
        }

    }

    public override void DrawStateGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
   }
    }

[System.Serializable]
public class EnemyChaseState : EnemyBehaviourState
{
    [SerializeField]
    private float chaseSpeed = 5f;
    [SerializeField]
    private AudioClip chaseClip;
    public float stunDistance;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Caught Player!");
            SceneManager.LoadScene("EndGameScene2");
        }
    }

    public EnemyChaseState(Enemy instance, EnemyChaseState chase) : base(instance)
    {
        chaseSpeed = chase.chaseSpeed;
        chaseClip = chase.chaseClip;
    }

    public override void OnStateEnter()
    {
        Instance.Agent.isStopped = false;
        Instance.Agent.speed = chaseSpeed;
        Instance.Anim.SetBool("isMoving", true);
        Instance.Anim.SetBool("isChasing", true);
        Instance.AudioSource.PlayOneShot(chaseClip);
        Debug.Log("Entered chase state");
    }

    public override void OnStateExit()
    {
        Debug.Log("Exited chase state");
    }

    public override void OnStateUpdate()
    {
        if (Vector3.Distance(Instance.transform.position, Instance.player.position) > Instance.viewRadius)
        {
            if(Instance.ForceChaseTarget == true)
            {
                Instance.Agent.SetDestination(Instance.player.position);
            }
            else
            {
                Instance.SetState(Instance.wanderState);
            }
        }
        else
        {
            Instance.Agent.SetDestination(Instance.player.position);
        }

    }
}

[System.Serializable]
public class EnemyStunState : EnemyBehaviourState
{
    [SerializeField]
    private Vector2 stunTimeRange = new Vector2(3, 10);
    [SerializeField]
    private AudioClip stunClip;

    private float timer = -1;
    private float stunTime = 0;

    public EnemyStunState(Enemy instance, EnemyStunState stun) : base(instance)
    {
        stunTimeRange = stun.stunTimeRange;
        stunClip = stun.stunClip;
    }

    public override void OnStateEnter()
    {

        stunTime = 3.5F;
        timer = 0;
        Instance.Anim.SetBool("isMoving", false);
        Instance.Anim.SetBool("isStunned", true);

        Instance.AudioSource.PlayOneShot(stunClip);

        Debug.Log("Stunned for " + stunTime + " seconds!");
    }

    public override void OnStateExit()
    {
        timer = -1;
        stunTime = 3.5F;
        Instance.Anim.SetBool("isStunned", false);
        Debug.Log("Exiting the stun state.");
    }

    public override void OnStateUpdate()
    {
        if (timer >= 0)
        {
            timer += Time.deltaTime;
            if (timer >= stunTime)
            {
                Debug.Log("Exiting stun State after " + "3.5" + " seconds. ");

                if (Instance.ForceChaseTarget == true)
                {
                    Instance.SetState(Instance.chaseState);
                }
                else
                {
                    Instance.SetState(Instance.wanderState);
                }
            }
        }
    }
}


