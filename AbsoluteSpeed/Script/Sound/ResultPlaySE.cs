using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPlaySE : MonoBehaviour {

    [SerializeField]
    private AudioSource mAudioSource;

    public void PlaySwiftSE()
    {
        PlaySE.Instance.Play(mAudioSource, ConstAudio.SE_SWIFT_ANIM);
    }

    public void PlayResultSE()
    {
        PlaySE.Instance.Play(mAudioSource, ConstAudio.SE_GOAL);
    }

    public void PlayResult()
    {
        PlaySE.Instance.Play(mAudioSource, ConstAudio.SE_RESULT);
    }
}
