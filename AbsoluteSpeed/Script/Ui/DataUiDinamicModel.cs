using UnityEngine;
using UniRx;

/// <summary>
/// UIに対する値の反映を管理する
/// </summary>
public class DataUiDinamicModel : MonoBehaviour {
    //各UIのUIに表示される値
    private FloatReactiveProperty mTimeData = new FloatReactiveProperty();
    private FloatReactiveProperty mLapTimeData = new FloatReactiveProperty();   
    private FloatReactiveProperty mRankData = new FloatReactiveProperty();
    //ゲームの状態によって変わるUIの為のリアクティブプロパティ
    private ReactiveProperty<InGameState> mCurrentGameState = new ReactiveProperty<InGameState>();
    //各ReactivePropertyのプロパティ
    public FloatReactiveProperty Time { get { return mTimeData; } private set { } }
    public FloatReactiveProperty LapTime { get { return mLapTimeData; } private set { } }
    public FloatReactiveProperty Rank { get { return mRankData; } private set { } }
    public ReactiveProperty<InGameState> CurrentGameState { get { return mCurrentGameState; } private set { } }

    private void OnEnable()
    {
        //各値の初期化
        mTimeData.Value = 0;
        mLapTimeData.Value = 0;
        mRankData.Value = 0;
    }

    /// <summary>
    /// 各UIに反映する値の更新処理
    /// </summary>
    /// <param name="currentValue">更新する現在の値</param>
    public void TimeDataUpdate(float currentValue)
    {
        mTimeData.Value = currentValue;
    }
    public void LapTimeDataUpdate(float currentValue)
    {
        mLapTimeData.Value = currentValue;
    }
    public void RankDataUpdate(float currentValue)
    {
        mRankData.Value = currentValue;
    }

    /// <summary>
    /// ゲームの状態(現在のステート)を取得し、更新
    /// </summary>
    /// <param name="currentState">更新するステート</param>
    public void GameStateDataUpdate(InGameState currentState)
    {
        mCurrentGameState.Value = currentState;
    }
}
