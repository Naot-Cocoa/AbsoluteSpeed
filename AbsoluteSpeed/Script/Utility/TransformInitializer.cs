using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformInitializer
{
    private Vector3 mPosition;
    private Quaternion mRotation;
    private Transform mTransform;

    public TransformInitializer(Transform transform)
    {
        mTransform = transform;
        mPosition = transform.position;
        mRotation = transform.rotation;
    }
    public void Reset()
    {
        mTransform.position = mPosition;
        mTransform.rotation = mRotation;
    }
}
