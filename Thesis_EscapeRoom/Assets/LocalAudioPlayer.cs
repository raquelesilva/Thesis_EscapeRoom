using CoreSystems.Extensions.Attributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class LocalAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip audioClips;
    [SerializeField] private bool is3D = true;
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [Header("Pitch")]
    [SerializeField] private bool applyPichVariance;
    [SerializeField, Disable("applyPichVariance")] private Vector2 pichVariance = new Vector2(1, 1); 

    private AudioSource audioSource;

    public void Start()
    {
        if (audioSource == null)
        {
            audioSource = this.AddComponent<AudioSource>(); 
        }

        audioSource.volume = volume;
        audioSource.spatialBlend = is3D ? 1 : 0;
        if(audioMixer != null)
        {
            audioSource.outputAudioMixerGroup = audioMixer;
        }
    }

    public void Play()
    {
        //int randomIndex = Random.Range(0, audioClips.Count + 1);
        audioSource.clip = audioClips;

        if (applyPichVariance)
        {
            float newPitch = Random.Range(pichVariance.x, pichVariance.y);
            audioSource.pitch = newPitch;
        }
        audioSource.Play();
    }

    public void PlayIfDone()
    {
        if (audioSource.isPlaying) return;

        //int randomIndex = Random.Range(0, audioClips.Count);
        audioSource.clip = audioClips;

        if (applyPichVariance)
        {
            float newPitch = Random.Range(pichVariance.x, pichVariance.y);
            audioSource.pitch = newPitch;
        }
        audioSource.Play();
    }
}
