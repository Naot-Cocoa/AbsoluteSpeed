using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCalculator
{
    /// <summary>前ギアとの差が大きかったとき回転数の上昇量を下げる割合。
    /// 小さいほどあがりづらい</summary>
    private const float RATE = 0.3f;
    /// <summary>
    /// 加算する回転数を計算する処理
    /// ■engineSpeed:速度
    /// ■accPow:アクセルの強さ
    /// ■accel:アクセルの入力
    /// ■needEngineSpeed:ギアチェンジに必要な速度
    /// </summary>
    /// <param name="engineSpeed"></param>
    /// <param name="accel"></param>
    /// <param name="needEngineSpeed"></param>
    /// <returns></returns>
    public static float CalcRotToAdd(float engineSpeed, float accPow, float accel, float needEngineSpeed)
    {
        engineSpeed = Mathf.Abs(engineSpeed);
        needEngineSpeed = Mathf.Abs(engineSpeed);
        //現在のギアとアクセルの踏み具合で加算するエンジン回転数の量を決める
        float accPowPs = accel * (accPow * Time.deltaTime);
        /*現在の回転数とギアチェンジに必要な回転数から増加比率を出す
         ギア1->ギア6に入れられた場合、増加は遅い。
         回転数が上昇するにつれ、増加速度も上昇する*/
        float rateOfUp = 1.0f;//割合の為1で初期化
        if (needEngineSpeed > 0) { rateOfUp = engineSpeed / needEngineSpeed; }
        /*必要回転数を超過した場合、大きい値になってしまう為1を超えないようにする。
          下は小さくなりすぎるため、値を用意する*/
        rateOfUp = Mathf.Clamp(rateOfUp, RATE, 1.0f);//割合の為最大が1
        float rotToAdd = accPowPs * rateOfUp;
        return rotToAdd;
    }
    /// <summary>
    /// ブレーキで落とす速度を算出する処理
    /// ■brake:ブレーキのinput
    /// ■brakePow:ブレーキの強さ
    /// </summary>
    /// <param name="brake"></param>
    /// <param name="engineSpeed"></param>
    /// <returns></returns>
    public static float CalcBrakeAmt(float brake, float brakePow)
    {
        //inputの為
        //brake = Mathf.Clamp01(brake);
        /*velocityを下げてもエンジン回転数は下がらないので
         エンジン回転数を下げて速度を落とす*/
        float brakeAmt = -(brakePow * brake * Time.deltaTime);
        //0より下に下がらないようにする
        return brakeAmt;
    }

}
