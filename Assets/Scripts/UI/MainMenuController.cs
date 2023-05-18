using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button NoAIButton;
    [SerializeField] private Button AI2Button;
    [SerializeField] private Button AI3Button;

    [SerializeField] private Toggle toggleMusicMute;

    private Loader.Scene levelScene;

    private void Start()
    {

        level1Button.onClick.AddListener(() =>
        {
            levelScene = Loader.Scene.LevelWaterfall;
        });
        level2Button.onClick.AddListener(() =>
        {
            levelScene = Loader.Scene.LevelDesert;
        });
        level3Button.onClick.AddListener(() =>
        {
            levelScene = Loader.Scene.LevelPastizal;
        });

        NoAIButton.onClick.AddListener(() =>
        {
            Loader.Load(levelScene, Loader.AI.NoAI);
        });
        AI2Button.onClick.AddListener(() =>
        {
            Loader.Load(levelScene, Loader.AI.AI1);
        });
        AI3Button.onClick.AddListener(() =>
        {
            Loader.Load(levelScene, Loader.AI.AI2);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });

        toggleMusicMute.onValueChanged.AddListener((value) =>
        {
            MusicManager.Instance.SetMute(value);
        });

        if (GameManager.Instance.IsGamePause())
        {
            GameManager.Instance.TogglePauseGame();
        }

        //highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
    }

}
