using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI Instance { get; private set; }

    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Transform padreTutoriales;
    [SerializeField] private GameObject tutor;
    [SerializeField] private GameObject advertenciaPorNoDisparar;
    [SerializeField] private CinemachineVirtualCamera vCamTut;

    private bool showTutoriales;

    private GameObject currentTutorial;

    private int indexTutorials;

    private Animator animatorTutor;
    private int triggerTutorSeVaID;

    private bool firstTutorialShowed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        firstTutorialShowed = GameManager.Instance.IsTutorialComplete();
        showTutoriales = true;

        animatorTutor =tutor.GetComponent<Animator>();
        triggerTutorSeVaID = Animator.StringToHash("tutorSeVa");
        animatorTutor.ResetTrigger(triggerTutorSeVaID);
        tutor.SetActive(false);

        indexTutorials = 0;

        currentTutorial = padreTutoriales.GetChild(0).gameObject;
        //currentTutorial.SetActive(true);

        nextButton.onClick.AddListener(() =>
        {
            if (showTutoriales)
            {
                indexTutorials++;
                if (indexTutorials > 0)
                {
                    backButton.interactable = true;
                }
                if (indexTutorials < padreTutoriales.childCount)
                {
                    currentTutorial.SetActive(false);
                    GameObject nextTutorial = padreTutoriales.GetChild(indexTutorials).gameObject;
                    nextTutorial.SetActive(true);
                    currentTutorial = nextTutorial;
                }
                else
                {
                    FinalizarTutorial();
                }
            }
            else
            {
                FinalizarTutorial();
            }
            
        });

        backButton.onClick.AddListener(() =>
        {
            if (showTutoriales)
            {
                indexTutorials--;
                if (indexTutorials < 0)
                {
                    indexTutorials = 0;
                    backButton.interactable = false;
                }
                else
                {
                    currentTutorial.SetActive(false);
                    GameObject previousTut = padreTutoriales.GetChild(indexTutorials).gameObject;
                    previousTut.SetActive(true);
                    currentTutorial = previousTut;
                }
            }
            else
            {
                FinalizarTutorial();
            }
            
        });

        Hide();

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        if (!GameManager.Instance.IsTutorialComplete())
        {
            showTutoriales = true;
            GameManager.Instance.ActivarTutorial();
        }
        else
        {
            StartCoroutine(LateStartCountdown());
        }
        
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private IEnumerator LateStartCountdown()
    {
        yield return new WaitForSecondsRealtime(0.111f);
        GameManager.Instance.StartCountdownToPlay(false);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsInTutorial())
        {
            StartCoroutine(ShowTutor());
            Time.timeScale = 0;
        }
    }

    private void FinalizarTutorial()
    {
        Hide();
        StartCoroutine(HideTutor());

        if (!showTutoriales)
        {
            showTutoriales = true;
            GameManager.Instance.AdvertenciaComplete();
        }
        else
        {
            GameManager.Instance.TutorialComplete();
        }
        GameManager.Instance.TutorialComplete();
        Time.timeScale = 1;

        GameManager.Instance.StartCountdownToPlay(firstTutorialShowed);
        if (!firstTutorialShowed)
        {
            firstTutorialShowed = true;
        }

        indexTutorials = 0;
        currentTutorial.SetActive(false);
        currentTutorial = padreTutoriales.GetChild(0).gameObject;
        
    }

    private void Show()
    {
        tutorialUI.SetActive(true);
        if (showTutoriales)
        {
            currentTutorial.SetActive(true);
        }
        else
        {
            advertenciaPorNoDisparar.SetActive(true);
        }
    }

    private IEnumerator ShowTutor()
    {
        yield return new WaitForSecondsRealtime(0.111f);
        tutor.SetActive(true);
        //print("empieza animacion?");
        yield return new WaitForSecondsRealtime(1.666f);
        vCamTut.Priority = 11;
        //yield return new WaitUntil(() => animatorTutor.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 
        //                                 && !animatorTutor.IsInTransition(0));
        Show();

        //print("terminar animacion de show?");
    }

    private IEnumerator HideTutor()
    {
        vCamTut.Priority = 0;
        animatorTutor.SetTrigger(triggerTutorSeVaID);
        //print("empieza animacion?"+Time.time);
        yield return new WaitForSecondsRealtime(2f);
        //yield return new WaitUntil(() => animatorTutor.GetCurrentAnimatorStateInfo(0).normalizedTime > 1
        //                                 && !animatorTutor.IsInTransition(0));
        
        tutor.SetActive(false);
        //print("terminar animacion de show?"+Time.time);
    }

    private void Hide()
    {
        tutorialUI.SetActive(false);
        if (showTutoriales)
        {
            currentTutorial.SetActive(false);
        }
        else
        {
            advertenciaPorNoDisparar.SetActive(false);
        }
    }

    public void ShowAdvertenciaPorNoDisparar()
    {
        showTutoriales = false;
        GameManager.Instance.ActivarTutorial();
    }
}
