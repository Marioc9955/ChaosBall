using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Proyectil : MonoBehaviour
{
    private int numCol = 0;

    [SerializeField] private int colisionsForExplode;

    public GameObject particleExplosion;
    [SerializeField] private float explosionForce, radioExplosion;

    private Rigidbody rb;
    private Collider col;

    public virtual void Awake()
    {
        numCol = 0;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    private void OnEnable()
    {
        numCol= 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        numCol++;

        //print(name + " colisiona con " + collision.gameObject.name);

        if (numCol >= colisionsForExplode)
        {
            Explode();
        }

    }

//    [ContextMenu("explotar")]
    public virtual void Explode()
    {
        Instantiate(particleExplosion, transform.position, Quaternion.identity);
        Collider[] colInExpl = Physics.OverlapSphere(transform.position, radioExplosion);
        foreach (Collider c in colInExpl)
        {
            Rigidbody rb = c.attachedRigidbody;
            if (rb != null && !rb.isKinematic && !c.CompareTag("Robable"))
            {
                rb.AddExplosionForce(explosionForce, transform.position, radioExplosion);
                Ragdoll rd = rb.GetComponent<Ragdoll>();
                if (rd != null)
                {
                    rd.SetEnabled(true);
                    //Debug.LogError(rd.name + " muere por explosion");
                }
            }
        }
    }

    public virtual void Fired(Vector3 force)
    {
        col.enabled = false;
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(force, ForceMode.Impulse);
        rb.useGravity = true;
        numCol = 0;
    }

    public int GetNumCol()
    {
        return numCol;
    }

}
