using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameStateManager : MonoBehaviour
{
    
    private ReactiveProperty<GearState> mCurrentGearState = new ReactiveProperty<GearState>();
    private TransMissionState mCurrentTransState;
    private ReactiveProperty<InGameState> mCurrentGameState = new  ReactiveProperty<InGameState>();
    public ReactiveProperty<GearState> CurrentGearState { get { return mCurrentGearState; } private set { } }
    public TransMissionState CurrentTransMissionState { get { return mCurrentTransState; }private set { } }
    public ReactiveProperty<InGameState> CurrentGameState { get { return mCurrentGameState; } private set { } }
    private void OnEnable()
    {
        //●各値の初期化
        mCurrentGearState.Value = GearState.NEUTRAL;//ギアのステートをニュートラルに
        mCurrentGameState.Value = InGameState.READY;//ゲームのステートをレディに
        //モードセレクトで管理しているトランスミッションをとってくる処理？
    }


    /// <summary>
    /// ギアのUIに反映する値の更新処理
    /// </summary>
    /// <param name="currentGear">更新される現在のギア(何速か)</param>
    public void GearUpdate(GearState currentGear)
    {
        mCurrentGearState.Value = currentGear;
    }
    /// <summary>
    /// ゲームの状態を更新する処理
    /// </summary>
    /// <param name="currentState">更新するゲームのステート</param>
    public void GameStateUpdate(InGameState currentState)
    {
        mCurrentGameState.Value = currentState;
    }
}

