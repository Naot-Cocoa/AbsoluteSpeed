using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class SpeedMeter : MonoBehaviour
{
    [SerializeField]
    private float mStartZ = -0.389f;
    [SerializeField]
    private float mEndZ = -114.82f;
    [SerializeField]
    private float mMaxSpeed = 255f;
    private void OnEnable()
    {
        var carData = FindObjectOfType<DataUiCarModel>();
        this.UpdateAsObservable()
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ => CalcRote(carData.CurrentSpeed.Value));
    }

    private void CalcRote(float speed)
    {
        var rate = speed / mMaxSpeed;
        var rote = Mathf.Lerp(mStartZ, mEndZ, rate);
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, rote);
    }
}
