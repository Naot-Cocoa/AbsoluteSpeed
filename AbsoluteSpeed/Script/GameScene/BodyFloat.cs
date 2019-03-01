using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFloat
{
    /// <summary>接地判定 </summary>
    public bool IsGround { get; private set; }


    /// <summary>機体のtransform</summary>
    private readonly Transform mTransform;
    private readonly AirRideRay mAirRideRay;
    private readonly VehicleSettings.AirRideSettings mAirRideSettings;
    public BodyFloat(Transform transform, AirRideRay airRideRay, VehicleSettings.AirRideSettings airRideSettings)
    {
        mTransform = transform;
        mAirRideRay = airRideRay;
        mAirRideSettings = airRideSettings;
    }



    /// <summary>
    /// 接地判定用
    /// </summary>
    /// <returns></returns>
    public void FloatBody()
    {
        RaycastHit hitInfo;
        IsGround = mAirRideRay.UnderBoxCast(out hitInfo);
        //地面から少し浮かす処理
        FixHeight(mAirRideSettings.Height, hitInfo.distance);
    }

    /// <summary>
    /// 車体を少し浮かす処理
    /// ■height:地面からの距離
    /// ■distance:下方向レイの長さ
    /// </summary>
    /// <param name="height"></param>
    /// <param name="distance"></param>
    private void FixHeight(float height, float distance)
    {
        if (!IsGround) { return; }
        //指定の高さとレイの長さの差
        float diff = height - distance;
        //高さを修正する速度。割合
        float fixSpeed = 0.3f;
        Vector3 pos = mTransform.position;
        pos.y = Mathf.Lerp(pos.y, pos.y + diff, fixSpeed);
        mTransform.position = pos;
    }

    public void GizmosFloatHeight()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mAirRideRay.FromPos, -mTransform.up * mAirRideSettings.Height);
    }
}