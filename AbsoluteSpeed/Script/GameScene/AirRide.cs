using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 姿勢制御クラス
/// </summary>
public class AirRide
{
    /*■■■PUBLIC■■■*/

    /// <summary>接地判定 </summary>
    public bool IsGround { get { return mBodyFloat.IsGround; } }

    private Transform mTransform;
    private GroundFollow mGroundFollow;
    private BodyFloat  mBodyFloat;

    public AirRide(Transform transform, RayConfig.AirRideRayConfig rayConfig,VehicleSettings.AirRideSettings airRideSettings)
    {
        mTransform = transform;
        AirRideRay airRideRay = new AirRideRay(transform, rayConfig);
        mGroundFollow = new GroundFollow( transform,airRideRay,airRideSettings);
        mBodyFloat = new BodyFloat(transform,airRideRay,airRideSettings);
    }

    /// <summary>
    /// 姿勢制御処理
    /// ■velocity:Rigidbodyのvelocity
    /// </summary>
    public void FixBalance(Vector3 velocity)
    {
        //地面の傾斜にあわせて車体を傾ける処理
        mGroundFollow.FollowGround();
        //浮遊処理
        mBodyFloat.FloatBody();
        //落下中前方に回転する処理
        RotateForward(velocity);
    }

    /// <summary>
    /// ｷﾞｽﾞﾓ処理
    /// </summary>
    public void DrawGizmos()
    {
        mBodyFloat.GizmosFloatHeight();
        mGroundFollow.DrawGizmos();

    }
    /*■■■PUBLIC■■■*/


    /// <summary>
    /// 落下中前方方向に回転する処理
    /// </summary>
    /// <param name="velocity"></param>
    private void RotateForward(Vector3 velocity)
    {
        //接地中なら返す
        if (IsGround) { return; }
        //落下中でない場合返す
        if (velocity.y >= 0.5f) { return; }
        Vector3 eulerAngles = mTransform.rotation.eulerAngles;
        eulerAngles.x = 30.0f;
        Quaternion fixRot = Quaternion.Euler(eulerAngles);
        mTransform.rotation = Quaternion.Slerp(mTransform.rotation, fixRot, 1.0f * Time.deltaTime);
    }
}