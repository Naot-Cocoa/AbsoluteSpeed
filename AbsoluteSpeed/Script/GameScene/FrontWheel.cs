using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
/// <summary>
/// 前輪
/// </summary>
public class FrontWheel : Wheel
{
    ///<summary>前タイヤの回転角度(y軸)</summary>
    private const float STEER_ANGLE = 5.0f;
    private void Start()
    {
        //Hundleの入力があったとき、タイヤを左右に動かす
        PlayerInput.Hundle.Subscribe(x =>
        {
            transform.localRotation = TurnTire(x);
        });

    }


    /// <summary>
    /// Y軸回転。HMDならいらないかも知れない。
    /// ■hundleInput:ハンドルからの入力。-1f~1f■
    /// </summary>
    private Quaternion TurnTire(float hundleInput)
    {
        return Quaternion.AngleAxis(hundleInput * STEER_ANGLE, transform.up);
    }


    /// <summary>
    ///ブレーキで停止する
    /// </summary>
    /// <param name="rotSpeed"></param>
    public override void RotateWheel(float rotSpeed)
    {
        float brake = PlayerInput.Brake.Value;
        //入力値が大きいほど0に近づくようにする
        brake = 1.0f - brake;
        rotSpeed *= brake;
        base.RotateWheel(rotSpeed);
    }
}
