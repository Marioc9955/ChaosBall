using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class CannonAIManager : MonoBehaviour
{
    public static CannonAIManager Instance{get; private set;}
    public bool muerto { get; private set; }

    [SerializeField] private NNModel AI1, AI2;

    [SerializeField] private GameObject noAI;
    [SerializeField] private GameObject AI;

    private CannonAI cannonAI;

    [Header("Control de la vida del cannon AI")]
    [SerializeField] private float life = 1f;
    [SerializeField] private ParticleSystem psCannonDamage;
    [SerializeField] private ParticleSystem psCannonExplosion;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        switch (Loader.targetAI)
        {
            case Loader.AI.NoAI:
                //noAI.SetActive(true);
                //AI.SetActive(false);
                print("No AI");
                noAI.SetActive(false);
                AI.SetActive(true);
                cannonAI = AI.GetComponent<CannonAI>();
                cannonAI.CannonAIModel = AI1;
                break;
            case Loader.AI.AI1:
                noAI.SetActive(false);
                AI.SetActive(true);
                cannonAI = AI.GetComponent<CannonAI>();
                cannonAI.CannonAIModel = AI1;
                break;
            case Loader.AI.AI2:
                noAI.SetActive(false);
                AI.SetActive(true);
                cannonAI = AI.GetComponent<CannonAI>();
                cannonAI.CannonAIModel = AI2;
                break;
            default:
                break;
        }
        ActualizarEstadoCannon();
    }

    public void HurtCannonAI()
    {
        print("hurt cannon ai");
        life -= 0.2f;
        ActualizarEstadoCannon();
    }

    private void ActualizarEstadoCannon()
    {
        float emisionParticles = Mathf.Lerp(12, 0, life);
        ParticleSystem.EmissionModule emModule = psCannonDamage.emission;
        emModule.rateOverTime = emisionParticles;
        if (life <= 0)
        {
            Muere();
        }
    }

    private void Muere()
    {
        muerto = true;
        psCannonExplosion.Play();
        noAI.SetActive(true);
        AI.SetActive(false);
    }
}
