using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip playerClip;
    public AudioClip aiClip1;
    public AudioClip aiClip2;
    private AudioSource audioSource;
    private static AudioManager instance;
    public static AudioManager Instance=>instance;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance=this;
    }
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
    }

    public void PlayAudio(int currentP)
    {
        if(currentP==1)
            audioSource.clip=playerClip;
        else if(currentP==0)
            audioSource.clip=aiClip1;
        else
            audioSource.clip=aiClip2;
        audioSource.Play();
    }
}
