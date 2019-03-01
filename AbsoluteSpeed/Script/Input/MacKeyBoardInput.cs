using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MacKeyBoardInput : MonoBehaviour, IPlayerInput,IGearManageable
{
    public IReadOnlyReactiveProperty<float> Accel { get { return mAccel; } }
    public IReadOnlyReactiveProperty<float> Brake { get { return mBrake; } }
    public IReadOnlyReactiveProperty<float> Hundle { get { return mHundle; } }
    public IReadOnlyReactiveProperty<GearState> GetCurrentGear { get { return mCurrentGear; } }

    private FloatReactiveProperty mAccel = new FloatReactiveProperty(0);
    private FloatReactiveProperty mBrake = new FloatReactiveProperty(0);
    private FloatReactiveProperty mHundle = new FloatReactiveProperty(0);
    private ReactiveProperty<GearState> mCurrentGear = new ReactiveProperty<GearState>(GearState.NEUTRAL);
    [SerializeField]
    private GearManager mGearManager;

    public GearManager GetGearManager { get { return mGearManager; } }


    private void Awake()
    {
        mGearManager = new MacGearManager(FindObjectOfType<VehicleCore>());

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //アクセル
                float accel = Input.GetAxis(ConstString.Input.VERTICAL);
                accel = Mathf.Clamp01(accel);
                mAccel.SetValueAndForceNotify(accel);

                //ハンドル
                mHundle.SetValueAndForceNotify(Input.GetAxis(ConstString.Input.HORIZONTAL));

                //ブレーキ
                float brake = Input.GetAxis(ConstString.Input.VERTICAL);
                //下矢印キーでブレーキ。ハンコンでは01で取得するため符号反転してる
                brake = -Mathf.Clamp(brake, -1.0f, 0.0f);
                mBrake.SetValueAndForceNotify(brake);

                //ギア
                mCurrentGear.Value = mGearManager.GetGear();
            });
    }


    private void OnEnable() { mGearManager.OnEnable(); }

    public void ConvertTo(Transmission transmission) { mGearManager.ConvertTo(transmission); }
}

