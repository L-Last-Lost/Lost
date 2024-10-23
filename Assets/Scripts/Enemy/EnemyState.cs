using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyState 
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    protected bool triggerCalled;
    protected float stateTimer;
    private string animBoolName;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.stateMachine = _stateMachine;
        this.enemyBase = _enemyBase;
        this.animBoolName = _animBoolName;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        // Debug.Log("??");
    }
    public virtual void Enter()
    {
        triggerCalled = false;

        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName, true); 
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.lastAnimBoolName = animBoolName;

    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
