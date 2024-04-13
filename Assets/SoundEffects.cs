using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioClip BubblePop;
    public AudioClip SphereShow;
    public AudioClip Absorbing;
    public AudioClip DownTheVortex;
    public AudioClip Wrong;
    public AudioClip Win;
    public AudioClip GameOver;
    List<AudioSource> audioSources;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponents<AudioSource>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBubble()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (freeSource != null)
        {
            freeSource.clip = BubblePop;
            freeSource.pitch = 1;
            freeSource.Play();
        }

    }

    public void PlaySphere()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);

        if (freeSource != null)
        {
            freeSource.pitch = 2;
            freeSource.clip = SphereShow;
            freeSource.Play();
        }
    }

    public void PlayAbsorbing()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (freeSource != null)
        {
            if (freeSource != null)
            {
                freeSource.pitch = 1f;
                freeSource.clip = Absorbing;
                freeSource.Play();
            }
        }
    }

    public void PlayDownTheVortex()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (freeSource != null)
        {
            freeSource.pitch = 0.7f;
            freeSource.clip = DownTheVortex;
            freeSource.Play();
        }
    }

    public void PlayWrong()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (freeSource != null)
        {
            freeSource.pitch = 2f;
            freeSource.clip = Wrong;
            freeSource.Play();
        }
    }
    public void PlayWin()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (freeSource != null)
        {
            freeSource.pitch = 1f;
            freeSource.clip = Win;
            freeSource.Play();
        }
    }

    public void PlayGameOver()
    {
        var freeSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (freeSource != null)
        {
            freeSource.pitch = 1f;
            freeSource.clip = GameOver;
            freeSource.Play();
        }
    }

}
