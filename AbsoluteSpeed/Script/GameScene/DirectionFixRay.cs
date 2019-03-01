using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>進行方向修正に使用するレイクラス</summary>
public class DirectionFixRay
{
    private readonly Transform mTransform;
    private Vector3 mRFTo { get { return (mTransform.forward * 2.0f + mTransform.right) / 3.0f; } }
    private Vector3 mLFTo { get { return (mTransform.forward * 2.0f + -mTransform.right) / 3.0f; } }
    private RayConfig.DirectionFixRayConfig mRayConfig;
    public DirectionFixRay(Transform transform, RayConfig.DirectionFixRayConfig conf) { mTransform = transform; mRayConfig = conf; }

    public bool RFRaycast(out RaycastHit hitInfo)
    {
        Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.RfOffset);
        return Physics.Raycast(from, mRFTo, out hitInfo, mRayConfig.LRLength);
    }

    public bool LFRaycast(out RaycastHit hitInfo)
    {
        Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.LfOffset);
        return Physics.Raycast(from, mLFTo, out hitInfo, mRayConfig.LRLength);
    }

    public bool ForwardBoxCast(out RaycastHit hitInfo)
    {
        Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.ForwardOffset);
        Vector3 harf = mRayConfig.BoxSize / 2.0f;

        return Physics.BoxCast(from,harf, mTransform.forward, out hitInfo, mTransform.rotation, mRayConfig.ForwardBoxcastLength);
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(RayMethod.MakeFrom(mTransform, mRayConfig.RfOffset), mRFTo * mRayConfig.LRLength);
        Gizmos.DrawRay(RayMethod.MakeFrom(mTransform, mRayConfig.LfOffset), mLFTo * mRayConfig.LRLength);
        GizmosBoxcast();
    }

    public void GizmosBoxcast()
    {
        RaycastHit hitInfo;
        ForwardBoxCast(out hitInfo);
        Gizmos.color = Color.cyan;
        Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.ForwardOffset);
        float dis = (hitInfo.distance <= 0f ? mRayConfig.ForwardBoxcastLength : hitInfo.distance);

        Gizmos.DrawWireCube((from) + (mTransform.forward * dis), mRayConfig.BoxSize);
    }
}
