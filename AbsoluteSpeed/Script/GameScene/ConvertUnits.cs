using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ConvertUnits
{
    private const float THOUSAND = 1000.0f;
    private const float ONE_HOUR = 3600.0f;

    /// <summary>
    /// m/sをKm/hに変換する
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static float MpsToKmph(float num)
    {
        num = num / THOUSAND * ONE_HOUR;
        return num;
    }

    /// <summary>
    /// Km/hをm/sに変換する
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static float KmphToMps(float num)
    {
        num = num * THOUSAND / ONE_HOUR;
        return num;
    }
}