using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{

    public AudioSource source;

    public AudioClip Click;
    public AudioClip Scale;
    public AudioClip ChangeFallDirection;

    public AudioClip FirstSelect;
    public AudioClip SelectInvalid;
    public AudioClip SelectValid;

    public AudioClip ControllerUnlock;

    public AudioClip Swap;
    public AudioClip Unswap;

    public AudioClip DoopDoopDoop;

    public List<AudioClip> FallSounds;

    public List<AudioClip> CelebrationSounds;


    public void PlayCelebrationSound()
    {
        PlaySound(CelebrationSounds[Random.Range(0, CelebrationSounds.Count)]);
    }

    public void PlayDoopDoop(int pitch, out float clipLen)
    {
        AudioClip t = DoopDoopDoop;

        source.pitch = (pitch * 0.1f) + 1;
        source.PlayOneShot(t);

        clipLen = t.length;
    }

    public void OnClick()
    {
        PlaySound(Click);
    }

    public void PlaySound(AudioClip ac)
    {
        source.PlayOneShot(ac);
    }


}
