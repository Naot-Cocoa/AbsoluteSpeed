using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider),typeof(AudioSource))]
public class FFBZone : MonoBehaviour
{
    private FFB mFFB = null;
    private AudioSource mSource;
    [SerializeField]
    private int mPower;
    [SerializeField]
    private float mTime;

    private void Awake()
    {
        mSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        mFFB = other.GetComponent<FFB>();
        if (mFFB == null) return;
        print("動かす");
        mFFB.Vibration(mPower, mTime);
        if (mSource.clip != null) mSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (mFFB == null) return;
        print("止める");
        mFFB.Vibration(0,0f);
        mFFB = null;
        if (mSource.clip != null) mSource.Stop();
    }
}
