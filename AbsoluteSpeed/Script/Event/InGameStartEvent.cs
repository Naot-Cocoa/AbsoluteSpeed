using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class InGameStartEvent : MonoBehaviour {

    private GameStateManager mGameStateManager;

    /// <summary>
    /// 初期取得処理
    /// </summary>
    private void Awake()
    {
        mGameStateManager = GameObject.Find("GameManager").GetComponent<GameStateManager>();
    }

    /// <summary>
    /// ストリーム生成
    /// アニメーション再生
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// ステート変更メソッド
    /// アニメーションから呼び出す
    /// </summary>
    public void StateChangeInGame()
    {
        mGameStateManager.GameStateUpdate(InGameState.PLAY);
    }
}
