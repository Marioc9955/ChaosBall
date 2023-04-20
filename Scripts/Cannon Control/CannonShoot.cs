using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class CannonShoot : MonoBehaviour
{
    public static CannonShoot Instance { get; private set; }

    [Header("Variables para control de disparo")]
    //[SerializeField] private Proyectil esferaPrefab;
    [SerializeField] private float fuerzaMax;
    [SerializeField] private LayerMask layerCanon, layerGround;
    [SerializeField] private Transform cannonTip, cannonBottom;
    [SerializeField] private ParticleSystem psCannonExplosion;
    [SerializeField] private GameObject esferasPadre;
    [SerializeField] private float cooldownTime = .999f;
    [SerializeField] private float timeAllowedWithNoShoot;

    [Header("Variables para visualizar disparo")]
    [SerializeField] private MeshRenderer mrCannon;
    [SerializeField] private Material materialTransparente, materialCannon;
    [SerializeField] private Projection _projection;

    private ProyectilPlayer _proyectil;
    private Vector3 _dirShoot;
    
    private ManipulateChildren manipularEsferas;

    private float fireInputValue;

    private GameInput gameInput;

    private bool cannonCargado, canShoot;

    private float timeWithoutShoot;

    private float inverseLerpBallPosition;

    private MeshRenderer primerProyectilMR;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inverseLerpBallPosition = -1;

        canShoot = true;
        cannonCargado = false;
        gameInput = GameInput.Instance;

        gameInput.OnFireStarted += GameInput_OnFireStarted;
        gameInput.OnFireCanceled += GameInput_OnFireCanceled;

        manipularEsferas= esferasPadre.GetComponent<ManipulateChildren>();

        materialCannon.color = Color.HSVToRGB(0, 0, .75f);

        timeWithoutShoot = 0;

        primerProyectilMR = esferasPadre.transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    private void GameInput_OnFireCanceled(object sender, System.EventArgs e)
    {
        fireInputValue = 0;
        ShootCannon();
    }

    private void GameInput_OnFireStarted(object sender, System.EventArgs e)
    {
        fireInputValue = 0;
        CargarCannon(cannonTip.position);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (primerProyectilMR!=null)
        {
            float sProyectil = Mathf.InverseLerp(0, timeAllowedWithNoShoot, timeWithoutShoot);
            primerProyectilMR.material.color = Color.HSVToRGB(0, sProyectil, 1);
        }
        else
        {
            if (esferasPadre.transform.childCount>0)
            {
                primerProyectilMR = esferasPadre.transform.GetChild(0).GetComponent<MeshRenderer>();
            }
            else
            {
                primerProyectilMR = null;
            }
        }
        timeWithoutShoot += Time.deltaTime;
        if (timeWithoutShoot >= timeAllowedWithNoShoot)
        {
            timeWithoutShoot = 0;
            //print("Se demora en disparar");
            //explotar un proyectil
            if (primerProyectilMR!=null)
            {
                primerProyectilMR.transform.parent = null;
                primerProyectilMR.GetComponent<Proyectil>().Explode();
                manipularEsferas.PosCircularHijos();
            }
            else
            {
                GameManager.Instance.GameOver("Te quedaste sin esferas de cañón explosivas");
            }
            //mostrar advertencia sobre consecuencias de no disparar solo si aun no la ha visto
            if (!GameManager.Instance.IsAdvertenciaComplete())
            {
                TutorialUI.Instance.ShowAdvertenciaPorNoDisparar();
            }
            //herir jugador
            PlayerLifeController.Instance.HurtPlayer();
        }

        if (!cannonCargado)
        {
            return;
        }

        float v = gameInput.GetFireValue();
        if (v != 0 && fireInputValue <= 1)
        {
            fireInputValue += Time.deltaTime;
            float s = Mathf.Lerp(0.24f, 1, fireInputValue);
            materialTransparente.color = Color.HSVToRGB(0, s, .55f);

            Vector3 pos = Vector3.Lerp(cannonTip.position, cannonBottom.position, fireInputValue);
            UpdateProjectilePositionAndDirShoot(pos);
        }
    }

    private void OnMouseDown()
    {
        CargarCannon(GetMouseWorldPosition(layerCanon));
    }

    private void OnMouseDrag()
    {
        if (!GameManager.Instance.IsGamePlaying() || !cannonCargado) return;
        Vector3 pos = GetMouseWorldPosition(layerCanon);
        if (pos != Vector3.zero)
        {
            pos = PuntoMasCercanoRectaDisparo(pos);
            UpdateProjectilePositionAndDirShoot(pos);

            inverseLerpBallPosition = VirtualJoystick.InverseLerp(cannonTip.position, cannonBottom.position, pos);
            //materialTransparente.color = Color.HSVToRGB(0, inverseLerpBallPosition, .55f);
            UpdateTransparencyMaterialTrans(inverseLerpBallPosition);
        }
        else
        {
            inverseLerpBallPosition = -1;
        }
    }

    public void UpdateTransparencyMaterialTrans(float transparency)
    {
        materialTransparente.color = Color.HSVToRGB(0, transparency, .55f);
    }

    private void OnMouseUp()
    {
        ShootCannon();
    }

    public void ShootCannon()
    {
        if (!GameManager.Instance.IsGamePlaying() || !cannonCargado) return;

        _dirShoot = (cannonTip.position - _proyectil.transform.position);
        _proyectil.Fired(_dirShoot * fuerzaMax);

        SoundManager.Instance.PlayCannonFireSFX(cannonTip.position,.666f);
        
        mrCannon.material = materialCannon;
        
        StartCoroutine(CannonCooldown());
        
        psCannonExplosion.Play();
        
        _projection.DrawTrajectoryOnShoot(_proyectil);

        cannonCargado = false;

        timeWithoutShoot = 0;

        inverseLerpBallPosition = -1;
    }

    IEnumerator CannonCooldown()
    {
        float timeCooling = cooldownTime;
        while (timeCooling > 0)
        {
            float s = Mathf.InverseLerp(0, cooldownTime, timeCooling);
            materialCannon.color = Color.HSVToRGB(0, s, .75f);
            yield return new WaitForSeconds(Time.deltaTime);
            timeCooling -= Time.deltaTime;
        }
        canShoot = true;
    }

    public void UpdateProjectilePositionAndDirShoot(Vector3 pos)
    {
        _proyectil.transform.position = pos;
        _dirShoot = (cannonTip.position - pos);
    }

    public void CargarCannon(Vector3 cannonPos)
    {
        if (!GameManager.Instance.IsGamePlaying() || cannonCargado) return;

        //_proyectil = Instantiate(esferaPrefab, cannonPos, Quaternion.identity);
        if (!canShoot)
        {
            return;
        }
        if (esferasPadre.transform.childCount > 0)
        {
            SoundManager.Instance.PlayLoadingCannonSFX(cannonTip.position,.666f);

            _proyectil = esferasPadre.transform.GetChild(0).GetComponent<ProyectilPlayer>();
            _proyectil.transform.parent = cannonTip;

            if (esferasPadre.transform.childCount>0)
            {
                primerProyectilMR = esferasPadre.transform.GetChild(0).GetComponent<MeshRenderer>();
            }
            else
            {
                primerProyectilMR = null;
            }

            _proyectil.transform.localScale = Vector3.one;
            _proyectil.transform.localScale = new Vector3(1 / _proyectil.transform.lossyScale.x, 
                1 / _proyectil.transform.lossyScale.y, 1 / _proyectil.transform.lossyScale.z);

            _proyectil.transform.position = cannonPos;

            manipularEsferas.PosCircularHijos();

            cannonCargado = true;
            canShoot= false;
            timeWithoutShoot = 0;
        }
        else
        {
            cannonCargado = false;
            GameManager.Instance.GameOver("Te quedaste sin esferas de cañón explosivas");
            return;
        }
        mrCannon.material = materialTransparente;
    }

    private Vector3 GetMouseWorldPosition(LayerMask lm)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, lm))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 PuntoMasCercanoRectaDisparo(Vector3 positionEsfera)
    {
        //obtengo direccion recta
        Vector3 ct = cannonTip.position;
        Vector3 dirRec = ct - cannonTip.parent.position;

        //obtengo ecuacion plano Ax + By + Cz = D Donde ABC dir recta y (x, y z) punto esfera para obtener D
        float D = Vector3.Dot(dirRec, positionEsfera);

        //float landa = (D - dirRec.x * ct.x - dirRec.y * ct.y) /(dirRec.x * dirRec.x);
        float landa = (D - Vector3.Dot(dirRec, ct)) / Vector3.Dot(dirRec, dirRec);

        return ct + landa * dirRec;
    }

    public float GetFireInputValue()
    {
        return fireInputValue;
    }

    public bool IsCannonCargado()
    {
        return cannonCargado;
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    public float GetInverseLerpBallPosition()
    {
        return inverseLerpBallPosition;
    }

    public Vector3 LerpCannonTipToBottom(float v)
    {
        return Vector3.Lerp(cannonTip.position, cannonBottom.position, v);
    }
}

