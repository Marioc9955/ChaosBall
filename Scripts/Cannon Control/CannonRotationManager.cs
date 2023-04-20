using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonRotationManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SENS_VERT = "SensVert";
    private const string PLAYER_PREFS_SENS_HOR = "SensHort";
    public static CannonRotationManager Instance { get; private set; }

    [SerializeField] private CannonRotation rotacionHorizontal;
    [SerializeField] private CannonRotation rotacionVertical;

    [SerializeField] private float minRotationFactorHor, minRotationFactorVert;
    [SerializeField] private float maxRotationFactorHor, maxRotationFactorVert;

    private float sensVert, sensHor;

    private void Awake()
    {
        Instance = this;
        sensHor= PlayerPrefs.GetFloat(PLAYER_PREFS_SENS_HOR,0.5f);
        sensVert=PlayerPrefs.GetFloat(PLAYER_PREFS_SENS_VERT,0.5f);
        AplicarSensHor(sensHor);
        AplicarSensVer(sensVert);
    }

    public void RotarConJoystick(Vector2 dir)
    {
        rotacionHorizontal.RotacionHorizontal(dir.x*2);
        rotacionVertical.RotacionVertical(dir.y*2);
    }

    public void ChangeSensibilityVertical(float sensibility)
    {
        sensVert = sensibility;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SENS_VERT, sensVert);
        AplicarSensVer(sensibility);
    }

    public void ChangeSensibilityHorizontal(float sensibility)
    {
        sensHor = sensibility;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SENS_HOR, sensHor);
        AplicarSensHor(sensibility);
    }

    private void AplicarSensHor(float sensibility)
    {
        float rotFactor = Mathf.Lerp(minRotationFactorHor, maxRotationFactorHor, sensibility);
        rotacionHorizontal.RotationFactor = rotFactor;
    }

    private void AplicarSensVer(float sensibility)
    {
        float rotFactor = Mathf.Lerp(minRotationFactorVert, maxRotationFactorVert, sensibility);
        rotacionVertical.RotationFactor = rotFactor;
    }

    public float GetSensVert()
    {
        return sensVert;
    }

    public float GetSensHor()
    {
        return sensHor;
    }
}
