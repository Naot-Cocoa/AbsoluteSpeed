using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/WindSoundParam", fileName = "WindSoundParam")]
public class WindSoundParam : BaseSoundParam
{
    [SerializeField,Header("風切り音の音量を絞る割合"), Range(0f, 1f)]
    private float WindRushVolumeRate = 0.005f;
    [SerializeField,Header("風切り音のピッチを絞る割合"), Range(0f, 1f)]
    private float WindRushPitchRate = 0.003f;

    public override void SoundUpdate(CarSoundParam carSoundParam)
    {
        if (!mIsPlay) return;
        mSource.volume = VolumeCurve.Evaluate(carSoundParam.VelocityInput * WindRushVolumeRate);
        mSource.pitch = 1f + PitchCurve.Evaluate(carSoundParam.VelocityInput * WindRushPitchRate);
    }
}
