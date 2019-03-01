using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class WheelManager
{
    /// <summary>自分の車の全輪</summary>
    private List<Wheel> mWheels;

    /*■■■PUBLIC PROPERTY■■■*/

    /// <summary>
    /// 自分のホイール型リストを代入
    /// </summary>
    public WheelManager(List<Wheel> wheels)
    {
        mWheels = wheels;
    }
    /*■■■PUBLIC PROPERTY■■■*/

    /*■■■PUBLIC METHOD■■■*/

    /// <summary>
    /// タイヤを回転させる処理
    /// ■rotSpeed:現在の速度■
    /// </summary>
    /// <param name="rotSpeed"></param>
    public void RotateWheels(float rotSpeed)
    {
        if (mWheels == null) { return; }

        foreach (Wheel tire in mWheels)
        {
            tire.RotateWheel(rotSpeed);
        }
    }
    /*■■■PUBLIC METHOD■■■*/
}
