using UnityEngine;

public class Enemy_Bullet : Bullet
{
    protected override void OnCollisionEnter(Collision collision)
    {

        CreateImpactFx(collision);
        ReturnBulletToPool();

        Player player = collision.gameObject.GetComponentInParent<Player>();

        if (player != null)
        {
            Debug.Log("Shot the player");
        }
    }
}
