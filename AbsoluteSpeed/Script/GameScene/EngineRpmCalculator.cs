using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;


/// <summary>
/// 表示用エンジン回転数の計算クラス
/// </summary>
public class EngineRpmCalculator
{
    /// <summary>表示用エンジン回転数の最大値</summary>
    private const float MAX_ENGINE_RPM = 8000.0f;
    /// <summary>ギアで設定された最大エンジン回転数</summary>
    private float mMaxSpeed = 0.0f;
    /// <summary>1フレーム前の最大エンジン回転数</summary>
    private float mMaxPreSpeed = 0.0f;
    /// <summary>現在のギア</summary>
    private GearState mGearState;

    //コンストラクタ
    public EngineRpmCalculator(GearParam gearParam, IReadOnlyReactiveProperty<GearState> gearState)
    {
        gearState
            .Buffer(2, 1)
            .Subscribe(x =>
        {
            mMaxSpeed = gearParam.GetGearData(x.Last()).MaxSpeed;
            mGearState = x.Last();
            mMaxPreSpeed = gearParam.GetGearData(x.First()).MaxSpeed;
        });
    }

    /// <summary>
    /// エンジン回転数を計算する処理
    /// ■speed:車の速度
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float CalcEngineRpm(float speed)
    {

        float max = 0.0f;
        if(mGearState == GearState.NEUTRAL) { max = mMaxPreSpeed; }
        else { max = mMaxSpeed; }
        //現在の速度と現在の最大回転数の比を出す
        //float rate = speed * GetCoeff(mGearState) / mMaxSpeed;
        float rate = speed / max;

        if (mGearState == GearState.NEUTRAL) { rate = 0.0f; }
        //比から表示用の回転数を求める
        //float rpm = rate * GetInnerMaxRot(mGearState);
        float rpm = rate;
        rpm = Mathf.Clamp(rpm, 0.0f, MAX_ENGINE_RPM);
        return rpm;

    }

    /// <summary>
    /// 表示用のエンジン回転数を微調整したい場合に使用する。
    /// ギア変更時に回転数の上下具合を調節できる。
    /// </summary>
    /// <param name="gearState"></param>
    /// <returns></returns>
    private float GetCoeff(GearState gearState)
    {
        switch (gearState)
        {
            case GearState.FIRST: { return 0.9f; }
            case GearState.SECOND: { return 0.9f; }
            case GearState.THIRD: { return 0.9f; }
            case GearState.FOURTH: { return 0.9f; }
            case GearState.FIFTH: { return 0.9f; }
            case GearState.SIXTH: { return 0.9f; }
            default: return 1.0f;
        }
    }

    /// <summary>
    /// 表示用のエンジン回転数を微調整したい場合に使用する。
    /// GetCoeff()のせいでエンジン回転数が最大まで達しない場合、かける値を大きくすることで
    /// 達するようにする
    /// </summary>
    /// <param name="gearState"></param>
    /// <returns></returns>
    private float GetInnerMaxRot(GearState gearState)
    {
        switch (gearState)
        {
            case GearState.FIRST: { return 9000; }
            case GearState.SECOND: { return 10000; }
            case GearState.THIRD: { return 10000; }
            case GearState.FOURTH: { return 10000; }
            case GearState.FIFTH: { return 10000; }
            case GearState.SIXTH: { return 10000; }
            default: return 10000;
        }
    }
}