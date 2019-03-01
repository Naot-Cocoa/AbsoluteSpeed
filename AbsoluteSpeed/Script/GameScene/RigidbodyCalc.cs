using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RigidbodyCalc
{
    /// <summary>1フレームに加算する重力</summary>
    private const float GRAVITY = -5.0f;
    /// <summary>最大降下速度</summary>
    private const float MIN_GRAVITY = -50.0f;
    /// <summary>最大上昇速度 </summary>
    private const float MAX_GRAVITY = 5.0f;

    /// <summary>
    /// 重力加算処理
    /// </summary>
    public static Vector3 UpdateGravity(Vector3 velocity)
    {
        velocity += new Vector3(0.0f, GRAVITY);
        velocity.y = Mathf.Clamp(velocity.y, MIN_GRAVITY, MAX_GRAVITY);
        return velocity;
    }

    /// <summary>
    /// floatの速度を求める処理
    /// </summary>
    /// <returns></returns>
    public static float GetFloatVelocity(Vector3 velocity)
    {
        Vector3 vel = velocity;
        vel.y = 0.0f;
        return vel.magnitude;
    }
}


