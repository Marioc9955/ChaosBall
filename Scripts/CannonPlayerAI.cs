using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Unity.Barracuda;
using Unity.Mathematics;
using UnityEngine;

public class CannonPlayerAI : MonoBehaviour
{
    [SerializeField] private NNModel cannonAIModel;

    private Model m_CannonRuntimeModel;
    private IWorker m_Worker;
    Dictionary<string, Tensor> inputs;

    private List<ProyectilPlayer> shootedProjectiles;

    [SerializeField] private Transform objetivo;
    [SerializeField] private Transform objetivosEstaticos;

    [Header("Variables para disparar")]
    [SerializeField] private float reloadCannonTime;
    [SerializeField] private Transform cannonTip;
    [SerializeField] private float force;
    [SerializeField] private ParticleSystem psCannonExplosion;
    [SerializeField] private int totalMultipleShots;
    [SerializeField] private int totalShotsOneTarget;
    [SerializeField] private GameObject esferasPadre;

    [Header("Variables para rotacion")]
    [SerializeField] private Transform rotacionVertical; //eje X
    [SerializeField] private Transform rotacionHorizontal; //eje Y
    [SerializeField] private float rotationFactor;

    private bool canAim, canShoot, changeTarget;

    private int shotsOneTarget;


    private void Start()
    {
        canAim = true;
        canShoot = true;

        m_CannonRuntimeModel = ModelLoader.Load(cannonAIModel);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, m_CannonRuntimeModel);

        inputs = new Dictionary<string, Tensor>();
        inputs["obs_0"] = new Tensor(1, 1, 3, 10);
        inputs["obs_1"] = new Tensor(1, 1, 1, 4);
        inputs["obs_2"] = new Tensor(1, 1, 1, 3);
        inputs["action_masks"] = new Tensor(1, 1, 1, 2);

        shootedProjectiles = new List<ProyectilPlayer>();

        shotsOneTarget= 0;
        changeTarget = true;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        //if (shotsOneTarget >= totalShotsOneTarget)
        //{
        //    changeTarget = true;
        //    shotsOneTarget = 0;
        //}

        //if (changeTarget)
        //{
        //    ElegirObjetivo();
        //    changeTarget = false;
        //}

        CollectObservations();

        m_Worker.Execute(inputs);

        PerformActions();

