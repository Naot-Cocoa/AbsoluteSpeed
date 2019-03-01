using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FadeScreenActiveManager : MonoBehaviour {

    [SerializeField]
    private GameObject mFadeScreen;

    private Fade mFade;
    private GameStateManager mGameStateManager;

    private void Awake()
    {
        mGameStateManager = GameObject.FindObjectOfType<GameStateManager>();
        mFade = GameObject.Find("GameFadeScreen").GetComponent<Fade>();
    }

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => mGameStateManager.CurrentGameState.Value == InGameState.READY)
            .Where(_ => mFade.CutoutThreshold <= 0)
            .Subscribe(_ => NonActiveScreen());

        this.UpdateAsObservable()
            .Where(_ => mGameStateManager.CurrentGameState.Value == InGameState.RESULT)
            .Where(_ => 0.1 <= mFade.CutoutThreshold)
            .Subscribe(_ => ActiveScreen());
    }

    /// <summary>
    /// FadeScreenのSetActiveを管理
    /// </summary>
    private void NonActiveScreen()
    {
        mFadeScreen.gameObject.SetActive(false);
    }

    private void ActiveScreen()
    {
        mFadeScreen.gameObject.SetActive(true);
    }
}
