using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTransmission : UiSuperClass
{
    public void UiUpdate(TransMissionState currentState)
    {
        //渡されたStateに応じて表示UIを変更する(後にスプライトを変更する処理に変わるかも)
        switch (currentState)
        {
            case TransMissionState.AT:
                mUiCurrentText = "AT";
                break;

            case TransMissionState.MT:
                mUiCurrentText = "MT";
                break;
        }

        //変更の反映
        mUiTextComponent.text = mUiCurrentText;
    }
}
