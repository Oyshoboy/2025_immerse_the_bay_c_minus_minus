/*
Attach to any object.
Set clips, songIndex, timeoutDuration.
Call OnTrigger() to unmute and reset timer.
Toggle 'play' to start or stop playback.
*/

using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GodmodeController : MonoBehaviour
{
    public AudioClip[] clips;
    public int songIndex;
    public float timeoutDuration;
    public bool play;
    
    private AudioSource audioSource;
    private float timer;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }
    
    public void OnTrigger()
    {
        TriggerTheMusic();
    }

    private void TriggerTheMusic()
    {
        timer = timeoutDuration;
        audioSource.mute = false;
    }

    public void TriggerMusicExternally(){
        TriggerTheMusic();
    }

    void Update()
    {
        if (play && (audioSource.clip != clips[songIndex] || !audioSource.isPlaying))
        {
            audioSource.clip = clips[songIndex];
            audioSource.Play();
        }
        
        if (!play && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        if (audioSource.isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0 && !audioSource.mute)
            {
                audioSource.mute = true;
            }
        }
    }
}
