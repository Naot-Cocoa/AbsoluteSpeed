using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/SlipSoundParam", fileName = "SlipSoundParam")]
public class SlipSoundParam : BaseSoundParam
{
    [SerializeField,Header("スリップ音が最大になる角度")]
    public float MaxSlipAngle = 180f;

    public override void SoundUpdate(CarSoundParam carSoundParam)
    {
        if (!mIsPlay) return;
        var angle = Vector3.Angle(mTransform.forward, carSoundParam.PreVelocity);
        var rate = Mathf.Clamp01(angle / MaxSlipAngle);
        mSource.volume = VolumeCurve.Evaluate(rate);
        mSource.pitch = PitchCurve.Evaluate(rate);
    }
}
