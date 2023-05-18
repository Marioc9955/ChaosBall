using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLifeController : MonoBehaviour
{
    public event EventHandler OnLifeChanged;

    public static PlayerLifeController Instance { get; private set; }

    private float life = 1.0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    public float GetLife()
    {
        return life;
    }

    [ContextMenu("Herir jugador")]
    public void HurtPlayer()
    {
        life -= .1f;
        OnLifeChanged?.Invoke(this, EventArgs.Empty);
        DetectDead();
    }

    public void HurtPlayer(float damage)
    {
        life -= damage;
        OnLifeChanged?.Invoke(this, EventArgs.Empty);
        DetectDead();
    }

    private void DetectDead()
    {
        if (life <= 0)
        {
            GameManager.Instance.GameOver("Tu cañón quedó destruido");
        }
    } 
}
