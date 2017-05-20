using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSMBase : MonoBehaviour
{
    public CharacterState state = CharacterState.Idle;
    public Animator ani;
    public NavMeshAgent agent;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
    }

    public virtual void OnEnable()
    {
        StartCoroutine("FSMMain");
    }

    protected IEnumerator FSMMain()
    {
        while (true)
        {
            yield return StartCoroutine(state.ToString());
        }
    }

    public virtual IEnumerator Idle()
    {
        //Enter


        while (state == CharacterState.Idle)
        {
            yield return null;
            //Stay

        }

        //Exit
    }

    public void SetState(CharacterState _newState)
    {
        state = _newState;
        ani.SetInteger("state", (int)state);
    }
}
