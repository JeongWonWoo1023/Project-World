using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public SoundVolum master;
    public SoundVolum bgm;
    public SoundVolum soundEffect;
    public SoundMod soundMod;
    public AudioMixer mixer;
    public AudioSource bgmSource;
    public AudioSource soundEffectSource;

    public void ChangeBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
