using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRideRay
{

    private readonly Transform mTransform;
    /// <summary>レイを飛ばし始める場所の座標 </summary>
    public Vector3 FromPos { get { return RayMethod.MakeFrom(mTransform, mRayConfig.CenterOffset); } }
    /// <summary>前下方向</summary>
    private Vector3 ToFrontDown { get { return (-mTransform.up + mTransform.forward * mRayConfig.FBRayAngle) / (1f + mRayConfig.FBRayAngle); } }
    /// <summary>後ろ下方向</summary>
    private Vector3 ToBackDown { get { return (-mTransform.up - mTransform.forward * mRayConfig.FBRayAngle) / (1f + mRayConfig.FBRayAngle); } }
    /// <summary>右下方向</summary>
    private Vector3 ToRightDown { get { return (-mTransform.up * mRayConfig.LRRayAngle + mTransform.right) / (1f + mRayConfig.LRRayAngle); } }
    /// <summary>左下方向</summary>
    private Vector3 ToLeftDown { get { return (-mTransform.up * mRayConfig.LRRayAngle - mTransform.right) / (1f + mRayConfig.LRRayAngle); } }
    private readonly RayConfig.AirRideRayConfig mRayConfig;
    public AirRideRay(Transform transform, RayConfig.AirRideRayConfig airRideRay) { mTransform = transform; mRayConfig = airRideRay; }
    public bool FrontDownRay(out RaycastHit hitInfo) { return Physics.Raycast(FromPos, ToFrontDown, out hitInfo, mRayConfig.FBRayLength); }
    public bool BackDownRay(out RaycastHit hitInfo) { return Physics.Raycast(FromPos, ToBackDown, out hitInfo, mRayConfig.FBRayLength); }
    public bool LeftDownRay(out RaycastHit hitInfo) { return Physics.Raycast(FromPos, ToLeftDown, out hitInfo, mRayConfig.LRRayLength); }
    public bool RightDownRay(out RaycastHit hitInfo) { return Physics.Raycast(FromPos, ToRightDown, out hitInfo, mRayConfig.LRRayLength); }
    public bool UnderBoxCast(out RaycastHit hitInfo)
    {
        Vector3 harfSize = mRayConfig.BoxSize / 2.0f;
        return Physics.BoxCast(FromPos, harfSize, -mTransform.up, out hitInfo, mTransform.rotation, mRayConfig.UnderRayLength);
    }

    /// <summary>
    /// 浮遊用boxcastのｷﾞｽﾞﾓ
    /// </summary>
    public void GizmosBoxcast()
    {
        RaycastHit hitInfo;
        UnderBoxCast(out hitInfo);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((FromPos) + (-mTransform.up * hitInfo.distance), mRayConfig.BoxSize);
    }

    /// <summary>
    /// 4方向レイのｷﾞｽﾞﾓ
    /// </summary>
    public void GizmosFourRay()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(FromPos, ToFrontDown * mRayConfig.FBRayLength);
        Gizmos.DrawRay(FromPos, ToBackDown * mRayConfig.FBRayLength);
        Gizmos.DrawRay(FromPos, ToLeftDown * mRayConfig.LRRayLength);
        Gizmos.DrawRay(FromPos, ToRightDown * mRayConfig.LRRayLength);
    }

    /// <summary>
    /// 下方向レイのｷﾞｽﾞﾓ
    /// </summary>
    public void GizmosFloat()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(FromPos, -mTransform.up * mRayConfig.UnderRayLength);
    }
}