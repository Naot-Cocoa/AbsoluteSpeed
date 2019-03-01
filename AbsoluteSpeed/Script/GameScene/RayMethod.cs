using UnityEngine;


public static class RayMethod
{
    /// <summary>
    /// レイを発射するpositionを作成
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Vector3 MakeFrom(Transform transform, Vector3 offset)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Vector3 from = pos + (rot * offset);
        return from;
    }
}