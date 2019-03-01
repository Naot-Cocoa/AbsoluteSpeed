using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeActiveFalse : MonoBehaviour {

    private GameStateManager mGameStateManager;

    private void Awake()
    {
        mGameStateManager = GameObject.FindObjectOfType <GameStateManager>();
    }

    void OnEnable () {
        if (mGameStateManager.CurrentGameState.Value != InGameState.PLAY)
        { 
            this.gameObject.SetActive(false);
        }
	}
}
