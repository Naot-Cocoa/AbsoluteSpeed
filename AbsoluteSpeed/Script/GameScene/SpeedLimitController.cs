using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最高速度を決める処理
/// </summary>
public class SpeedLimitController
{
    /// <summary>
    /// アクセルの踏み具合で最高速度を調節する処理
    /// ■engineSpeed:速度
    /// ■accel:アクセルのインプット
    /// ■maxSpeed:最高速度
    /// ■engineBrake:エンジンブレーキの強さ
    /// </summary>
    /// <param name="engineSpeed"></param>
    /// <param name="accel"></param>
    /// <param name="maxEngineSpeed"></param>
    /// <returns></returns>
    public static float EngineBrake(float engineSpeed, float accel, float maxEngineSpeed, float engineBrake)
    {
        float absMax = Mathf.Abs(maxEngineSpeed);
        float sign = Mathf.Sign(engineSpeed);
        float absSpeed = Mathf.Abs(engineSpeed);
        accel = Mathf.Clamp01(accel);
        //アクセルの踏み具合で回転数の限界を決める
        float accelEngineLimit = accel * absMax;
        //回転数がペダルの踏み具合に対して多い場合、逓減させる処理
        if (absSpeed > accelEngineLimit)
        {
            absSpeed -= Time.deltaTime * engineBrake;
            absSpeed = Mathf.Clamp(absSpeed, accelEngineLimit, Mathf.Infinity);
        }
        return absSpeed * sign;
    }

 

    /// <summary>
    /// ギアを下げられたとき速度をゆっくり下げる処理
    /// ■maxSpeed:ギアで設定された最高速度
    /// ■preMaxSpeed:一つ前のギアで設定された最高速度
    /// ■currentMaxSpeed:変動させる現在の最高速度
    /// ■decreaseSpeed:逓減させる速度。/s。
    /// </summary>
    public static float DecreaseMaxSpeed(float maxSpeed, float preMaxSpeed, float currentMaxSpeed, float decreaseSpeed)
    {
        //ギアが上げられた場合
        if (maxSpeed > preMaxSpeed) { return maxSpeed;}
        //ギアが下げられた場合上限を下げて返す
        return Mathf.Lerp(currentMaxSpeed, maxSpeed,decreaseSpeed);
    }

}