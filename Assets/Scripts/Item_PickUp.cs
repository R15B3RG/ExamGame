using UnityEngine;

public class Item_PickUp : MonoBehaviour
{

    [SerializeField] private Weapon weapon;
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponController>()?.PickupWeapon(weapon);
    }
}
