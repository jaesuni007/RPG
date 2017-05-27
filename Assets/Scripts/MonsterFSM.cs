using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : FSMBase
{
    public float walkSpeed      = 1.5f;
    public float runSpeed       = 3.0f;
    public float turnSpeed      = 180.0f;
    public float attackRange    = 1.5f;
    public float restTime       = 1.5f;

    public int attack = 10;
    public int maxHP = 10;
    public int currentHP = 10;

    public Transform waypoint;
    [HideInInspector]
    public Transform[] waypoints;

    public PlayerFSM playerFSM;
    public Transform player;

    public Camera sight;

    public override void Awake()
    {
        base.Awake();

        agent.speed = walkSpeed;
        agent.angularSpeed = turnSpeed;
        agent.acceleration = 2000.0f;

        waypoints = waypoint.GetComponentsInChildren<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerFSM = player.GetComponent<PlayerFSM>();

        sight = GetComponentInChildren<Camera>();
    }

    public bool IsDectectPlayer()
    {
        Plane[] ps = GeometryUtility.CalculateFrustumPlanes(sight);
        // AABB : Axis Align Bounding Box
        return GeometryUtility.TestPlanesAABB(ps,playerFSM.renderer.bounds);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override IEnumerator Idle()
    {
        //Enter
        float totalTime = 0;
        agent.SetDestination(transform.position);

        while (state == CharacterState.Idle)
        {
            yield return null;
            //Stay
            totalTime += Time.deltaTime;

            if (totalTime > restTime)
            {
                SetState(CharacterState.Walk);
                break;
            }

            if (IsDectectPlayer())
            {
                SetState(CharacterState.Run);
                break;
            }
        }

        //Exit
    }

    IEnumerator Run()
    {
        //Enter
        agent.stoppingDistance = attackRange;
        agent.speed = runSpeed;

        while (state == CharacterState.Run)
        {
            yield return null;

            //Stay
            agent.SetDestination(player.position);
            if (agent.remainingDistance <= attackRange)
            {
                SetState(CharacterState.Attack);
                break;
            }
            else if (!IsDectectPlayer())
            {
                SetState(CharacterState.Idle);
                break;
            }
        }

        //Exit
    }

    IEnumerator Walk()
    {
        //Enter
        Transform target = waypoints[Random.Range(0,waypoints.Length)];
        agent.SetDestination(target.position);
        agent.speed = walkSpeed;
        agent.stoppingDistance = 0;

        while (state == CharacterState.Walk)
        {
            yield return null;

            //Stay
            if (agent.remainingDistance == 0)
            {
                SetState(CharacterState.Idle);
            }

            if (IsDectectPlayer())
            {
                SetState(CharacterState.Run);
                break;
            }
        }

        //Exit
    }

    IEnumerator Attack()
    {
        //Enter

        while (state == CharacterState.Attack)
        {
            yield return null;

            //Stay
            if(Vector3.Distance(transform.position, player.position) > attackRange)
            {
                SetState(CharacterState.Run);
                break;
            }
        }

        //Exit
    }

    IEnumerator Dead()
    {
        //Enter

        while (state == CharacterState.Dead)
        {
            yield return null;

            //Stay
        }

        //Exit
    }

    public void ProcessDamage(float damage)
    {
        currentHP -= (int)damage;

        if (currentHP <= 0)
        {
            SetState(CharacterState.Dead);
            currentHP = 0;
        }
    }
}
