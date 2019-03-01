using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GoalStopMusic : MonoBehaviour {

    [SerializeField]
    private AudioSource mAudioSource;
    [SerializeField]

    private GameStateManager mGameStateManager;

    private void Awake()
    {
        mGameStateManager = GameObject.FindObjectOfType<GameStateManager>();
    }

    private void OnEnable()
    {
        //ResultになったらBGM再生を停止する
        this.UpdateAsObservable()
            .TakeUntil(this.OnDisableAsObservable())
            .Where(_ => mGameStateManager.CurrentGameState.Value == InGameState.RESULT)
            .Take(1)
            .Subscribe(_ =>
            {
                mAudioSource.Stop();
                PlaySE.Instance.Play(mAudioSource, ConstAudio.SE_GOAL);
            });
    }
}
