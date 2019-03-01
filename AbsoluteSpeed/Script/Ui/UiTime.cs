using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiTime : UiSuperClass
{
    double mDisplayTime;


    public override void UiUpdate(float currentValue)
    {
        mDisplayTime = currentValue;
        //mDisplayTime = Math.Round(currentValue, 2, MidpointRounding.AwayFromZero);
        var minutes = currentValue / 60;                 //分計算
        var seconds = currentValue % 60;                 //秒計算
        var miliSeconds = currentValue * 1000 % 1000;    //ミリ秒計算
        mUiCurrentText = ((int)minutes).ToString("00") + " " + ((int)seconds).ToString("00") + " " + ((int)miliSeconds).ToString("00");
        //変更の反映
        mUiTextComponent.text = mUiCurrentText;
    }
}