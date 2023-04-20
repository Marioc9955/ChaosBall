using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputChaosBall playerInput;

    public event EventHandler OnFireStarted;
    public event EventHandler OnFireCanceled;
    //public event EventHandler OnPauseAction;

    private void Awake()
    {
        Instance = this;

        playerInput = new InputChaosBall();

        playerInput.Player.Enable();

        playerInput.Player.Fire.started += Fire_started;
        playerInput.Player.Fire.canceled += Fire_canceled;
        playerInput.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInput.Player.Fire.started -= Fire_started;
        playerInput.Player.Fire.canceled -= Fire_canceled;
        playerInput.Player.Pause.performed -= Pause_performed;

        //playerInput.Disable();
        playerInput.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        //OnPauseAction?.Invoke(this, EventArgs.Empty);
        GameManager.Instance.TogglePauseGame();
    }

    private void Fire_canceled(InputAction.CallbackContext obj)
    {
        OnFireCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Fire_started(InputAction.CallbackContext obj)
    {
        OnFireStarted?.Invoke(this, EventArgs.Empty);
    }

    public float GetFireValue()
    {
        return playerInput.Player.Fire.ReadValue<float>();
    }
}
