using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject>
{
}

public class SpawnEnemies : MonoBehaviour
{
    [HideInInspector] public GameObjectEvent OnCarrotStolen;
    [HideInInspector] public GameObjectEvent OnCarrotRecovered;
    [HideInInspector] public GameObjectEvent OnCarrotLost;

    public static SpawnEnemies Instance { get; private set; }

    public List<Ladron> spawnedEnemies { get; set; }

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject enemyPrefab;
     
    //ubi objetivos para eenemigos
    [SerializeField] private Transform objetivosPadre;
    private List<Transform> objetivos;

    [SerializeField] private int numEnemigos;
    [SerializeField] private float tiempoEntreSpawn;
    private float nextSpawnTime;

    bool spawnear;

    int enObjetivoID;

    [SerializeField] private float enemySpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.TotalMovilTargets = numEnemigos;
        enObjetivoID = Animator.StringToHash("enObjetivo");
        //nextSpawnTime = Time.time + tiempoEntreSpawn;
        objetivos = new List<Transform>();
        for (int i = 0; i < objetivosPadre.childCount; i++)
        {
            objetivos.Add(objetivosPadre.GetChild(i));
        }
        for (int i = 0; i < numEnemigos; i++)
        {
            Ladron ladronActual = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Ladron>();
            ladronActual.EnemiesSpeed= enemySpeed;
            ladronActual.SpawnEnemies = this;
            ladronActual.transform.parent = transform;
            ladronActual.gameObject.SetActive(false);
        }
        spawnear = true;
        //PrintObjetivos();

        spawnedEnemies = new List<Ladron>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (spawnear && Time.time > nextSpawnTime && numEnemigos > 0)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + tiempoEntreSpawn;
            numEnemigos--;
        }
        else if (transform.childCount>0 && !spawnear && objetivos.Count > 0)
        {
            spawnear = true;
        }
    }

    void SpawnEnemy()
    {
        if (objetivos.Count > 0 && transform.childCount > 0)
        {
            Ladron ladronActual = transform.GetChild(0).GetComponent<Ladron>();
            ladronActual.Objetivo = objetivos[0].gameObject;
            ladronActual.SpawnEnemies = this;
            ladronActual.puntoRetorno = spawnPoint;
            ladronActual.EnObjetivoID = enObjetivoID;

            ladronActual.gameObject.SetActive(true);

            ladronActual.transform.parent = null;
            
            objetivos.RemoveAt(0);

            spawnedEnemies.Add(ladronActual);
            
        }
        else
        {
            spawnear = false;
        }
        //PrintObjetivos();
    }

    void PrintObjetivos()
    {
        for (int i = 0; i < objetivos.Count; i++)
        {
            print(i + " " + objetivos[i]);
        }
    }

    private void ObtainObjetivoSuelto(Transform objetivoSuelto)
    {
        objetivoSuelto.transform.parent = objetivosPadre;
        objetivos.Add(objetivoSuelto);
    }

    public Transform GetObjetivosPadre() {
        return objetivosPadre;
    }

    public void ObjetivoRobado(GameObject objetivo)
    {
        OnCarrotStolen?.Invoke(objetivo);
    }

    public void ObjetivoRecuperado(GameObject objetivo)
    {
        ObtainObjetivoSuelto(objetivo.transform);
        OnCarrotRecovered?.Invoke(objetivo);
    }

    public void ObjetivoPerdido(GameObject objetivo)
    {
        OnCarrotLost?.Invoke(objetivo);

        if (LevelManager.Instance.TodosLosObjetivosRobados())
        {
            for (int i = 0; i < numEnemigos; i++)
            {
                LevelManager.Instance.EnemyKilled();
            }
            numEnemigos = 0;
        }
    }

    //public void DestruirObjetivo(GameObject objetivo)
    //{
    //    objetivos.Remove(objetivo.transform);
    //    Transform psDestruction = objetivo.transform.GetChild(0);
    //    psDestruction.parent = null;
    //    psDestruction.GetComponent<ParticleSystem>().Play();
    //    Destroy(objetivo, .25f);
    //}
}
