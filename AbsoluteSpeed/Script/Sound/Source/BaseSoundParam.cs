using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSoundParam : ScriptableObject
{
    [Tooltip("鳴らす音")]
    public AudioClip Clip;
    [Tooltip("音量のアニメーションカーブ")]
    public AnimationCurve VolumeCurve;
    [Tooltip("ピッチのアニメーションカーブ")]
    public AnimationCurve PitchCurve;
    public bool mIsPlay = true;

    protected AudioSource mSource;
    protected Transform mTransform;
    /// <summary>
    /// 最初の初期化
    /// 必要なAudioSourceを生成し設定を行う
    /// </summary>
    /// <param name="parent">親オブジェクトのトランスフォーム</param>
    public virtual void Init(Transform parent)
    {
        if (!mIsPlay) return;
        mTransform = parent;
        var go = new GameObject(Clip.name);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        mSource = go.AddComponent<AudioSource>();
        mSource.clip = Clip;
        mSource.loop = true;
        mSource.volume = 0f;
        mSource.pitch = 1f;
        mSource.Play();
    }


    /// <summary>
    /// 2回目以降の初期化
    /// </summary>
    public virtual void Init()
    {
        if (!mIsPlay) return;
        mSource.Stop();
        mSource.volume = 0f;
        mSource.pitch = 1f;
        mSource.Play();
    }

    public abstract void SoundUpdate(CarSoundParam carSoundParam);

}