        foreach (Tensor input in inputs.Values)
        {
            input.Dispose();
        }
    }

    public void CambiarObjetivo()
    {
        changeTarget = true;
    }

    private void ElegirObjetivo()
    {
        bool trueOrFalse = UnityEngine.Random.value > 0.5f;
        if (trueOrFalse)
        {
            int i = UnityEngine.Random.Range(0, objetivosEstaticos.childCount);
            objetivo = objetivosEstaticos.GetChild(i);
        }
        else
        {
            List<Ladron> objetivosMoviles = SpawnEnemies.Instance.spawnedEnemies;
            if (objetivosMoviles.Count > 0)
            {
                int i = UnityEngine.Random.Range(0, objetivosMoviles.Count);
                objetivo = objetivosMoviles[i].transform;
            }
            else
            {
                int i = UnityEngine.Random.Range(0, objetivosEstaticos.childCount);
                objetivo = objetivosEstaticos.GetChild(i);
            }
        }
    }

    void PointAndShoot(Tensor discreteAction, Tensor continuousActions)
    {
        if (canAim)
        {
            var verticalMove = continuousActions[0];
            Vector3 ear = rotacionVertical.transform.rotation.ToEulerAngles();
            if ((verticalMove > 0 && ear.z < 0f) ||
            (verticalMove < 0 && ear.z > -1.45333f))
            {
                rotacionVertical.Rotate(0, 0, verticalMove * rotationFactor);
            }

            var horizontalMove = continuousActions[1];
            rotacionHorizontal.Rotate(0, horizontalMove * rotationFactor, 0);
        }
        if (canShoot && discreteAction[0] > 0 && shootedProjectiles.Count < totalMultipleShots)
        {
            Invoke(nameof(Disparar), .111f);
            canAim = false;
            canShoot = false;
            Invoke(nameof(ActivateAim), .5f);
            Invoke(nameof(ActivateShoot), reloadCannonTime);
        }
    }

    void ActivateAim()
    {
        canAim = true;
    }

    void ActivateShoot()
    {
        canShoot = true;
    }

    void Disparar()
    {
        
        if (esferasPadre.transform.childCount > 0)
        {
            Vector3 _dirShoot = cannonTip.position - cannonTip.parent.position;
            ProyectilPlayer _proyectil;
            _proyectil = esferasPadre.transform.GetChild(0).GetComponent<ProyectilPlayer>();
            _proyectil.transform.position = cannonTip.parent.position;
            _proyectil.transform.localScale = Vector3.one;
            _proyectil.transform.localScale = new Vector3(1 / _proyectil.transform.lossyScale.x,
                1 / _proyectil.transform.lossyScale.y, 1 / _proyectil.transform.lossyScale.z);
            psCannonExplosion.Play();
            _proyectil.Fired(_dirShoot * force);
            _proyectil.cannonPlayerAI = this;
            shootedProjectiles.Add(_proyectil);

            shotsOneTarget++;
        }
        else
        {
            GameManager.Instance.GameOver("Te quedaste sin esferas de cañón explosivas");
            return;
        }
    }

    private void PerformActions()
    {
        Tensor discreteAction = m_Worker.PeekOutput("discrete_actions");
        Tensor continuousActions = m_Worker.PeekOutput("continuous_actions");
        PointAndShoot(discreteAction, continuousActions);
    }

    public void OnDestroy()
    {
        m_Worker?.Dispose();

        foreach (var key in inputs.Keys)
        {
            inputs[key].Dispose();
        }

        inputs.Clear();
    }

    private void CollectObservations()
    {
        //var lowerD = 100f;
        //GameObject nearestP = null;
        for (int i = 0; i < shootedProjectiles.Count; i++)
        {
            Vector3 pPos = NormalizedProjectilePositon(shootedProjectiles[i].transform.position - transform.position);
            float[] obsProjectile = { pPos.x, pPos.y, pPos.z };
            for (int j = 0; j < 3; j++)
            {
                inputs["obs_0"][0, 0, j, i] = obsProjectile[j];
            }
        }
        Vector2 objPos = NormalizedTargetPosition(objetivo.position - transform.position);
        inputs["obs_1"][0] = objPos.x;
        inputs["obs_1"][1] = objPos.y;
        if (objetivo.TryGetComponent(out Ladron l))
        {
            Vector2 velNormObj = l.GetNormalizedVelocity();
            inputs["obs_1"][2] = velNormObj.x;
            inputs["obs_1"][3] = velNormObj.y;
        }
        else {
            inputs["obs_1"][2] = 0; //velocidad objetivo estatico es cero
            inputs["obs_1"][3] = 0;
        }

        float angleHorizontalRotation = rotacionHorizontal.localRotation.eulerAngles.y / 360f;
        inputs["obs_2"][0] = angleHorizontalRotation;
        float angleVerticalRotation = Vector3.Angle(transform.forward, cannonTip.position - cannonTip.parent.position);
        angleVerticalRotation = angleVerticalRotation / 90.0f;
        inputs["obs_2"][1] = angleVerticalRotation;

        Vector3 _dirShoot = cannonTip.position - cannonTip.parent.position;
        Vector3 _dirTarget = objetivo.position - transform.position;

        //angulo entre la direccion de apuntado y direccion del 
        float angle = Vector3.Angle(_dirShoot, _dirTarget);
        inputs["obs_2"][2] = angle;
    }

    Vector2 NormalizedTargetPosition(Vector3 targetPos)
    {
        //float minValue = -radioTargetRandomPosicion;
        //float maxValue = radioTargetRandomPosicion;
        //float value = TargetPos.x;
        //float normalizedValueX = (value - minValue) / (maxValue - minValue);  // normalize between 0 and 1
        //normalizedValueX = (normalizedValueX * 2) - 1;  // scale to -1 to 1 range
        //value = TargetPos.z;
        //float normalizedValueZ = (value - minValue) / (maxValue - minValue);  // normalize between 0 and 1
        //normalizedValueZ = (normalizedValueZ * 2) - 1;  // scale to -1 to 1 range
        float value = targetPos.x;
        float normalizedValueX = value / 88;
        value = targetPos.z;
        float normalizedValueZ = value / 88;
        return new Vector2(normalizedValueX, normalizedValueZ);
    }

    Vector3 NormalizedProjectilePositon(Vector3 projectilePos)
    {
        float value = projectilePos.x;
        float normalizedValueX = value / 88;
        value = projectilePos.z;
        float normalizedValueZ = value / 88;
        float normalizedValueY = projectilePos.y / 37.2f; //37.2 altura maxima de un projectile disparado hacia arriba con el mayor angulo permitido
        return new Vector3(normalizedValueX, normalizedValueY, normalizedValueZ);
    }

    public void DeleteShootedProjectile(ProyectilPlayer p)
    {
        if (shootedProjectiles.Contains(p))
        {
            shootedProjectiles.Remove(p);
        }
    }
}
