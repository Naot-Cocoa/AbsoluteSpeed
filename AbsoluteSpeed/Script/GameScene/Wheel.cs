using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// タイヤの親クラス
/// </summary>
public class Wheel : MonoBehaviour
{
    ///<summary>最大熱量</summary>
    private float mMaxJoule;
    /// <summary>スリップしているか</summary>
    private bool mIsSlip;
    protected IPlayerInput PlayerInput { get; private set; }

    protected void Awake()
    {
        //Playerからの入力クラスを取得
        PlayerInput = this.FindObjectOfInterface<IPlayerInput>();
        PlayerInput.Accel.Subscribe(_ =>
        {
            //transform.rotation *= Quaternion.Euler()
        });
    }

    /*■■■PUBLIC METHOD■■■*/
    /// <summary>
    /// タイヤを回転させる(転がす)処理
    /// ■rotSpeed:回転させる速度■
    /// </summary>
    public virtual void RotateWheel(float rotSpeed)
    {
        Transform pivot = transform.GetChild(0);
        //値が大きかったので100で割ってます->要修正
        pivot.localRotation *= Quaternion.Euler(rotSpeed / 100f, 0.0f, 0.0f);
    }
    /*■■■PUBLIC METHOD■■■*/

    /// <summary>
    /// バウンド処理
    /// </summary>
    protected void Bound(){}



}
