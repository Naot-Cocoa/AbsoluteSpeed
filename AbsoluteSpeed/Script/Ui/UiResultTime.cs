using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiResultTime : UiSuperClass
{
    public override void UiUpdate(float currentValue)
    {
        //ゴールしてから存在させる
        var minutes = currentValue / 60;                 //分計算
        var seconds = currentValue % 60;                 //秒計算
        var miliSeconds = currentValue * 1000 % 1000;    //ミリ秒計算
        mUiCurrentText = ((int)minutes).ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)miliSeconds).ToString("00");
        //変更の反映
        mUiTextComponent.text = mUiCurrentText;
    }

    /// <summary>
    /// Animationから起動するSetActive切り替え
    /// </summary>
    public void ChangeSetActive()
    {
        this.gameObject.SetActive(true);
    }
}