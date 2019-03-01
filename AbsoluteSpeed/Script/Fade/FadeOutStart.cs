using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FadeOutStart : MonoBehaviour {

    private GameSceneManager mSceneStateManager;
    private Fade mFade;

    /// <summary>
    /// 初期取得処理
    /// </summary>
    private void Awake()
    {
        mSceneStateManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        mFade = GetComponent<Fade>();
    }

    void Start () {
        this.UpdateAsObservable()
            .Where(_ => mSceneStateManager.SceneNumProperty == 0)
            .Take(1)
            .Subscribe(async _ => { await mSceneStateManager.TitleFadeOutStart(); print("FadeOut"); });
	}
}
