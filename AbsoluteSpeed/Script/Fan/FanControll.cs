using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FanControll : MonoBehaviour
{
    [SerializeField,Tooltip("サーキュレータの風力を弱にする速度")]
    private float mLowSpeed = 10f;
    [SerializeField, Tooltip("サーキュレータの風力を中にする速度")]
    private float mMiddleSpeed = 20f;
    [SerializeField, Tooltip("サーキュレータの風力を強にする速度")]
    private float mHighSpeed = 200f;
    [SerializeField, Tooltip("サーキュレータを動かす")]
    private InGameState mFanStartState = InGameState.PLAY;
    [SerializeField, Tooltip("サーキュレータを止める")]
    private InGameState mFanEndState = InGameState.PAUSE;

    // Use this for initialization
    private void OnEnable()
    {
        var carData = FindObjectOfType<DataUiCarModel>();
        var fan = new FanManager(mLowSpeed, mMiddleSpeed, mHighSpeed);
        var state = FindObjectOfType<GameStateManager>();
        if (state)
        {
            var scene = FindObjectOfType<GameSceneManager>();
            if (scene.SceneState != SceneState.GAME) return;

            var start = state.CurrentGameState
                .Where(x => x == mFanStartState)
                .TakeUntil(this.OnDisableAsObservable());

            start
                .Subscribe(_ => FanStaticController.ChangeAirFlowStatic(FanPower.START));

            var end = state.CurrentGameState
                .Where(x => x == mFanEndState)
                .TakeUntil(this.OnDisableAsObservable());

            end
                .Subscribe(_ => FanStaticController.ChangeAirFlowStatic(FanPower.END));

            fan.GetProcess();
            Observable.Interval(System.TimeSpan.FromSeconds(2))
                .SkipUntil(start)
                .TakeUntil(end)
                .Where(_ => carData.CurrentGear.Value != GearState.REVERSE)
                .Subscribe(_ => fan.ChangeAirFlow(carData.CurrentSpeed.Value));
        }
        else
        {
            fan.GetProcess();
            Observable.Interval(System.TimeSpan.FromSeconds(2))
                .TakeUntil(this.OnDisableAsObservable())
                .Where(_ => carData.CurrentGear.Value != GearState.REVERSE)
                .Subscribe(_ => fan.ChangeAirFlow(carData.CurrentSpeed.Value));
        }       
    }
}
