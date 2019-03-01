using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BoardInput : MonoBehaviour, IPlayerInput
{
    public IReadOnlyReactiveProperty<float> Accel { get { return mAccel; } }
    public IReadOnlyReactiveProperty<float> Brake { get { return mBrake; } }
    public IReadOnlyReactiveProperty<float> Hundle { get { return mHundle; } }
    public IReadOnlyReactiveProperty<GearState> GetCurrentGear { get { return mCurrentGear; } }

    private FloatReactiveProperty mAccel = new FloatReactiveProperty(0);
    private FloatReactiveProperty mBrake = new FloatReactiveProperty(0);
    private FloatReactiveProperty mHundle = new FloatReactiveProperty(0);
    private ReactiveProperty<GearState> mCurrentGear = new ReactiveProperty<GearState>(GearState.NEUTRAL);
    private int mCurrentGearIndex = 0;

    private void Awake()
    {

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //アクセル
                float accel = Input.GetAxis("Vertical");
                accel = Mathf.Clamp01(accel);
                mAccel.SetValueAndForceNotify(accel);

                //ハンドル
                mHundle.SetValueAndForceNotify(Input.GetAxis("Horizontal"));

                //ブレーキ
                float brake = Input.GetAxis("Vertical");
                //下矢印キーでブレーキ。ハンコンでは01で取得するため符号反転してる
                brake = -Mathf.Clamp(brake, -1.0f, 0.0f);
                mBrake.SetValueAndForceNotify(brake);

                //ギア
                mCurrentGear.Value = ChangeGear();
            });
    }


    private void OnEnable()
    {
        mCurrentGearIndex = (int)GearState.FIRST;
    }


    private GearState ChangeGear()
    {
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            mCurrentGearIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            mCurrentGearIndex--;
        }
        mCurrentGearIndex = Mathf.Clamp(mCurrentGearIndex, 0, (int)GearState.MAX - 1);

        return (GearState)Enum.ToObject(typeof(GearState), mCurrentGearIndex);
    }

}

