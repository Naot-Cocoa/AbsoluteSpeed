using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFollow
{

    /// <summary>機体のtransform</summary>
    private readonly Transform mTransform;
    private readonly AirRideRay mAirRideRay;
    private readonly VehicleSettings.AirRideSettings mAirRideSettings;

    public GroundFollow(Transform transform, AirRideRay airRideRay, VehicleSettings.AirRideSettings airRideSettings)
    {
        mTransform = transform;
        mAirRideRay = airRideRay;
        mAirRideSettings = airRideSettings;
    }

    /// <summary>
    /// 地面の傾斜に合わせて機体を傾ける処理
    /// </summary>
    public void FollowGround()
    {

        float xDiff = MakeXDiff();
        float zDiff = MakeZDiff();
        mTransform.Rotate(xDiff * mAirRideSettings.GroundFollowSpeed, 0.0f, 0.0f);
        //機体の傾斜を修正する速度。割合
        mTransform.Rotate(0.0f, 0.0f, zDiff * mAirRideSettings.GroundFollowSpeed);
    }

    public void DrawGizmos()
    {
        mAirRideRay.GizmosFourRay();
        mAirRideRay.GizmosFloat();
        mAirRideRay.GizmosBoxcast();
    }

    /// <summary>
    /// 前後レイの長さの差
    /// </summary>
    /// <returns></returns>
    private float MakeXDiff()
    {
        RaycastHit frontDownHitInfo;
        RaycastHit backDownHitInfo;
        bool frontDownRayIsHit = mAirRideRay.FrontDownRay(out frontDownHitInfo);
        bool backDownRayIsHit = mAirRideRay.BackDownRay(out backDownHitInfo);

        float frontDownDis = frontDownHitInfo.distance;
        float backDownDis = backDownHitInfo.distance;
        /*後輪か前輪のどちらかが浮いた場合
         どのくらい倒すか
         後輪が浮いた場合、値が大きければ大きいほど
         前方に倒れる*/
        float tiltRate = 1.3f;
        //前レイと後ろレイの長さの差
        if (!frontDownRayIsHit)
        {
            if (!backDownRayIsHit) { return 0.0f; }
            frontDownDis = backDownDis * tiltRate;
        }
        if (!backDownRayIsHit)
        {
            if (!frontDownRayIsHit) { return 0.0f; }
            backDownDis = frontDownDis * tiltRate;
        }
        float frontBackDiff = frontDownDis - backDownDis;
        return frontBackDiff;
    }

    /// <summary>
    /// 左右レイの長さの差
    /// </summary>
    /// <returns></returns>
    private float MakeZDiff()
    {
        RaycastHit leftDownHitInfo;
        RaycastHit rightDownHitInfo;
        bool leftDownRayIsHit = mAirRideRay.LeftDownRay(out leftDownHitInfo);
        bool rightDownRayIsHit = mAirRideRay.RightDownRay(out rightDownHitInfo);

        if (!leftDownRayIsHit || !rightDownRayIsHit) { return 0.0f; }
        float leftRightDiff = leftDownHitInfo.distance - rightDownHitInfo.distance;
        return leftRightDiff;
    }

}