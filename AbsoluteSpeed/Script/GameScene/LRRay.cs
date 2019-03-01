using UnityEngine;

/// <summary>車の左右から照射するレイの管理クラス</summary>
public class LRRay
{
    private readonly Transform mTransform;
    private readonly RayConfig.LRRayConfig mRayConfig;
    private Vector3 RTo { get { return mTransform.right; } }
    private Vector3 LTo { get { return -mTransform.right; } }
    /*■■■PUBLIC■■■*/
    public LRRay(Transform transform, RayConfig.LRRayConfig rayConfig) { mTransform = transform; mRayConfig = rayConfig; }

    //public bool RRay(out RaycastHit hitInfo)
    //{
    //    Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.ROffset);
    //    return Physics.Raycast(from, RTo, out hitInfo, mRayConfig.LrLength);
    //}

    //public bool LRay(out RaycastHit hitInfo)
    //{
    //    Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.LOffset);
    //    return Physics.Raycast(from, LTo, out hitInfo, mRayConfig.LrLength);
    //}

    public bool RBoxRay(out RaycastHit hitInfo)
    {
        Vector3 harfSize = mRayConfig.BoxcastSize / 2.0f;
        Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.CenterOffset);
        return Physics.BoxCast(from, harfSize, RTo, out hitInfo, mTransform.rotation, mRayConfig.LrLength);
    }

    public bool LBoxRay(out RaycastHit hitInfo)
    {
        Vector3 harfSize = mRayConfig.BoxcastSize / 2.0f;
        Vector3 from = RayMethod.MakeFrom(mTransform, mRayConfig.CenterOffset);
        return Physics.BoxCast(from, harfSize, LTo, out hitInfo, mTransform.rotation, mRayConfig.LrLength);
    }

    public void DrawRayGizmos()
    {
        Gizmos.color = Color.red;
        if (mRayConfig == null) { return; }
        //Gizmos.DrawRay(RayMethod.MakeFrom(mTransform, mRayConfig.ROffset), mTransform.right * mRayConfig.LrLength);
        //Gizmos.DrawRay(RayMethod.MakeFrom(mTransform, mRayConfig.LOffset), -mTransform.right * mRayConfig.LrLength);
        RaycastHit hitInfo;
        RBoxRay(out hitInfo);
        Gizmos.DrawWireCube(RayMethod.MakeFrom(mTransform, mRayConfig.CenterOffset) + (RTo * (hitInfo.distance <= 0f ? mRayConfig.LrLength : hitInfo.distance)), mRayConfig.BoxcastSize);
        LBoxRay(out hitInfo);
        Gizmos.DrawWireCube(RayMethod.MakeFrom(mTransform, mRayConfig.CenterOffset) + (LTo * (hitInfo.distance <= 0f ? mRayConfig.LrLength : hitInfo.distance)), mRayConfig.BoxcastSize);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(RayMethod.MakeFrom(mTransform, mRayConfig.LOffset), -mTransform.right * 1.5f);
    }
    /*■■■PUBLIC■■■*/
}
