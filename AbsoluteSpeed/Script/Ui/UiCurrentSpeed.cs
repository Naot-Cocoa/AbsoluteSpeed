using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCurrentSpeed : UiSuperClass
{
    private float mDisplaySpeed;

    public override void UiUpdate(float currentValue)
    {
        mDisplaySpeed = Mathf.Round(currentValue);
        mUiCurrentText = mDisplaySpeed.ToString();
        //変更の反映
        mUiTextComponent.text = mUiCurrentText;
    }
}