using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Animator animator;

    private Rigidbody[] rbsRagdoll;
    private Collider[] collRagdoll;

    private Collider colliderPadre;
    private Rigidbody rb;
    
    [SerializeField] private float maxDistanceRay;

    private bool ragdollEnabled;

    public bool isMale=true;

    void Start()
    {
        colliderPadre = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        rbsRagdoll = transform.GetComponentsInChildren<Rigidbody>();
        collRagdoll = transform.GetComponentsInChildren<Collider>();

        foreach (Collider coll in collRagdoll)
        {
            Physics.IgnoreCollision(coll, colliderPadre);
        }
        
        SetEnabled(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * maxDistanceRay);
    }

    public void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;
        foreach (Rigidbody rigidbody in rbsRagdoll)
        {
            rigidbody.isKinematic = isKinematic;
        }

        animator.enabled = !enabled;

        rb.isKinematic = enabled;

        colliderPadre.enabled = !enabled;

        ragdollEnabled = enabled;

    }

    void Update()
    {
        if (ragdollEnabled) { return; }
        Ray ray = new Ray(transform.position, Vector3.down);
        bool onGround = Physics.Raycast(ray, out _, maxDistanceRay);
        rb.isKinematic = onGround;
    }


    [ContextMenu("Habilitar")]
    public void Habilitar()
    {
        SetEnabled(true);
    }

    [ContextMenu("Deshabilitar")]
    public void Deshabilitar()
    {
        SetEnabled(false);
    }

    public bool IsRagdollActive()
    {
        return ragdollEnabled;
    }
}