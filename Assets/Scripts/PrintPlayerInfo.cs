using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrintPlayerInfo : MonoBehaviour
{
    [Header("Imprimir rotacion")]
    [SerializeField] private TextMeshPro textMeshToPrint;
    [SerializeField] private Transform rotacionY, rotacionX;

    [Header("Imprimir vida")]
    [SerializeField] private Image lifeImage;
    [SerializeField] private ParticleSystem psCannonDamage;


    private void Start()
    {
        PlayerLifeController.Instance.OnLifeChanged += PlayerLifeController_OnLifeChanged;
    }

    void Update()
    {
        textMeshToPrint.text = string.Format("Rotación\nEje Y: {0}\nEje X: {1}",
            Mathf.Round(rotacionY.localEulerAngles.y - 180 > 0 ? rotacionY.localEulerAngles.y - 180 : rotacionY.localEulerAngles.y + 180),
            Mathf.Round(rotacionX.eulerAngles.x));
    }

    private void PlayerLifeController_OnLifeChanged(object sender, System.EventArgs e)
    {
        float life = PlayerLifeController.Instance.GetLife();

        lifeImage.fillAmount = life;

        Color c = Color.HSVToRGB((life * 111f) / 360f, 1, 1);

        lifeImage.color = c;

        float emisionParticles = Mathf.Lerp(12, 0, life);
        ParticleSystem.EmissionModule emModule = psCannonDamage.emission;
        emModule.rateOverTime = emisionParticles;
    }
}
