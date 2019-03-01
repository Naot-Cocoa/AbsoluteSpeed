using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraControl : MonoBehaviour {
    [SerializeField]
    private float mTraceSpeed;

    [SerializeField]
    GameObject snatchPos;
    [SerializeField]
    Transform mTarget;
    Vector3 rotationAngles;
    public Vector3 rotationOffset;
    Quaternion currentRotation;
    public Vector3 positionOffset;

    private void Awake()
    {
        mTarget = GameObject.Find(ConstString.Path.PLAYER).transform;
    }

    private void OnEnable()
    {

        this.UpdateAsObservable()
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ =>
        {
            rotationAngles = transform.eulerAngles;
            rotationAngles.x = Mathf.LerpAngle(rotationAngles.x, mTarget.transform.eulerAngles.x + rotationOffset.x, mTraceSpeed * Time.deltaTime);
            rotationAngles.y = Mathf.LerpAngle(rotationAngles.y, mTarget.transform.eulerAngles.y + rotationOffset.y, mTraceSpeed * Time.deltaTime);
            rotationAngles.z = rotationOffset.z;

            currentRotation = Quaternion.Euler(rotationAngles);

            Quaternion rotation = currentRotation;
            Vector3 position = rotation * (positionOffset) + mTarget.transform.position;
            transform.rotation = rotation;
            transform.position = position;
        });
    }
}
