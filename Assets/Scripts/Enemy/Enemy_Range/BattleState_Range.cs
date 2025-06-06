using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;
    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();

        enemy.visuals.EnableIK(true, true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.visuals.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);


        if(WeaponOutOfBullets())
        {

            if (WeaponOnCooldown())
                AttemptToResetWeapon();

            return;
        }


        if (CanShoot())
        {
            Shoot();
        }
    }

    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
    }



    private bool WeaponOnCooldown() => Time.time > lastTimeShot + weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack;

    private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate;
    

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }
}
