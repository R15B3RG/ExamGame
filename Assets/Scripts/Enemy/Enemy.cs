using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] protected int healthPoints = 20;

    [Header("Idle data")]
    public float idleTime;
    public float aggressionRange;


    [Header("Move data")]
    public float turnSpeed;
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }

    public Transform player { get; private set; }

    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine {  get; private set; }

    public Enemy_Visuals visuals { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        visuals = GetComponent<Enemy_Visuals>();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }


    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    protected bool ShouldEnterBattleMode()
    {
        bool inAggressionRange = Vector3.Distance(transform.position, player.position) < aggressionRange;

        if(inAggressionRange && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }


    #region Animation events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;

    public bool ManualMovementActive() => manualMovement;


    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;


    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();


    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    #endregion

    #region Patrol logic

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;

        if(currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for(int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);

    }

}
