using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamaraCuadrosMainMenu : MonoBehaviour
{
    public enum Cuadro { MainMenu, Creditos, Options };

    public Quaternion rotacionMainMenu, rotacionCredits, rotacionOptions;
    public Vector3 posMainMenu, posCredits, posOptions;

    public Cuadro cuadroActual;

    public float transitionTime;

    public Transform camara;

    Vector3 posActual, posNueva;
    Quaternion rotActual, rotNueva;
    float tiempoInicial;
    bool cambiandoCuadro;

    private void Start()
    {
        cuadroActual = Cuadro.MainMenu;
        cambiandoCuadro = false;
    }

    void MoverCuadro(Cuadro cuadroNuevo)
    {
        cambiandoCuadro = true;
        posActual = GetPosicionFromCuadro(cuadroActual);
        rotActual = GetRotacionFromCuadro(cuadroActual);
        posNueva = GetPosicionFromCuadro(cuadroNuevo);
        rotNueva = GetRotacionFromCuadro(cuadroNuevo);

        tiempoInicial = Time.time;

    }

    void Update()
    {
        if (cambiandoCuadro)
        {
            float t = (Time.time - tiempoInicial) / (transitionTime);
            if (t <= 1)
            {
                Vector3 nuevaPos = Vector3.Lerp(posActual, posNueva, t);
                camara.position = nuevaPos;
                camara.rotation = Quaternion.Lerp(rotActual, rotNueva, t);
            }
            else
            {
                cambiandoCuadro = false;
            }
        }

    }

    public void CambiarCuadroACreditos()
    {
        cuadroActual = Cuadro.Creditos;
        MoverCuadro(cuadroActual);
    }

    public void CambiarCuadroAOptions()
    {
        cuadroActual = Cuadro.Options;
        MoverCuadro(cuadroActual);

    }

    public void CambiarCuadroMainMenu()
    {
        cuadroActual = Cuadro.MainMenu;
        MoverCuadro(cuadroActual);

    }

    Vector3 GetPosicionFromCuadro(Cuadro c)
    {
        switch (c)
        {
            case Cuadro.MainMenu:
                return posMainMenu;
            case Cuadro.Creditos:
                return posCredits;
            case Cuadro.Options:
                return posOptions;
            default:
                return Vector3.zero;
        }

    }

    Quaternion GetRotacionFromCuadro(Cuadro c)
    {
        switch (c)
        {
            case Cuadro.MainMenu:
                return rotacionMainMenu;
            case Cuadro.Creditos:
                return rotacionCredits;
            case Cuadro.Options:
                return rotacionOptions;
            default:
                return Quaternion.identity;
        }

    }
}
