using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoFinalText, finalScoreText, razonFinal;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
            Time.timeScale = 1.0f;
        });

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();

            string timeSurvived = GameManager.Instance.GetGamePlayingTimer().ToString("F2");
            string totalEnemyTargets = LevelManager.Instance.GetTotalEnemyTargets().ToString();
            string enemyTargetsLeft = LevelManager.Instance.GetEnemyTargetsLeft().ToString();
            string totalEnemies = LevelManager.Instance.GetTotalPlayerTargets().ToString();
            string enemiesKilled= LevelManager.Instance.GetFinalTargetsKilled().ToString();

            infoFinalText.text = "Sobreviviste por: " + timeSurvived+ " segundos.\n"
                +"Protegiste: " + enemyTargetsLeft + " de " + totalEnemyTargets + " zanahorias.\n"
                +"Mataste: " + enemiesKilled + " de " + totalEnemies + " enemigos.\n";

            finalScoreText.text = "Puntuación final: \n" + GameManager.Instance.GetScore();

            razonFinal.text = GameManager.Instance.RazonFinal;
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
