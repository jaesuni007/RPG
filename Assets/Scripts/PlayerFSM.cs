using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFSM : FSMBase
{
    public GameObject movePoint;
    public GameObject attackPoint;

    public float moveSpeed = 4.0f;
    public float turnSpeed = 360.0f;

    public LayerMask layerMask;
    public float attackRange    = 1.5f;
    public float attack         = 100.0f;
    public int maxHP            = 100;
    public int currentHP        = 100;

    public Renderer renderer;

    public MonsterFSM monsterFSM;

    public override void Awake()
    {
        base.Awake();

        movePoint = GameObject.Find("MovePoint");
        movePoint.SetActive(false);
        attackPoint = GameObject.Find("AttackPoint");
        attackPoint.SetActive(false);
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        agent.acceleration = 2000.0f;

        renderer = GetComponentInChildren<Renderer>();

        layerMask = LayerMask.GetMask("Click", "Block", "Monster");
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 100.0f, layerMask))
            {
                int layer = hitInfo.collider.gameObject.layer;

                if (layer == LayerMask.NameToLayer("Click"))
                {
                    movePoint.transform.position = hitInfo.point;
                    movePoint.SetActive(true);
                    attackPoint.SetActive(false);
                    agent.SetDestination(movePoint.transform.position);
                    SetState(CharacterState.Run);
                    agent.stoppingDistance = 0;
                }
                else if(layer == LayerMask.NameToLayer("Monster"))
                {
                    attackPoint.transform.SetParent(hitInfo.transform);
                    attackPoint.transform.localPosition = Vector3.zero;
                    attackPoint.SetActive(true);
                    movePoint.SetActive(false);
                    agent.SetDestination(attackPoint.transform.position);
                    SetState(CharacterState.AttackRun);
                    agent.stoppingDistance = attackRange;
                    monsterFSM = hitInfo.transform.GetComponent<MonsterFSM>();
                }
            }
        }
    }

    public override IEnumerator Idle()
    {
        //Enter


        while (state == CharacterState.Idle)
        {
            yield return null;
            //Stay

        }

        //Exit
    }

    IEnumerator Run()
    {
        //Enter


        while (state == CharacterState.Run)
        {
            yield return null;
            //Stay
            if (agent.remainingDistance <= 0.4)
            {
                SetState(CharacterState.Idle);
                movePoint.SetActive(false);
            }
        }

        //Exit

    }

    IEnumerator AttackRun()
    {
        //Enter


        while (state == CharacterState.AttackRun)
        {
            yield return null;
            //Stay
            if (state == CharacterState.AttackRun)
            {
                agent.SetDestination(attackPoint.transform.position);
            }

            if (agent.remainingDistance <= attackRange)
            {
                SetState(CharacterState.Attack);
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
            if (monsterFSM.IsDead() && RemainTime(0.7))
            {
                attackPoint.SetActive(false);
                SetState(CharacterState.Idle);
                break;
            }
        }

        //Exit
    }

    public void OnPlayerAttack()
    {
        monsterFSM.ProcessDamage(attack);
    }
}
