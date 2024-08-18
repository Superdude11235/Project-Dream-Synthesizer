using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource soundFXObject;

    //List of sound effects
    public AudioClip Jump;
    public AudioClip Slide;
    public AudioClip PlayerHurt;
    public AudioClip BowCharge;
    public AudioClip BowFire;
    public AudioClip Sword;
    public AudioClip ItemPickup;
    public AudioClip Checkpoint;
    public AudioClip Death;
    public AudioClip Enemyhurt;

    //Background music
    public AudioSource Background;




  

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    public void StopBackground()
    {
        Background.Stop();
    }
}
