using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InGameManager : MonoBehaviour
{

    private GameSceneManager mGameSceneManager;


    private void Awake()
    {
        mGameSceneManager = transform.root.GetComponentInChildren<GameSceneManager>();
    }

    private void OnEnable()
    {
        mGameSceneManager.AwakeStage();

        Observable.NextFrame()
            .Subscribe(_ => Replay()); 
    }

    private void OnDisable()
    {
        mGameSceneManager.SleepStage();
    }

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
