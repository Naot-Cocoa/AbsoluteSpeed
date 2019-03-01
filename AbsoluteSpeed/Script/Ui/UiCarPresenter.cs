using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 全てのUIを更新させるクラス
/// </summary>
public class UiCarPresenter : MonoBehaviour {

    DataUiCarModel mUiCarModel;
    GameStateManager mGameState;

    //Scene遷移時に一度変更される
    UiTransmission mUiTransmission;

    //GameState内のリアクティブプロパティ
    UiCurrentGear mUiCurrentGear;

    //CarModel内のリアクティブプロパティ
    UiCurrentSpeed mUiCurrentSpeed;
    UiEnginePower mUiEnginePower;

    /// <summary>
    /// 更新させるUIのインスタンスを取得
    /// </summary>
    private void Awake()
    {
        mGameState = FindObjectOfType<GameStateManager>();
        mUiCurrentGear = FindObjectOfType<UiCurrentGear>();
        mUiCurrentSpeed = FindObjectOfType<UiCurrentSpeed>();
        mUiEnginePower = FindObjectOfType<UiEnginePower>();
        mUiTransmission = FindObjectOfType<UiTransmission>();
        mUiCarModel = FindObjectOfType<DataUiCarModel>();
    }

    /// <summary>
    /// Modelの値変更時の処理
    /// </summary>
    void Start () {
        mGameState.CurrentGearState.Subscribe(_ => mUiCarModel.GearDataUpdate(_));
        mUiCarModel.CurrentGear.Subscribe(_ => mUiCurrentGear.UiUpdate(_));
        mUiCarModel.CurrentSpeed.Subscribe(_ => mUiCurrentSpeed.UiUpdate(_));
        mUiCarModel.CurrentEnginePower.Subscribe(_=>mUiEnginePower.UiUpdate(_));
	}
}
