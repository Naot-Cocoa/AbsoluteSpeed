using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/BrakeSoundParam", fileName = "BrakeSoundParam")]
public class BrakeSoundParam : BaseSoundParam
{
    public override void SoundUpdate(CarSoundParam carSoundParam)
    {
        if (!mIsPlay) return;
        mSource.volume = VolumeCurve.Evaluate(carSoundParam.BrakeInput);
        mSource.pitch = PitchCurve.Evaluate(carSoundParam.BrakeInput);
    }
}
