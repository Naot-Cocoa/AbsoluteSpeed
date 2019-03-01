using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.PageUp))
            .Subscribe(_ => InputTracking.Recenter())
            .AddTo(gameObject);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => InputTracking.Recenter())
            .AddTo(gameObject);
    }

    public void Calibration() { InputTracking.Recenter(); }
}
