using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeUpEvent : MonoBehaviour
{
    private GameSceneManager mScene;

    [SerializeField]
    private GameObject mTimeUpObject;
    [SerializeField]
    private float mIntervalTime = 5f;
    private void Awake()
    {
        mScene = FindObjectOfType<GameSceneManager>();
        MessageBroker.Default.Receive<TimeUpParam>()
            .Subscribe(_ => EndProcessing())
            .AddTo(gameObject);
    }

    private void EndProcessing()
    {
        if (mTimeUpObject) mTimeUpObject.SetActive(true);
        if (GetComponent<FanControll>().enabled == true) FanStaticController.ChangeAirFlowStatic(FanPower.END);
        var next = new Subject<Unit>();

        Observable.Timer(System.TimeSpan.FromSeconds(mIntervalTime))
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ =>
            {
                if (mTimeUpObject) mTimeUpObject.SetActive(false);
                next.OnNext(Unit.Default);
            });

        next
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(async _ => await mScene.NextSceneAsync());
    }
}
