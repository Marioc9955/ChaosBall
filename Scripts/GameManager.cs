using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.CullingGroup;

public class ScoreInWorldEventArgs : EventArgs
{
    public Vector3 position;
    public int upScore;

    public ScoreInWorldEventArgs(Vector3 vector, int upScore)
    {
        this.position = vector;
        this.upScore= upScore;
    }
}

public class GameManager : MonoBehaviour
{
    public const string PLAYER_PREFS_TUTORIAL_COMPLETE = "IsTutorialComplete";
    public const string PLAYER_PREFS_ADVERTENCIA_COMPLETE = "IsAdvertenciaComplete";
    public string RazonFinal { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler<ScoreInWorldEventArgs> OnUpScore;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
        Tutorial
    }

    public static GameManager Instance { get; private set; }

    private int score;

    private State state;
    private float countdownToStartTimer = 1f;
    private float gamePlayingTimer;
    private bool isGamePaused = false;
    private bool isTutorialComplete;
    private bool isAdvertenciaComplete;
    private bool isGameStarted;

    private void Awake()
    {
        isGameStarted = false;
        isTutorialComplete = PlayerPrefs.GetInt(PLAYER_PREFS_TUTORIAL_COMPLETE, 0) != 0;
        isAdvertenciaComplete = PlayerPrefs.GetInt(PLAYER_PREFS_ADVERTENCIA_COMPLETE, 0) != 0;
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        state = State.WaitingToStart;
    }

    //private void Start()
    //{
    //    GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
    //}

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    if (!isGameStarted) gamePlayingTimer = 0;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer += Time.deltaTime;
                break;
            case State.GameOver:
                break;
            case State.Tutorial: break;
        }
    }

    public void MainMenu()
    {
        //   SceneManager.LoadScene(0);
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void StartCountdownToPlay(bool gameStarted)
    {
        isGameStarted = gameStarted;
        if (isGameStarted)
        {
            countdownToStartTimer = 1f;
        }
        else
        {
            countdownToStartTimer = 3f;
        }
        

        state= State.CountdownToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void GameOver(string razonFinal)
    {
        RazonFinal= razonFinal;
        state = State.GameOver;
        SoundManager.Instance.PlayGameOverSFX(1);
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;

            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }


    public void UpScore(int score, Vector3 posicionScoreUp)
    {
        this.score += score;
        OnUpScore?.Invoke(this, new ScoreInWorldEventArgs(posicionScoreUp, score));
    }

    public void ActivarTutorial()
    {
        state = State.Tutorial;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TutorialComplete()
    {
        isTutorialComplete = true;
        PlayerPrefs.SetInt(PLAYER_PREFS_TUTORIAL_COMPLETE, 1);
    }

    public void AdvertenciaComplete()
    {
        isAdvertenciaComplete = true;
        PlayerPrefs.SetInt(PLAYER_PREFS_ADVERTENCIA_COMPLETE, 1);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying && !isGamePaused;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public bool IsInTutorial()
    {
        return state == State.Tutorial;
    }

    public bool IsTutorialComplete()
    {
        return isTutorialComplete;
    }

    public bool IsAdvertenciaComplete()
    {
        return isAdvertenciaComplete;
    }

    public bool IsGamePause()
    {
        return isGamePaused;
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer;
    }

    public int GetScore()
    {
        return score;
    }

    //public void FinishGame()
    //{
    //    if (PlayerPrefs.HasKey("HighScore"))
    //    {
    //        if (score > PlayerPrefs.GetInt("HighScore"))
    //        {
    //            highScore = score;
    //            PlayerPrefs.SetInt("HighScore", highScore);
    //        }
    //    }
    //    else
    //    {
    //        highScore = score;
    //        PlayerPrefs.SetInt("HighScore", highScore);
    //    }
    //}
}
