using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class InGameGoalEvent : MonoBehaviour {

    [SerializeField]
    private AudioSource mAudioSource;
    [SerializeField, Tooltip("シーン移行までの時間")]
    private float transitionTime;
    [SerializeField]
    private GameObject mResultCanvas;
    [SerializeField]
    private HalfLineCheckPoint mHalfLineCheckPoint;

    private GameStateManager mGameStateManager; //GameStateManagerクラス
    private GameSceneManager mSceneStateManager;
    private GameObject mPlayer;                 //プレイヤーオブジェクト

    /// <summary>
    /// 初期取得処理
    /// </summary>
    private void Awake()
    {
        mPlayer = GameObject.Find("Player");//後でstringクラスできたらそれにするよー
        mGameStateManager = GameObject.Find("GameManager").GetComponent<GameStateManager>();
        mSceneStateManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    /// <summary>
    /// ストリーム生成
    /// 衝突検知　➡　リザルトシーンへ
    /// </summary>
    private void Start()
    {
        this.OnTriggerEnterAsObservable()
        .Where(collider => collider.gameObject == mPlayer)
        .Where(_ => mHalfLineCheckPoint.PassCheckPointFlag)
        .Where(_=> mGameStateManager.CurrentGameState.Value == InGameState.PLAY)
        .Subscribe(_ => ResultTransition());
    }

    /// <summary>
    /// 数秒間待ってからリザルトシーンに移行する
    /// </summary>
    private void ResultTransition()
    {
        mGameStateManager.GameStateUpdate(InGameState.RESULT);
        PlaySE.Instance.Play(mAudioSource, ConstAudio.SE_RESULT);
        Observable.Timer(TimeSpan.FromSeconds(transitionTime))
            .Subscribe(_ => SceneTransition());
    }

    private async void SceneTransition()
    {
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(_ => mGameStateManager.GameStateUpdate(InGameState.READY));
        mResultCanvas.gameObject.SetActive(false);
        await mSceneStateManager.NextSceneAsync();
    }
	
}
