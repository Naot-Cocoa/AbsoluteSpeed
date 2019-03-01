using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

public class ReplaySceneManager : MonoBehaviour
{
    [SerializeField]
    private float intervalTime = 20f;
    private GameSceneManager mGameSceneManager;

    private void Awake()
    {
        mGameSceneManager = transform.root.GetComponentInChildren<GameSceneManager>();
    }

    private void OnEnable()
    {
        UnityEngine.XR.XRSettings.showDeviceView = false;
        mGameSceneManager.AwakeStage();

        Observable.NextFrame()
            .Subscribe(_ => Replay());
        
        Observable.Timer(System.TimeSpan.FromSeconds(intervalTime))
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(async _ => await mGameSceneManager.NextSceneAsync());
    }

    private void OnDisable()
    {
        UnityEngine.XR.XRSettings.showDeviceView = true;
        mGameSceneManager.SleepStage();
    }

    /// <summary>
    /// リプレイ起動
    /// </summary>
    private void Replay()
    {
        var replay = FindObjectsOfType<RePlayer>();
        foreach (var x in replay)
        {
            x.ReReplay();
            x.Replay();
        }
    }
}
