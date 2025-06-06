using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range : Enemy
{

    [Header("Cover system")]
    public bool canUseCovers = true;
    public CoverPoint lastCover;
    public List<Cover> allCovers = new List<Cover>();

    [Header("Weapon details")]
    public Enemy_RangeWeaponType weaponType;
    public Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform weaponHolder;
    public GameObject bulletPrefab;
    public Transform gunPoint;

    [SerializeField] List<Enemy_RangeWeaponData> availableWeaponData;

    
    
    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }

    public BattleState_Range battleState { get; private set; }

    public RunToCoverState_Range runToCoverState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        visuals.SetupLook();
        SetupWeapon();

        allCovers.AddRange(CollectNearByCovers());
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    #region Cover System

    public Transform AttemptToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

        foreach (Cover cover in allCovers)
        {
            collectedCoverPoints.AddRange(cover.GetCoverPoints());
        }

        CoverPoint closestCoverPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (currentDistance < shortestDistance)
            {
                closestCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }
        }

        if (closestCoverPoint != null)
        {
            lastCover = closestCoverPoint;
        }

        return lastCover.transform;
    }

    private List<Cover> CollectNearByCovers()
    {
        float coverRaduisCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRaduisCheck);
        List<Cover> collectedCovers = new List<Cover>();

        foreach (Collider collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();

            if (cover != null && collectedCovers.Contains(cover) == false)
                collectedCovers.Add(cover);
        }

        return collectedCovers;
    }

    #endregion

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");

        Vector3 bulletsDirection = ((player.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.linearVelocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        if(canUseCovers)
            stateMachine.ChangeState(runToCoverState);
        else
            stateMachine.ChangeState(battleState);
    }

    private void SetupWeapon()
    {
        List<Enemy_RangeWeaponData> filteredData = new List<Enemy_RangeWeaponData>();

        foreach (var weaponData in availableWeaponData)
        {
            if(weaponData.weaponType == weaponType)
                filteredData.Add(weaponData);
        }

        if (filteredData.Count > 0)
        {
            int random = Random.Range(0, filteredData.Count);

            weaponData = filteredData[random];
        }

        
        gunPoint = visuals.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;

    }

    
}
