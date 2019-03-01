using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UiSuperClass : MonoBehaviour {

    protected Text mUiTextComponent;//自身のTextコンポーネント
    protected string mUiCurrentText;//更新する値を入れるstringメンバ変数

    protected virtual void Awake()
    {
        mUiTextComponent = GetComponent<Text>();
    }

    /// <summary>
    /// UI_Dinamic_Presenterで定義したサブスクライブから呼び出され自身のTextを更新する
    /// </summary>
    /// <param name="currentValue">更新する現在の値</param>
    public virtual void UiUpdate(float currentValue)
    {
        mUiCurrentText = currentValue.ToString();//現在の値をstringに変換
        mUiTextComponent.text = mUiCurrentText;//Textとして更新
    }
}
