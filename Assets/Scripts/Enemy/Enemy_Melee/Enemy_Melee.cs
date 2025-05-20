using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;

    [UnityEngine.Range(1, 2)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }

public class Enemy_Melee : Enemy
{

    public Enemy_Visuals visuals {  get; private set; }

    #region States
    public IdleState_Melee idleState { get; private set; }

    public MoveState_Melee moveState { get; private set; }

    public RecoveryState_Melee recoveryState { get; private set; }

    public ChaseState_Melee chaseState { get; private set; }

    public AttackState_Melee attackState { get; private set; }

    public DeadState_Melee deadState { get; private set; }

    public AbilityState_Melee abilityState { get; private set; }

    #endregion

    [Header("Enemy settings")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge = -10;


    [Header("Axe throw ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    public Transform axeStartPoint;
    private float lastTimeAxeThrown;


    [Header("Attack data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    protected override void Awake()
    {
        base.Awake();

        visuals = GetComponent<Enemy_Visuals>();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); // Idle anim is just a placeholder. We use ragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        InitializeSpeciality();

        visuals.SetupLook();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        if(ShouldEnterBattleMode())
            EnterBattleMode();
        
    }

    public override void EnterBattleMode()
    {

        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        moveSpeed = moveSpeed * .6f;
        EnableWeaponModel(false);
    }

    private void InitializeSpeciality()
    {
        if(meleeType == EnemyMelee_Type.AxeThrow)
        {
            visuals.SetupWeaponType(Enemy_MeleeWeaponType.Throw);
        }


        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            visuals.SetupWeaponType(Enemy_MeleeWeaponType.OneHand);
        }

        if(meleeType == EnemyMelee_Type.Dodge)
        {
            visuals.SetupWeaponType(Enemy_MeleeWeaponType.Unarmed);
        }
    }

    public override void GetHit()
    {
        base.GetHit();

        if(healthPoints <= 0)
            stateMachine.ChangeState(deadState);
    }

    public void EnableWeaponModel(bool active)
    {
        visuals.currentWeaponModel.gameObject.SetActive(active);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;


    public void ActivateDodgeRoll()
    {

        if (meleeType != EnemyMelee_Type.Dodge)
            return;

        if (stateMachine.currentState != chaseState)
            return;

        if (Vector3.Distance(transform.position, player.position) < 1.5f)
            return;

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge Roll");

        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            lastTimeDodge = Time.time;

            anim.SetTrigger("Dodge");
        }
        
    }


    public bool CanThrowAxe()
    {

        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > lastTimeAxeThrown + axeThrowCooldown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }

        return false;
    }


    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if(clip.name == clipName)
                return clip.length;
        }

        Debug.Log(clipName + " animation not found!");
        return 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
