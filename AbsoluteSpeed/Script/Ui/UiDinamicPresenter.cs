using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UiDinamicPresenter : MonoBehaviour {

    DataUiDinamicModel mUiDinamicModel;
    GameStateManager mGameState;

    //☆☆☆☆☆☆全ての更新させるUIのクラス☆☆☆☆☆☆

    //◆◆◆ゲームシーンのStateが変更される事によって変わるUI◆◆◆

    //●RESULT
    UiGoal mGoal;

    //●ゲーム進行中に逐次変わるUI
    UiRank mCurrentRank;
    UiLap mCurrentLap;
    UiTime mCurrentTime;
    TimeController mTimeController;

    //◆◆◆Stateの変更処理に使用する数値◆◆◆
    List<float> mLapList = new List<float>();//ラップタイムを記録する配列
    float mAddLapTime;//リストに使用する時間
    public List<float> LapList { get { return mLapList; } private set { } }
    //●PAUSE
    const float mStopTime = 0;
    const float mPlayTime = 1;


    /// <summary>
    /// 更新させるUIのインスタンスを取得
    /// </summary>
    private void Awake()
    {
        mUiDinamicModel = FindObjectOfType<DataUiDinamicModel>();
        mGameState = FindObjectOfType<GameStateManager>();
        mGoal = FindObjectOfType<UiGoal>();
        mCurrentRank = FindObjectOfType<UiRank>();
        mCurrentLap = FindObjectOfType<UiLap>();
        mCurrentTime = FindObjectOfType<UiTime>();
        mTimeController = FindObjectOfType<TimeController>();
    }

    /// <summary>
    /// Modelの値変更時の処理
    /// </summary>
    void Start() {
        //time変更時
        mUiDinamicModel.Time.Subscribe(_ => mCurrentTime.UiUpdate(_));
        //rank変更時
        mUiDinamicModel.Rank.Subscribe(_ => mCurrentRank.UiUpdate(_));
        //laptime変更時
        //mUiDinamicModel.LapTime.Subscribe(_ => mCurrentLap.UiUpdate(_));

        //■■■■■■■■■ゲームシーンState変更時■■■■■■■■■■

        mGameState.CurrentGameState.Subscribe(_ => mUiDinamicModel.GameStateDataUpdate(_));

        ////ステートがPAUSEじゃなければタイムスケールを元に戻す
        //mGameState.CurrentGameState
        //   .Where(_ => mGameState.CurrentGameState.Value != InGameState.PAUSE)
        //   .Where(_ => Time.timeScale == mStopTime)
        //   .Subscribe(_ => mTimeController.TimeControl(mPlayTime));

        //ステートがRESULTに変更された時ゴールの処理を行う
        mGameState.CurrentGameState
            .Where(_ => mGameState.CurrentGameState.Value == InGameState.RESULT)
            .Subscribe(_ => mGoal.Goal(mUiDinamicModel.Time.Value, mUiDinamicModel.Rank.Value)); //,mLapList

        ////ステートがPAUSEに変更された際に時間を止めてPAUSE画面を表示する
        //mGameState.CurrentGameState
        //    .Where(_ => mGameState.CurrentGameState.Value == InGameState.PAUSE)
        //    .Subscribe(_ => mTimeController.TimeControl(mStopTime));

    }

    /// <summary>
    /// ゲーム内ラップ保存処理、LapGetterにより呼ばれる
    /// </summary>
    public void LapUpdate()
    {
        mAddLapTime = mUiDinamicModel.Time.Value;
        LapList.Add(mAddLapTime);
    }
}