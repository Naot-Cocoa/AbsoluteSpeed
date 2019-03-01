using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearData
{
    [SerializeField]
    private string mIndexName;
    [SerializeField]
    private GearState mGear;
    [SerializeField]
    private float mNeedSpeed = 0.0f;
    [SerializeField]
    private float mMaxSpeed = 0.0f;
    [SerializeField, Range(0, 100), Tooltip("アクセルを踏んでいないときの逓減速度")]
    private float mEngineBrake = 0.0f;
    [SerializeField, Range(0, 50), Tooltip("アクセルをべた踏みしたとき、1秒間に時速何km上げるか")]
    private float mEngineRps = 0.0f;

    public GearState Gear { get { return mGear; } }
    public float NeedSpeed { get { return ConvertUnits.KmphToMps(mNeedSpeed); } }
    public float MaxSpeed { get { return ConvertUnits.KmphToMps(mMaxSpeed); } }
    public float EngineBrake { get { return mEngineBrake; } }
    public float EngineRot { get { return mEngineRps; } }
}