using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/RoadSoundParam", fileName = "RoadSoundParam")]
public class RoadSoundParam : BaseSoundParam
{
    [SerializeField,Header("道を走っている音の音量を絞る割合"),Range(0f,1f)]
    private float RoadRollVolumeRate = 0.005f;
    [SerializeField,Header("道を走っている音のピッチを絞る割合"), Range(0f, 1f)]
    private float RoadRollPitchRate = 0.003f;

    public override void SoundUpdate(CarSoundParam carSoundParam)
    {
        if (!mIsPlay) return;
        mSource.volume = VolumeCurve.Evaluate(carSoundParam.VelocityInput * RoadRollVolumeRate);
        mSource.pitch = 0.1f + PitchCurve.Evaluate(carSoundParam.VelocityInput * RoadRollPitchRate);
    }
}
