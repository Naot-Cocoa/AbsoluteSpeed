using UnityEngine;

/// <summary>壁衝突回避クラス</summary>
public class DirectionFix
{
    private Transform mTransform;
    private DirectionFixRay mFixRay;
    public DirectionFix(Transform transform, RayConfig.DirectionFixRayConfig conf)
    {
        mTransform = transform;
        mFixRay = new DirectionFixRay(transform, conf);
    }


    //public void FixDirection()
    //{
    //    RaycastHit rFInfo;
    //    RaycastHit lFInfo;
    //    bool rFHit = mFixRay.RFRaycast(out rFInfo);
    //    bool lFHit = mFixRay.LFRaycast(out lFInfo);

    //    if (!rFHit && !lFHit) { return; }
    //    else if (lFHit && !rFHit) { /*Fix(lFInfo.normal);*/Rot(1f, lFInfo.distance); }
    //    else if (rFHit && !lFHit) { /*Fix(rFInfo.normal);*/Rot(-1f, lFInfo.distance); }
    //    else
    //    {
    //        if (rFInfo.distance < lFInfo.distance) { /*Fix(rFInfo.normal);*/Rot(-1f, lFInfo.distance); }
    //        if (rFInfo.distance > lFInfo.distance) { /*Fix(lFInfo.normal);*/Rot(1f, lFInfo.distance); }
    //    }
    //}

    public void FixDirection(float accel, float steer)
    {
        RaycastHit hitInfo;
        bool hit = mFixRay.ForwardBoxCast(out hitInfo);
        if (!hit) { return; }
        if (accel < 0.2f) { return; }
        //衝突している場合
        mTransform.Rotate(0.0f, 0.9f * steer, 0.0f);
    }

    private void Rot(float sign, float dis)
    {
        if (dis <= 0.0f) { return; }
        mTransform.Rotate(0.0f, 0.3f * sign, 0.0f);
    }

    //private void Fix(Vector3 normal)
    //{
    //    Vector3 fixedVector = Vector3.ProjectOnPlane(mTransform.forward, normal);
    //    mTransform.forward = Vector3.Lerp(mTransform.forward, fixedVector, 0.02f);
    //}

    private float Deg(Vector3 a, Vector3 b)
    {
        float theta = Mathf.Acos(Dot(a, b));
        float deg = theta * Mathf.Rad2Deg;
        return deg;
    }

    private float Dot(Vector3 a, Vector3 b)
    {
        float dot = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        return dot;
    }
}
