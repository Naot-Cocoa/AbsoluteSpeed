using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VehicleMove
{
    /// <summary>自車のRigidbody</summary>
    private readonly Rigidbody mRigidbody;
    /// <summary>自車のTransform</summary>
    private readonly Transform mTransform;
    /// <summary>ドリフト処理</summary>
    private DriftScript mDriftScript;
    /// <summary>ステアリング処理</summary>
    private readonly SteeringScript mSteeringScript;
    /// <summary>車の滑り具合の値配列 </summary>
    private readonly WheelParams mWheelParams;
    /// <summary>現在の車速</summary>
    private float mSpeed;
    /// <summary>1フレーム前のベロシティ</summary>
    private Vector3 mPreFrameVel = Vector3.zero;
    /// <summary>車のタイヤステート</summary>
    private WheelState mWheelState = WheelState.GRIP;
    /*■■■PROPERTY■■■*/
    public float GetSpeed { get { return mSpeed; } }
    public WheelState GetWheelState{ get { return mWheelState; }}
    /*■■■PROPERTY■■■*/

    /// <summary>
    /// ■rigidbody:自分の車のRigidbody
    /// ■transform:自分の車のtrasnform
    /// ■wheelParams:タイヤの各状態時の滑り具合
    /// </summary>
    /// <param name="rigidbody"></param>
    public VehicleMove(Rigidbody rigidbody, Transform transform, WheelParams wheelParams,
                       VehicleSettings.DriftSettings driftSettings,VehicleSettings.SteerSensitivities steerSensitivities)
    {
        mRigidbody = rigidbody;
        mTransform = transform;
        mWheelParams = wheelParams;
        mSteeringScript = new SteeringScript(steerSensitivities);
        mDriftScript = new DriftScript(driftSettings);
    }

    /// <summary>
    /// 車を操縦する処理
    /// ■enigneRot:エンジン回転数
    /// ■steer:ハンドルからの入力
    /// </summary>
    /// <param name="engineRot"></param>
    /// <param name="steer"></param>
    public void UpdateMove(float engineRot, float steer, float brake, bool isGround)
    {
        if (isGround)
        {
            //前後処理
            UpdateBaseMove(mTransform.forward, engineRot);
            //旋回処理
            mTransform.rotation *= mSteeringScript.CalcAddRotValue(steer, mSpeed, mWheelState);
            //タイヤの状態をドリフト状態に変える処理
            mWheelState = mDriftScript.ChangeState(mSpeed, steer, brake, mWheelState);
            mWheelState = mDriftScript.CancelDrift(mTransform.forward,mRigidbody.velocity,mWheelState);
        }
        else
        {
            //重力処理
            mRigidbody.velocity = RigidbodyCalc.UpdateGravity(mRigidbody.velocity);
        }
    }

    /// <summary>
    /// 車を操縦する処理
    /// ■forward：車の正面方向ベクトル
    /// ■engineRot：エンジン回転数(速度)
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="engineRot"></param>
    private void UpdateBaseMove(Vector3 forward, float engineRot)
    {
        //符号を取得
        //float sign = Mathf.Sign(engineRot);
        //正面方向に進むベクトルを生成
        Vector3 forwardVel = forward;
        mPreFrameVel = mPreFrameVel.normalized;
        mSpeed = RigidbodyCalc.GetFloatVelocity(mRigidbody.velocity);
        Vector3 vel = Vector3.Lerp(mPreFrameVel, forwardVel, mWheelParams.GetGripCoef(mSpeed, mWheelState));
        mRigidbody.velocity = vel * engineRot;
        mPreFrameVel = mRigidbody.velocity;
    }

    public string OnGUITexts()
    {
        string text = string.Format("\twheel coef{0}\n", mWheelParams.GetGripCoef(mSpeed, mWheelState));
        text += string.Format("\twheel state{0}\n", mWheelState);
        text += string.Format("\tgravity{0}\n", mRigidbody.velocity.y);
        return text;
    }
}