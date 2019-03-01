using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///ゲームシーン内の時間を管理するクラス 
/// </summary>
public class TimeController : MonoBehaviour {

    private DataUiDinamicModel mUiModel;
    private GameStateManager mGameStateManager;
    
    float mDriveTime;   //ドライブ経過時間s

    private void OnEnable()
    {
        mDriveTime = 0;
    }

    private void Awake()
    {
        mUiModel = FindObjectOfType<DataUiDinamicModel>();
        mGameStateManager = FindObjectOfType<GameStateManager>();
    }

    private void Update()
    {
        if (mGameStateManager.CurrentGameState.Value != InGameState.PLAY) return;
        mDriveTime += Time.deltaTime;
        mUiModel.TimeDataUpdate(mDriveTime);
    }

    /// <summary>
    /// ゲーム内の進行時間を変える
    /// </summary>
    /// <param name="time">変えたい時間</param>
    //public void TimeControl(float time)
    //{
    //    Time.timeScale = time;
    //}
}
