using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Models;
public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer audioMixer;
    public AudioSource musicAudioSource;
    public AudioSource soundAudioSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    private bool musicOn;
    public bool MusicOn
    {
        get
        {
            return musicOn;
        }
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    public bool SoundOn
    {
        get
        {
            return soundOn;
        }
        set
        {
            soundOn = value;
            this.MusicMute(!soundOn);
        }
    }

    private void MusicMute(bool mute)
    {
        this.SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    private void SoundMute(bool mute)
    {
        this.SetVolume("SoundVolume", mute ? 0 : soundVolume);
    }

    private void SetVolume(string name, int value)
    {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);
    }

    void Start()
    {
        this.MusicOn = Config.MusicOn;
        this.SoundOn = Config.SoundOn;
        this.MusicVolume = Config.MusicVolume;
        this.SoundVolume = Config.SoundVolume;
    }

    private int musicVolume;
    private int soundVolume;
    public int SoundVolume
    {
        get
        {
            return soundVolume;
        }
        set
        {
            if(soundVolume != value)
            {
                soundVolume = value;
                if(soundOn) this.SetVolume("SoundVolume", soundVolume);
            }
        }
    }

    public int MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                if (musicOn) this.SetVolume("MusicVolume", musicVolume);
            }
        }
    }

    internal void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if(clip == null)
        {
            Debug.LogWarningFormat("PlayMusic:{0} not existed", name);
            return;
        }
        if(musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    internal void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound:{0} not existed", name);
            return;
        }

        soundAudioSource.PlayOneShot(clip);
    }
}

