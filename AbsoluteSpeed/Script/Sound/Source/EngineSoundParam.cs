using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EngineSoundParam",fileName = "EngineSoundParam")]
public class EngineSoundParam : BaseSoundParam
{
    [Tooltip("アニメーションカーブの始まりの時のRPM")]
    public float InRPM;
    [Tooltip("アニメーションカーブの末の時のRPM")]
    public float OutRPM;

    /// <summary>
    /// 音の更新
    /// 回転数に応じて音量ピッチを変更する
    /// </summary>
    /// <param name="carSoundParam"></param>
    public override void SoundUpdate(CarSoundParam carSoundParam)
    {
        if (!mIsPlay) return;
        if(carSoundParam.RpmInput < InRPM || carSoundParam.RpmInput > OutRPM)
        {
            mSource.volume = Mathf.Clamp01(Mathf.Lerp(mSource.volume, 0f, 0.8f));
            mSource.pitch = Mathf.Lerp(mSource.pitch, 1f, 0.5f);
            return;
        }
        var rpmRate = (carSoundParam.RpmInput - InRPM) / (OutRPM - InRPM);
        if (rpmRate >= 1f) rpmRate = 1f;
        else if (rpmRate <= 0f) rpmRate = 0f;
        var ratio = VolumeCurve.Evaluate(rpmRate);
        mSource.volume = ratio;
        ratio = PitchCurve.Evaluate(rpmRate);
        mSource.pitch = 1f + ratio;
    }
}
    
