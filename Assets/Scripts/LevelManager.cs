using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Transform staticTargets;
    private Transform enemyTargets;


    private int totalStaticTargets;
    private int totalEnemyTargets;

    public int TotalMovilTargets { get; set; }

    private int killedEnemies;

    private int objetivosRobadosPorEnemigos;


    private void Awake()
    {
        killedEnemies = 0;
        Instance = this;
    }

    private void Start()
    {
        objetivosRobadosPorEnemigos = 0;
        totalStaticTargets = staticTargets.childCount;
        enemyTargets = SpawnEnemies.Instance.GetObjetivosPadre();
        totalEnemyTargets= enemyTargets.childCount;
    }

    public int GetTotalPlayerTargets()
    {
        return totalStaticTargets + TotalMovilTargets;
    }

    public int GetFinalTargetsKilled()
    {
        return killedEnemies;
    }

    public int GetEnemyTargetsLeft()
    {
        return enemyTargets.childCount;
    }

    public int GetTotalEnemyTargets()
    {
        return totalEnemyTargets;
    }

    public void EnemyKilled()
    {
        killedEnemies++;
        if (killedEnemies >= GetTotalPlayerTargets())
        {
            GameManager.Instance.GameOver("¡Ganaste! Derrotaste a todos los agentes que conquistaron estas tierras.");
        }
    }

    public void ObjetivoRobadoPorEnemigo()
    {
        objetivosRobadosPorEnemigos++;
    }

    public bool TodosLosObjetivosRobados()
    {
        return objetivosRobadosPorEnemigos >= totalEnemyTargets;
    }
}
