using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    [SerializeField]
    private AudioSource mAudioSource;
    [SerializeField]
    private AudioClip mAudioClip;

    public void Play()
    {
        PlaySE.Instance.PlayAnimEventSE(mAudioSource, mAudioClip);
    }
}
