using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThankYouPlaySE : MonoBehaviour {

    private AudioPlayer mAudioPlayer;

    private void Awake()
    {
        mAudioPlayer = GetComponent<AudioPlayer>();
    }

    void Start () {
        mAudioPlayer.Play();
	}
}
