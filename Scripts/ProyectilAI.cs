using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilAI : Proyectil
{
    public CannonAI CannonAgent { get; set; }


    public override void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerLifeController.Instance.HurtPlayer(0.05f);
            Explode();
        }
        
        base.OnCollisionEnter(collision);
    }

    public override void Explode()
    {
        BackToPool();
        base.Explode();
    }

    public void BackToPool()
    {
        //timeActive = 0;
        CannonAgent.DeleteShootedProjectile(this);
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        gameObject.SetActive(false);
    }
}
