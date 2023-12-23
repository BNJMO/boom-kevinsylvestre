using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioOnLobby : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip buttonAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = clickManager.instance.audioSource;
        buttonAudioClip = clickManager.instance.buttonAudioClip;
    }

    public void MakeSound()
    {
        audioSource.PlayOneShot(buttonAudioClip);
    }
}
