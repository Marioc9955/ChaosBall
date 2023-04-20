using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Toggle toggleMusicMute;
    [SerializeField] private Toggle toggleSFXMute;
    [SerializeField] private Slider sliderCannonRotationHorizontal;
    [SerializeField] private Slider sliderCannonRotationVertical;
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private Slider sliderSFXVolume;


    private void Awake()
    {
        Instance = this;

        sliderCannonRotationHorizontal.onValueChanged.AddListener((value) => {
            CannonRotationManager.Instance.ChangeSensibilityHorizontal(value);
        });
        sliderCannonRotationVertical.onValueChanged.AddListener((value) => {
            CannonRotationManager.Instance.ChangeSensibilityVertical(value);
        });
        toggleMusicMute.onValueChanged.AddListener((value) =>
        {
            MusicManager.Instance.SetMute(value);
            sliderMusicVolume.interactable = !value;
        });
        toggleSFXMute.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetMute(!value);
        });
        sliderMusicVolume.onValueChanged.AddListener((value) => {
            MusicManager.Instance.ChangeVolume(value);
        });
        sliderSFXVolume.onValueChanged.AddListener((value) => {
            SoundManager.Instance.ChangeVolume(value);
        });
    }

    private void Start()
    {
        sliderSFXVolume.value = SoundManager.Instance.GetVolume();
        sliderMusicVolume.value = MusicManager.Instance.GetVolume();
        sliderCannonRotationHorizontal.value = CannonRotationManager.Instance.GetSensHor();
        sliderCannonRotationVertical.value = CannonRotationManager.Instance.GetSensVert();
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
