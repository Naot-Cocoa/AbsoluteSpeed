using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 現在のギアをUIに表示
/// </summary>
public class UiCurrentGear : UiSuperClass
{
    const int mDifferenceValue = -1;    // Stateの差異

    [SerializeField,Header("MAXI")]
    private Font mAlphabetFont;
    private int mAlphabetFontSize = 45;
    [SerializeField,Header("GDDigit")]
    private Font mNumberFont;

    private int mNumberFontSize = 60;   // 数字フォントサイズ

    /// <summary>
    /// UIを更新
    /// </summary>
    /// <param name="currentState"> 現在のギア </param>
    public void UiUpdate(GearState currentState)
    {
        //Stateをintに変換(この時点でステートの値と表示する値の差異を埋める)
        int i = (int)currentState + mDifferenceValue;
        //その後stringへ変換
        if (i == 0) { ChangeFont("N", mAlphabetFont,mAlphabetFontSize); }
        else if (i == -1) { ChangeFont("R", mAlphabetFont,mAlphabetFontSize); }
        else { ChangeFont(i.ToString(), mNumberFont,mNumberFontSize); }

        //変更の反映
        mUiTextComponent.text = mUiCurrentText;
    }

    /// <summary>
    /// 表示フォントを変更する
    /// </summary>
    /// <param name="word"> 表示する文字 </param>
    /// <param name="font"> 変更するフォント </param>
    /// <param name="size"> 文字サイズ </param>
    private void ChangeFont(string word, Font font,int size)
    {
        mUiTextComponent.font = font;
        mUiCurrentText = word;
        mUiTextComponent.fontSize = size;
    }
}