using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioSource menuMusic;
    public AudioSource battleSelectMusic;

    public AudioSource[] bgm;
    private int currentBGM;
    private bool playingBGM;
    private float remainingTrackTime = 0f;

    public AudioSource[] sfx;


    private void Awake()
    {
        instance = this;

        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        if(playingBGM)
        {
   
            if (bgm[currentBGM].isPlaying == false)
            {
                currentBGM++;
                if(currentBGM >= bgm.Length)
                {
                    currentBGM = 0;
                }

                bgm[currentBGM].Play();

            }
        }

        remainingTrackTime = (bgm[currentBGM].clip.length - bgm[currentBGM].time);
    }

    public void StopMusic()
    {
        menuMusic.Stop();
        battleSelectMusic.Stop();

        foreach(AudioSource track in bgm)
        {
            track.Stop();
        }

        playingBGM = false;
    }

    public void PlayMenuMusic()
    {
        if (!menuMusic.isPlaying)
        {
            StopMusic();
            menuMusic.Play();
        }      
    }

    public void PlayBattleSelectMusic()
    {
        if(!battleSelectMusic.isPlaying)
        {
            StopMusic();
            battleSelectMusic.Play();
        }
    }

    public void PlayBGM()
    {
        StopMusic();

        currentBGM = Random.Range(0, bgm.Length);

        bgm[currentBGM].Play();
        playingBGM = true;

    }

    public void PlaySFX(int toPlay)
    {
        sfx[toPlay].Stop();
        sfx[toPlay].Play();

    }
}
