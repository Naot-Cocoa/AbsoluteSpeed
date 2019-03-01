using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/CollisionScrapeSoundParam", fileName = "CollisionScrapeSoundParam")]
public class CollisionScrapeSoundParam : BaseSoundParam
{
    [SerializeField,Header("衝突し続けている時の音の音量を絞る割合"), Range(0f, 1f)]
    private float CollisionScrapeVolumeRate = 0.1f;
    [SerializeField,Header("衝突し続けている時の音のピッチを絞る割合"), Range(0f, 1f)]
    private float CollisionScrapePitchRate = 0.05f;

    public override void Init(Transform parent)
    {
        if (!mIsPlay) return;
        mTransform = parent;
        var go = new GameObject(Clip.name);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        mSource = go.AddComponent<AudioSource>();
        mSource.clip = Clip;
        mSource.volume = 0f;
        mSource.pitch = 1f;
    }

    public override void Init()
    {
        if (!mIsPlay) return;
        Stop();
        mSource.volume = 0f;
        mSource.pitch = 1f;
    }

    public override void SoundUpdate(CarSoundParam carSoundParam)
    {
        if (!mIsPlay) return;
        if (carSoundParam.HitContinueVelocity <= 0f)
        {
            mSource.volume = 0f;
            mSource.pitch = 1f;
            if (mIsPlay) Stop();
            return;
        }
        mSource.volume = VolumeCurve.Evaluate(Mathf.Abs(carSoundParam.HitContinueVelocity) * CollisionScrapeVolumeRate);
        mSource.pitch = PitchCurve.Evaluate(Mathf.Abs(carSoundParam.HitContinueVelocity * CollisionScrapePitchRate));
        if (!mIsPlay) Play();
    }

    private void Play()
    {
        mSource.Play();
        mIsPlay = true;
    }

    private void Stop()
    {
        mSource.Stop();
        mIsPlay = false;
    }
}
