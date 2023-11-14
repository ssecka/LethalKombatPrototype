using System;
using Unity.VisualScripting;
using UnityEngine;


public class SoundEffects : MonoBehaviour
{
    public AudioSource AudioSrc;
    public AudioClip Jab, JabHit, Kick, KickHit, Block;
    private bool _checkForNull;

    public void Awake()
    {
        AudioSrc = GameObject.Find("AudioSrc").GetComponent<AudioSource>();
    }

    private void InitSource()
    {
        AudioSrc = GameObject.Find("AudioSrc").GetComponent<AudioSource>();
        _checkForNull = AudioSrc == null;
    }

    public void PlayJabSound(bool hit)
    {
        if (_checkForNull) InitSource();
        AudioSrc.clip = hit ? JabHit : Jab;
        AudioSrc.Play();
    }

    public void PlayKickSound(bool hit)
    {
        if (_checkForNull) InitSource();
        AudioSrc.clip = hit ? KickHit : Kick;
        AudioSrc.Play();
    }

    public void PlayBlockSound()
    {
        if (_checkForNull) InitSource();
        AudioSrc.clip = Block;
        AudioSrc.Play();
    }
}