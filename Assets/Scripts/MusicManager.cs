using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    private const string PLAYER_PREFS_SOUND_MUTE = "SoundMute";


    public static MusicManager Instance { get; private set; }


    private AudioSource audioSource;
    private float volume = .3f;

    private bool mute;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        audioSource.volume = volume;

        //player prefs int para mute por defecto es cero, si es cero mute es falso
        mute = PlayerPrefs.GetInt(PLAYER_PREFS_SOUND_MUTE, 0) != 0;
        SetMute(mute);
    }

    public void ChangeVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void SetMute(bool mute)
    {
        this.mute= mute;
        audioSource.mute = mute;
        int muteInt = mute ? 1 : 0;
        PlayerPrefs.SetInt(PLAYER_PREFS_SOUND_MUTE, muteInt);
    }

    public float GetVolume()
    {
        return volume;
    }
}
