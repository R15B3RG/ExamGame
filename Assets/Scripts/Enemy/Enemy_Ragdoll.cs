using UnityEngine;

public class Enemy_Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;


    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        RagdollActive(false);
    }


    public void RagdollActive(bool active)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }
    }

    public void CollidersActive(bool active)
    {
        foreach(Collider cd in ragdollColliders)
        {
            cd.enabled = active;
        }
    }
}
