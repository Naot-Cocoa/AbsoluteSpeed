using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TimeUp : MonoBehaviour
{
    private float mLimitTime = 0f;
    [SerializeField]
    private float mLimitMinute = 5f;
    [SerializeField]
    private float mLimitSecond = 30f;

    
    private void Awake()
    {
        mLimitTime = (mLimitMinute * 60f) + mLimitSecond;
    }

    private void OnEnable()
    {
        var data = FindObjectOfType<DataUiDinamicModel>();
        data.Time
            .Where(x => x >= mLimitTime)
            .TakeUntil(this.OnDisableAsObservable())
            .Take(1)
            .Subscribe(_ => MessageBroker.Default.Publish<TimeUpParam>(new TimeUpParam { MUnit = Unit.Default }));
    }
}
public class TimeUpParam
{
    public Unit MUnit { get; set; }
}

