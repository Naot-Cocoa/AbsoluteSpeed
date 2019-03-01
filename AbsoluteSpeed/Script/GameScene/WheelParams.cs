using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイヤの状態ごとの滑走具合を制御するクラス
/// </summary>
[CreateAssetMenu(menuName = "WheelParam", fileName = "WheelParam")]
public class WheelParams : ScriptableObject
{
    [SerializeField]
    private WheelData[] mWheelData;
    public WheelData[] WheelData { get { return mWheelData; } }

    /// <summary>
    /// ギア毎の滑り具合を決める係数を返す処理
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float GetGripCoef(float speed, WheelState wheelState)
    {
        int length = WheelData.Length;

        for (int i = 0; i < length; i++)
        {
            if (speed < WheelData[i].Speed)
            {
                return WheelData[i].GetCoef(wheelState);
            }
        }
        return WheelData[length - 1].Grip;
    }
}


