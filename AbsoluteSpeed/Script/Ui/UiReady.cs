using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiReady : MonoBehaviour {

    GameStateManager mGameState;

    private void Awake()
    {
        mGameState = FindObjectOfType<GameStateManager>();
    }

    /// <summary>
    /// ステートをPLAYに変更する、ボタンから呼び出す
    /// </summary>
    public void ReadyStateUpdate()
    {
        mGameState.GameStateUpdate(InGameState.PLAY);
    }
}
