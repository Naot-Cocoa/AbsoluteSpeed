using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringScript
{
    /// <summary>値以下の速度のとき、ステアリング入力を無視する</summary>
    private const float IGNORE_SPEED = 0.5f;
    /// <summary>方向転換のしやすさ</summary>
    private VehicleSettings.SteerSensitivities mSensitivities;
    public SteeringScript(VehicleSettings.SteerSensitivities steerSensitivities) { mSensitivities = steerSensitivities; }
    /// <summary>
    /// 旋回処理
    /// ■transform:自分の車のTransform
    /// ■steer:ハンドルからの入力
    /// </summary>
    public Quaternion CalcAddRotValue(float steer, float speed, WheelState wheelState)
    {
        //速度が遅い場合旋回を行わない
        if (speed < IGNORE_SPEED) return Quaternion.identity;
        /*steerの値が大きいと回転が速くなります*/
        //transform.Rotate(0f, steer, 0f);
        return Quaternion.Euler(0.0f, steer * mSensitivities.GetValue(wheelState), 0.0f);//SLICK -> 指数関数?
    }

    /// <summary>
    ///　バック走行時、ステアリング入力を反転させる
    /// </summary>
    /// <param name="engineRot"></param>
    /// <param name="steer"></param>
    /// <returns></returns>
    public static float InverseSteer(float engineRot, float steer)
    {
        if (engineRot < 0.0f) { return steer * -1.0f; }
        return steer;
    }

    /// <summary>
    /// 左のステアリング入力を無視する
    /// </summary>
    /// <param name="lHit"></param>
    /// <param name="steer"></param>
    /// <returns></returns>
    public static float IgnoreLSteer(bool lHit, float steer)
    {
        if (lHit) { return Mathf.Clamp(steer, 0.0f, 1.0f); }
        return steer;
    }

    /// <summary>
    ///　右のステアリング入力を無視する
    /// </summary>
    /// <param name="rHit"></param>
    /// <param name="steer"></param>
    /// <returns></returns>
    public static float IgnoreRSteer(bool rHit, float steer)
    {
        if (rHit) { return Mathf.Clamp(steer, -1.0f, 0.0f); }
        return steer;
    }
}


