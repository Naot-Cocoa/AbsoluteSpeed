using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/CollisionSoundParam", fileName = "CollisionSoundParam")]
public class CollisionSoundParam : BaseSoundParam
{
    [SerializeField,Header("衝突音の音量を絞る割合"), Range(0f, 1f)]
    public float CollisionVolumeRate = 0.02f;

    [SerializeField, Header("衝突音のピッチを絞る割合"), Range(0f, 1f)]
    public float CollisionPitchRate = 0.02f;

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
        if (carSoundParam.HitVelocity <= 0f)
        {
            mSource.volume = 0f;
            mSource.pitch = 1f;
            if (mIsPlay) Stop();
            return;
        }
        mSource.volume = VolumeCurve.Evaluate(Mathf.Abs(carSoundParam.VelocityInput) * CollisionVolumeRate);
        mSource.pitch = PitchCurve.Evaluate(Mathf.Abs(carSoundParam.VelocityInput) * CollisionPitchRate);
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
