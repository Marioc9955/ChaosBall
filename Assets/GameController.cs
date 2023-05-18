using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Button pauseButton, unPauseButton, mainMenuButton, mainMenuGameOver;
    [SerializeField] GameObject panelPausa, panelGameOver;

    private void Start()
    {
        pauseButton.onClick.AddListener(Pausa);
        unPauseButton.onClick.AddListener(DesPausa);
        mainMenuButton.onClick.AddListener(GameManager.Instance.MainMenu);
        mainMenuGameOver.onClick.AddListener(GameManager.Instance.MainMenu);
    }

    void Pausa()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0;
    }

    void DesPausa()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }

}
