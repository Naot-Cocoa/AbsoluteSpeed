using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各タイヤの状態の速度nKm時の滑走具合
/// </summary>
[System.Serializable]
public class WheelData
{
    [SerializeField, Tooltip("速度がnKm以下の時")]
    private float mSpeed;
    [SerializeField, Range(0, 1), Tooltip("mSpeed以下の時の通常時のタイヤの滑り具合")]
    private float mGrip;
    [SerializeField, Range(0, 1), Tooltip("mSpeed以下の時のドリフト時のタイヤの滑り具合")]
    private float mDrift;
    [SerializeField, Range(0, 1), Tooltip("mSpeed以下の時の滑っているときのタイヤの滑り具合")]
    private float mSlick;

    /// <summary>速度がnKm以下の時</summary>
    public float Speed { get { return mSpeed * 1000 / 3600; } }
    /// <summary> mSpeed以下の時の通常時のタイヤの滑り具合</summary>
    public float Grip { get { return Mathf.Clamp01(mGrip); } }
    /// <summary>mSpeed以下の時のドリフト時のタイヤの滑り具合</summary>
    public float Drift { get { return Mathf.Clamp01(mDrift); } }
    /// <summary>mSpeed以下の時の滑っているときのタイヤの滑り具合</summary>
    public float Slick { get { return Mathf.Clamp01(mSlick); } }

    /// <summary>
    /// タイヤの状態に合わせた値を返す
    /// </summary>
    /// <param name="wheelState"></param>
    /// <returns></returns>
    public float GetCoef(WheelState wheelState)
    {
        switch (wheelState)
        {
            case WheelState.GRIP: return Grip;
            case WheelState.DRIFT: return Drift;
            case WheelState.SLICK: return Slick;
            default: return 0.0f;
        }
    }
}


