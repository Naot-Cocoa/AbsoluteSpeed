using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
public class HundleControllerInput : MonoBehaviour, IPlayerInput, IGearManageable
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
    private GameStateManager mGameStateManager;
    private GameSceneManager mGameSceneManager;

    public GearManager GetGearManager { get { return mGearManager; } }



    private void Awake()
    {
        mGearManager = new GearManager(FindObjectOfType<VehicleCore>());
        mGameStateManager = FindObjectOfType<GameStateManager>();
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //ギア
                mCurrentGear.Value = mGearManager.GetGear();
                if(mGameSceneManager.SceneState != SceneState.GAME) { return; }
                if (mGameStateManager.CurrentGameState.Value == InGameState.READY) { return; }
                float accel = Input.GetAxisRaw(ConstString.Input.ACCEL);
                accel = Mathf.Clamp01(accel);
                mAccel.SetValueAndForceNotify(accel);

                //ハンドル
                mHundle.SetValueAndForceNotify(Input.GetAxisRaw(ConstString.Input.HORIZONTAL));

                //ブレーキ
                float brake = Input.GetAxisRaw(ConstString.Input.BRAKE);
                //下矢印キーでブレーキ。ハンコンでは01で取得するため符号反転してる
                brake = -Mathf.Clamp(brake, -1.0f, 0.0f);
                mBrake.SetValueAndForceNotify(brake);
                

            }).AddTo(gameObject);
    }

    private void OnEnable()
    {
        mAccel.Value = 0.0f;
        mBrake.Value = 0.0f;
        mHundle.Value = 0.0f;
        mGearManager.OnEnable();
    }

    /// <summary>AT,MT変換に使用</summary>
    /// <param name="transmission"></param>
    public void ConvertTo(Transmission transmission) { mGearManager.ConvertTo(transmission); }
}


