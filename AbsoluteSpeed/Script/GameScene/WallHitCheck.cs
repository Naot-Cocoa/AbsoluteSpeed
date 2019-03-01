using UnityEngine;

public class WallHitCheck
{
    private bool mLIsHit;
    private LRRay mLRRay;
    private RaycastHit hitInfo;

    public bool LHit { get { return mLRRay.LBoxRay(out hitInfo); } }

    public bool RHit { get { return mLRRay.RBoxRay(out hitInfo); } }
    /// <summary>左右どちらかのレイが当たった場合true</summary>
    public bool LOrRHit { get { return LHit || RHit; } }

    public WallHitCheck(Transform transform,RayConfig.LRRayConfig rayConfig) { mLRRay = new LRRay(transform,rayConfig); }
}
/*ぶつかった相手を取得しないと
 AICarが出てきた際に攻撃できない*/