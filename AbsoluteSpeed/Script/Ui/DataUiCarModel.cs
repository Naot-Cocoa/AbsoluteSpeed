using UnityEngine;
using UniRx;

/// <summary>
/// UIの車両に関連するデータ管理クラス
/// </summary>
public class DataUiCarModel : MonoBehaviour
{
    //各UIのUIに表示される値
    private FloatReactiveProperty mEnginePower = new FloatReactiveProperty();
    private FloatReactiveProperty mCurrentSpeed = new FloatReactiveProperty();
    private ReactiveProperty<GearState> mCurrentGear = new ReactiveProperty<GearState>();
    //各ReactivePropertyのプロパティ
    public FloatReactiveProperty CurrentEnginePower { get { return mEnginePower; } }
    public FloatReactiveProperty CurrentSpeed { get { return mCurrentSpeed; } }
    public ReactiveProperty<GearState> CurrentGear { get { return mCurrentGear; } }

    private void Awake()
    {
        //各値の初期化
        mEnginePower.Value = 0;
        mCurrentSpeed.Value = 0;
        mCurrentGear.Value = GearState.NEUTRAL;
    }

    private void OnEnable()
    {
        //各値の初期化
        mEnginePower.Value = 0;
        mCurrentSpeed.Value = 0;
        mCurrentGear.Value = GearState.NEUTRAL;
    }


    /// <summary>
    /// 各UIに反映する値の更新処理
    /// </summary>
    /// <param name="currentValue">更新する現在の値</param>
    public void TimeDataUpdate(float currentValue)
    {
        mEnginePower.Value = currentValue;
    }
    public void LapTimeDataUpdate(float currentValue)
    {
        mCurrentSpeed.Value = currentValue;
    }



    /// <summary>
    /// ギアのUIを反映させる処理
    /// </summary>
    /// <param name="currentState">更新するステート</param>
    public void GearDataUpdate(GearState currentState)
    {
        mCurrentGear.Value = currentState;
    }

}
