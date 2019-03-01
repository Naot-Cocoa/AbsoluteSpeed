using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DriftScript
{
    private float mTimer = 0.0f;
    private readonly VehicleSettings.DriftSettings mDriftSettings;
    public DriftScript(VehicleSettings.DriftSettings driftSettings) { mDriftSettings = driftSettings; }


    /// <summary>
    /// 条件を満たしていた場合ドリフトステートを返す
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="steer"></param>
    /// <param name="brake"></param>
    /// <returns></returns>
    public WheelState ChangeState(float speed, float steer, float brake, WheelState wheelState)
    {
        if (speed < ConvertUnits.KmphToMps(mDriftSettings.DriftSpeed)) { return WheelState.GRIP; }
        if (Mathf.Abs(steer) <  mDriftSettings.IgnoreSteerInput) { return wheelState; }
        //if (brake < mDriftBrakePow) { return wheelState; }
        return WheelState.DRIFT;
    }

    public WheelState CancelDrift(Vector3 forward, Vector3 velocity, WheelState wheelState)
    {
        if (wheelState == WheelState.GRIP)
        {
            mTimer = 0.0f;
            return WheelState.GRIP;
        }
        //車の正面方向と加速している方向をみて(ステアリング0かつ)その差が±何度以下の状態に数秒あったら戻す
        float deg = Vector3.Angle(velocity, forward);
        //指定角度以内か
        if (deg < mDriftSettings.DriftCancelAngle)
        {
            mTimer += Time.deltaTime;
            //指定時間留まったか
            if (mTimer >= mDriftSettings.DriftCancelTime)
            {
                return WheelState.GRIP;
            }
        }
        //留まってない場合やり直す
        else { mTimer = 0.0f; }
        return WheelState.DRIFT;
    }

    public void OnDrawGimos(Transform transform,Rigidbody rigidbody)
    {
        if (!transform) { return; }
        if (!rigidbody) { return; }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position,transform.forward * 20);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rigidbody.velocity);
    }
}
//ブレーキふまなくて良い
//ドリフト解除のタイミング
//行きたい方向と進行方向がx秒重なったとき
//ハンドルをひねる速度