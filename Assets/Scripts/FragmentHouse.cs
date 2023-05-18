using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentHouse : MonoBehaviour
{
    [SerializeField] private List<FragmentHouse> fragmentsNear = new List<FragmentHouse>();

    private Rigidbody rb;
    private Collider col;

    private ParticleSystem psDestroy;

    public bool enCasa;

    private void Start()
    {
        enCasa = true;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if (transform.childCount>0)
        {
            psDestroy = transform.GetChild(0).GetComponent<ParticleSystem>();
        }
        
    }


    public void LooseFragmentHouse(bool destroyOthers)
    {
        enCasa = false;
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
        }
        
        //tag = "Untagged";
        if (destroyOthers)
        {
            foreach (var fragment in fragmentsNear)
            {
                fragment.LooseFragmentHouse(false);
            }
            DestroyFragment();
        }
    }

    public void DestroyFragment()
    {
        if (psDestroy != null)
        {
            StartCoroutine(DestroyInstantly());
        }
    }

    IEnumerator DestroyInstantly()
    {
        //psDestroy.transform.parent = null;
        psDestroy.Play();
        yield return new WaitForSeconds(psDestroy.main.duration);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitUntil(() => psDestroy.isStopped);
        Destroy(gameObject, 3.33f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Ragdoll r))
        {
            if (r.TryGetComponent(out Ladron l))
            {
                l.Muere();
                GameManager.Instance.UpScore(5, collision.transform.position);
            }
            else
            {
                //static target
                r.transform.parent = null;
            }
            r.SetEnabled(true);

            GameManager.Instance.UpScore(10, collision.transform.position);

            LevelManager.Instance.EnemyKilled();
        }
    }
}
