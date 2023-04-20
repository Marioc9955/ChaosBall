using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    private const string PLAYER_PREFS_SOUND_MUTE = "SoundMute";


    public static SoundManager Instance { get; private set; }



    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    [SerializeField] private AudioSource cannonFireAudioSource;
    [SerializeField] private AudioSource cannonLoadingAudioSource;
    [SerializeField] private AudioSource gameOverAudioSource;


    private float volume = 1f;
    private bool mute;


    private void Awake()
    {
        Instance = this;

        //player prefs int para mute por defecto es cero, si es cero mute es falso
        mute = PlayerPrefs.GetInt(PLAYER_PREFS_SOUND_MUTE, 0) != 0;
        SetMute(mute);

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }


    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (!mute)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
        }
    }

    public void PlayCannonFireSFX(Vector3 position,float volumeMult)
    {
        //PlaySound(audioClipRefsSO.cannonFire, position, volume);
        cannonFireAudioSource.transform.position = position;
        cannonFireAudioSource.volume = this.volume * volumeMult;
        cannonFireAudioSource.Play();
    }

    public void PlayLoadingCannonSFX(Vector3 position, float volumeMult)
    {
        //PlaySound(audioClipRefsSO.loadingCannon, position, volume);
        cannonLoadingAudioSource.transform.position = position;
        cannonLoadingAudioSource.volume =  volume*volumeMult;
        cannonLoadingAudioSource.Play();
    }

    public void PlayGameOverSFX(float volume)
    {
        gameOverAudioSource.volume *= volume;
        gameOverAudioSource.Play();
    }

    public void PlayOuchMaleSFX(Vector3 position)
    {
        PlaySound(audioClipRefsSO.ouchMale, position);
    }
    public void PlayOuchFemaleSFX(Vector3 position)
    {
        PlaySound(audioClipRefsSO.ouchFemale, position);
    }

    public void ChangeVolume(float volume)
    {
        this.volume= volume;
        cannonFireAudioSource.volume= volume;
        cannonLoadingAudioSource.volume= volume;
        gameOverAudioSource.volume= volume;
    }

    public float GetVolume()
    {
        return volume;
    }

    public void SetMute(bool mute)
    {
        this.mute = mute;
        cannonFireAudioSource.mute= mute;
        cannonLoadingAudioSource.mute = mute;
        gameOverAudioSource.mute = mute;
        int muteInt = mute? 1 : 0;
        PlayerPrefs.SetInt(PLAYER_PREFS_SOUND_MUTE, muteInt);
    }
}
