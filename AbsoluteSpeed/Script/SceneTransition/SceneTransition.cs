using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class SceneTransition : MonoBehaviour {

    [SerializeField]
    private AudioSource mAudioSource;
    [SerializeField]
    private string mClipName;

    private GameSceneManager mSceneStateManager;
    public bool IsReplay { get; private set; } = false; 

    /// <summary>
    /// 初期取得
    /// </summary>
    private void Awake()
    {
        mSceneStateManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        this.UpdateAsObservable()
            .Where(_ => Input.GetAxisRaw(ConstString.Input.ACCEL) >= 0.9f || Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown(ConstString.Input.SUBMIT))
            .Where(_ => mSceneStateManager.mCanInput)
            .Where(_ => mSceneStateManager.SceneState == SceneState.TITLE)
            .Take(1)
            .Subscribe(_ => IsReplay = true);
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void OnEnable()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetAxisRaw(ConstString.Input.ACCEL) >= 0.9f || Input.GetKeyDown(KeyCode.Return))
            .Where(_ => mSceneStateManager.mCanInput)
            .Take(1)
            .Subscribe(async _ => {
                PlaySE.Instance.Play(mAudioSource, mClipName);
                await mSceneStateManager.NextSceneAsync();
            });
    }
}
