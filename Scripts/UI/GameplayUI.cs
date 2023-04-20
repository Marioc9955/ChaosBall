using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private GameObject deactivatedImage;
    [SerializeField] private Button followCamButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button fireButton;

    [SerializeField] private TextMeshProUGUI scoreText, timeText;

    [SerializeField] private Image cannonImage;
    [SerializeField] private Image ballImage;

    [SerializeField] private TextMeshPro scoreInWorldText;

    [SerializeField] private VirtualJoystick joystickRotationCannon;
    [SerializeField] private VirtualJoystick joystickRotationCamara;

    [SerializeField] private Transform padreImagesUIObjetivosEnemy;

    private Dictionary<GameObject, Image> objetivosEnemyGOImg;

    private CameraManager camManager;

    private void Start()
    {
        scoreInWorldText.gameObject.SetActive(false);
        camManager = CameraManager.Instance;

        UpdateImagenButtonFollowCam();

        followCamButton.onClick.AddListener(() =>
        {
            camManager.ToggleFollowCam();
            UpdateImagenButtonFollowCam();

            if (!camManager.IsFollowCamActive())
            {
                camManager.BackToCannon();
            }
        });

        pauseButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });

        joystickRotationCannon.outputEventVector.AddListener(dir => {
            CannonRotationManager.Instance.RotarConJoystick(dir);
        });
        joystickRotationCamara.outputEventVector.AddListener(dir => {
            CameraManager.Instance.MoverCamera(dir);
        });

        GameManager.Instance.OnUpScore += GameManager_OnUpScore;

        GameInput.Instance.OnFireStarted += GameInput_OnFireStarted;

        InicializarDictionatyObjetivosEnemy();

        SpawnEnemies.Instance.OnCarrotStolen.AddListener(carrot => CambiarColorObjetivoEnemy(carrot, new Color(1f,0.333f,0.333f)));
        SpawnEnemies.Instance.OnCarrotRecovered.AddListener(carrot => CambiarColorObjetivoEnemy(carrot, new Color(1f, 1f, 1f)));
        SpawnEnemies.Instance.OnCarrotLost.AddListener(carrot => CambiarColorObjetivoEnemy(carrot, new Color(0.333f, 0.333f, 0.333f)));
    }


    private void InicializarDictionatyObjetivosEnemy()
    {
        Transform objetivosPadre = SpawnEnemies.Instance.GetObjetivosPadre();
        objetivosEnemyGOImg = new Dictionary<GameObject, Image>();    
        for (int i = 0; i < objetivosPadre.childCount; i++)
        {
            objetivosEnemyGOImg[objetivosPadre.GetChild(i).gameObject] = padreImagesUIObjetivosEnemy.GetChild(i).GetComponent<Image>();
        }
    }

    private void CambiarColorObjetivoEnemy(GameObject objetivo, Color c)
    {
        Image imgObjetivoEnemy = objetivosEnemyGOImg[objetivo];
        imgObjetivoEnemy.color = c;
    }

    private void GameInput_OnFireStarted(object sender, System.EventArgs e)
    {
        if (CannonShoot.Instance.CanShoot())
        {
            ballImage.enabled = true;
            cannonImage.color = Color.HSVToRGB(0, 0, 1);
            ballImage.transform.localPosition = new Vector3(ballImage.transform.localPosition.x, 111);
        }
        
    }

    private void GameManager_OnUpScore(object sender, ScoreInWorldEventArgs e)
    {
        scoreInWorldText.gameObject.SetActive(true);
        //scoreInWorldText.transform.LookAt(Camera.main.transform.position);
        scoreInWorldText.transform.LookAt(PlayerLifeController.Instance.transform.position);
        scoreInWorldText.transform.Rotate(0, 180, 0);
        scoreText.text = "Puntaje: " + GameManager.Instance.GetScore().ToString();
        scoreInWorldText.text = "+" + e.upScore;
        scoreInWorldText.transform.position = e.position;
        StartCoroutine(DesaparecerScoreInWorld());
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnUpScore -= GameManager_OnUpScore;

        GameInput.Instance.OnFireStarted -= GameInput_OnFireStarted;
    }

    private IEnumerator DesaparecerScoreInWorld()
    {
        yield return new WaitForSeconds(2);
        scoreInWorldText.gameObject.SetActive(false);
    }

    private void Update()
    {
        timeText.text = "Tiempo: " + GameManager.Instance.GetGamePlayingTimer().ToString("F2");

        if (CannonShoot.Instance.IsCannonCargado())
        {
            float fireInputValue = CannonShoot.Instance.GetFireInputValue();
            if (fireInputValue > 0 && fireInputValue <= 1)
            {
                cannonImage.color = Color.HSVToRGB(0, fireInputValue, 1);
                float yBall = Mathf.Lerp(111, 0, fireInputValue);
                ballImage.transform.localPosition = new Vector3(ballImage.transform.localPosition.x, yBall);
            }
        }

        float inverseLerpBallPos = CannonShoot.Instance.GetInverseLerpBallPosition();
        if (inverseLerpBallPos != -1)
        {
            if (inverseLerpBallPos > 0 && inverseLerpBallPos <= 1)
            {
                ballImage.enabled = true;
                cannonImage.color = Color.HSVToRGB(0, inverseLerpBallPos, 1);
                float yBall = Mathf.Lerp(111, 0, inverseLerpBallPos);
                ballImage.transform.localPosition = new Vector3(ballImage.transform.localPosition.x, yBall);
            }
        }
    }

    void UpdateImagenButtonFollowCam()
    {
        var img = followCamButton.GetComponent<Image>();
        var col = img.color;
        if (camManager.IsFollowCamActive())
        {
            img.color = new Color(col.r, col.g, col.b, 1);
        }
        else
        {
            img.color = new Color(col.r, col.g, col.b, 111f / 255f);
        }
        deactivatedImage.SetActive(!camManager.IsFollowCamActive());
    }
}
