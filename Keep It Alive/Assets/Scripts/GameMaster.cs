﻿using System.Collections;
using UnityEngine;

/* This are the master class for the game, storing all the important data from start to finish
 * Can only be running once on splash screen
 */

public class GameMaster : MonoBehaviour
{
    // Game Setting :
    float bgmVol;
    public float BgmVol { get { return bgmVol; } set { bgmVol = value; } }
    float sfxVol;
    public float SfxVol { get { return sfxVol; } set { sfxVol = value; } }


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Initial Value for the game :
        BgmVol = 0.7f;
        SfxVol = 1.0f;
    }

    public IEnumerator AudioFadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = BgmVol;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 0;
    }

    public IEnumerator AudioFadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = BgmVol;
        audioSource.Play();
        while (audioSource.volume < BgmVol)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.volume = BgmVol;
    }
}
