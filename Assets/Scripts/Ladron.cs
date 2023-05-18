using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ladron : MonoBehaviour
{
    private GameObject objetivo;
    public GameObject Objetivo { get => objetivo;set=>objetivo= value; }

    //public GameObject objetivo;
    private MeshCollider objetivoMeshCollider;
    private Collider objetivoTriggerCollider;
    private Rigidbody objetivoRigidbody;
    [HideInInspector] public Transform puntoRetorno;

    NavMeshAgent navMeshAgent;
    Animator anim;

    private int enObjetivoID;

    [SerializeField] private Transform mano;

    private SpawnEnemies spawnEnemies;

    public SpawnEnemies SpawnEnemies { get => spawnEnemies; set => spawnEnemies = value; }
    public int EnObjetivoID { get => enObjetivoID; set => enObjetivoID = value; }

    private bool muerto = false;

    private bool objetivoRobado;

    public float EnemiesSpeed { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        objetivoRobado = false;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = EnemiesSpeed;
        anim = GetComponent<Animator>();

        if (objetivo!=null)
        {
            ActivarRobo();
            objetivoMeshCollider = objetivo.GetComponent<MeshCollider>();
            objetivoTriggerCollider = objetivo.GetComponent<SphereCollider>();
            objetivoRigidbody = objetivo.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (!objetivoRobado && objetivo != null)
        {
            ActivarRobo();
        }
    }

    private void ActivarRobo()
    {
        navMeshAgent.destination = objetivo.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (/*other.CompareTag("Robable") &&*/ other.gameObject == objetivo && !muerto)
        {
            StartCoroutine(RobarObjetivo());
        }

        if (other.gameObject.TryGetComponent<SpawnEnemies>(out _) && objetivoRobado)
        {
           // print(name + " vuelve a punto "+Time.time);
            objetivo.transform.parent = null;

            LevelManager.Instance.ObjetivoRobadoPorEnemigo(); //aumentamos el numero de objetivos robados
            LevelManager.Instance.EnemyKilled(); //matamos a este enemigo que ha robado el objetivo, por ahora solo para finalizar el juego

            spawnEnemies.ObjetivoPerdido(objetivo); 

            //desactivamos el gameobject del enemigo que lo robo y del objetivo
            objetivo.SetActive(false);
            gameObject.SetActive(false);
        }
    }


    IEnumerator RobarObjetivo()
    {
        anim.SetBool(enObjetivoID, true);
        yield return new WaitForSeconds(0.5f);

        AtraparObjetivo();

        navMeshAgent.destination = puntoRetorno.position;
    }

    void AtraparObjetivo()
    {
        objetivoTriggerCollider.enabled = false;
        objetivoMeshCollider.isTrigger = false;
        objetivoMeshCollider.enabled = false;
        objetivoRigidbody.isKinematic = true;
        objetivoRigidbody.constraints = RigidbodyConstraints.None;

        //objetivo.transform.GetChild(0).transform.localPosition = Vector3.zero;

        objetivo.transform.position = mano.position;
        objetivo.transform.SetParent(mano);

        spawnEnemies.ObjetivoRobado(objetivo);

        objetivoRobado = true;
    }

    void SueltaObjetivo()
    {
        objetivoTriggerCollider.enabled = true;
        objetivoMeshCollider.enabled = true;
        objetivoRigidbody.isKinematic = false;
        objetivoRigidbody.useGravity = true;

        spawnEnemies.ObjetivoRecuperado(objetivo);

        objetivoRobado = false;
    }

    public void Muere()
    {
        muerto = true;
        StopAllCoroutines();

        SueltaObjetivo();

        SpawnEnemies.Instance.spawnedEnemies.Remove(this);

        navMeshAgent.enabled = false;
        enabled = false;
    }

    public Vector2 GetNormalizedVelocity()
    {
        float x = navMeshAgent.velocity.x / navMeshAgent.speed;
        float z = navMeshAgent.velocity.z / navMeshAgent.speed;

        return new Vector2(x, z);
    }
}
