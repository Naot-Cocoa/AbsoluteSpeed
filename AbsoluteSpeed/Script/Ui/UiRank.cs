using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRank : UiSuperClass
{
    public override void UiUpdate(float currentValue)
    {
        mUiCurrentText = currentValue.ToString();
        //変更の反映
        mUiTextComponent.text = mUiCurrentText;
    }
}