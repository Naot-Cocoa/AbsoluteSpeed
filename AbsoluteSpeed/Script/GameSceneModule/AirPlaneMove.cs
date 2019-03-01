using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AirPlaneMove : MonoBehaviour
{
    [SerializeField]
    private Collider mStartTriggerCollider;
    [SerializeField]
    private Collider mEndTriggerCollider;
    [SerializeField]
    private Transform mFlyPos;
    [SerializeField]
    private float mFlyAccel = 1.0f;
    private bool mIsInit = false;
    private Vector3 mStartPos;
    private Quaternion mStartRote;

    private void OnEnable()
    {
        if (!mIsInit)
        {
            mStartPos = transform.position;
            mStartRote = transform.rotation;
            mIsInit = true;
        }
        else if (mIsInit)
        {
            transform.position = mStartPos;
            transform.rotation = mStartRote;
        }

        if (mStartTriggerCollider == null) return;
        if (mEndTriggerCollider == null) return;

        var startSub = new Subject<Unit>();
        var accel = 0.1f;      

        mStartTriggerCollider.OnTriggerEnterAsObservable()
            .Where(x => x.gameObject.GetComponent<IPlayerInput>() != null)
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(x => startSub.OnNext(Unit.Default));

        mEndTriggerCollider.OnTriggerEnterAsObservable()
            .Where(x => x.gameObject.GetComponent<IPlayerInput>() != null)
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ =>
            {
                transform.position = mFlyPos.position;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                accel = mFlyAccel;
            }
            );

        this.UpdateAsObservable()
            .SkipUntil(startSub)
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_=> transform.position += transform.forward * accel);
    }
}
