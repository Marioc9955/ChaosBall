using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilPlayer : Proyectil
{
    public CannonPlayerAI cannonPlayerAI { get; set; }

    public override void Awake()
    {
        cannonPlayerAI = null;
        base.Awake();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ragdoll"))
        {
            if (!collision.gameObject.TryGetComponent<Ragdoll>(out var r))
            {
                r = collision.gameObject.GetComponentInParent<Ragdoll>();
            }
            if (r != null && !r.IsRagdollActive())
            {
                if (r.TryGetComponent(out Ladron l))
                {
                    l.Muere();
                    GameManager.Instance.UpScore(5, transform.position);
                }
                else
                {
                    //static target
                    r.transform.parent = null;
                }

                if (r.isMale)
                {
                    SoundManager.Instance.PlayOuchMaleSFX(r.transform.position);
                }
                else
                {
                    SoundManager.Instance.PlayOuchFemaleSFX(r.transform.position);
                }
                r.SetEnabled(true);

                GameManager.Instance.UpScore(10, transform.position);

                LevelManager.Instance.EnemyKilled();
            }
            if (cannonPlayerAI != null)
            {
                cannonPlayerAI.CambiarObjetivo();
            }
        }

        if (collision.gameObject.CompareTag("Destruible"))
        {
            if (collision.gameObject.TryGetComponent(out FragmentHouse fd))
            {
                if (fd.enCasa)
                {
                    fd.LooseFragmentHouse(true);
                }
                else
                {
                    fd.DestroyFragment();
                }
                GameManager.Instance.UpScore(5, transform.position);
            }
            else
            {
                GameManager.Instance.UpScore(1, transform.position);
            }
            //GameManager.Instance.score += 5;

            Explode();
        }

        if (collision.gameObject.CompareTag("EnemyAI"))
        {
            GameManager.Instance.UpScore(2, transform.position);
            CannonAIManager.Instance.HurtCannonAI();
            Explode();
        }

        base.OnCollisionEnter(collision);
    }

    public override void Fired(Vector3 force)
    {
        transform.parent = null;
        transform.localScale = Vector3.one;

        base.Fired(force);

        if (CameraManager.Instance.IsFollowCamActive())
        {
            CameraManager.Instance.ActivateCamFollowProyectil(transform);
        }

    }

    public override void Explode()
    {
        if (cannonPlayerAI != null)
        {
            cannonPlayerAI.DeleteShootedProjectile(this);
        }
        base.Explode();
        CameraManager.Instance.BackToCannon();
        Destroy(gameObject);
    }
}
