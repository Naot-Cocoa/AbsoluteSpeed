using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.XR;

/// <summary>
/// タイトルシーン管理
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private float intervalTime = 20f;
    private SceneTransition mSceneTransition;

    private void Awake()
    {
        mSceneTransition = transform.root.GetComponentInChildren<SceneTransition>();
    }

    private void OnEnable()
    {
        // トラッキングの初期化
        InputTracking.Recenter();

        // リプレイ記録があるか
        if (!mSceneTransition.IsReplay) return;
        
        // 一定時間でリプレイへ遷移
        var gameSceneManager = FindObjectOfType<GameSceneManager>();
        Observable.Timer(System.TimeSpan.FromSeconds(intervalTime))
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(async _=>await gameSceneManager.PrevSceneAsync());
    }
}
