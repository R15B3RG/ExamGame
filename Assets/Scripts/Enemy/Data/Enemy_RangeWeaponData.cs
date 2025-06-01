using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Range weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon details")]
    public Enemy_RangeWeaponType weaponType;
    public float fireRate = 1f; // Bullets per second

    public int minBulletsPerAttack = 1;
    public int maxBulletsPerAttack = 1;

    public float minWeaponCooldown = 1.5f;
    public float maxWeaponCooldown = 3f;

    [Header("Bullet details")]
    public float bulletSpeed = 20;
    public float weaponSpread = .1f;

    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack);
    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }
}
