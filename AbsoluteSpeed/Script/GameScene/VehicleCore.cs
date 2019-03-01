using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class VehicleCore : MonoBehaviour
{

    private VehicleSettings mVehicleSettings;
    ///<summary>各ギアのときの速度が入っている。ScriptableObject</summary>
    private GearParam mGearParam;
    ///<summary>速度ごとのタイヤの状態を定義したスクリプタブルオブジェクトを格納する</summary>
    private WheelParams mWheelParams;
    ///<summary>Rayの設定</summary>
    private RayConfig mRayConfig;
    private Engine mEngine;
    /// <summary>車本体のRigidbody</summary>
    private Rigidbody mRigidbody;
    ///<summary>車を動かす為の処理クラス</summary>
    private VehicleMove mVehicleMove;
    /// <summary>車体の姿勢矯正クラス</summary>
    private AirRide mAirRide;
    /// <summary>進行方向修正クラス </summary>
    private DirectionFix mDirFix;
    /// <summary>壁接触判定クラス</summary>
    private WallHitCheck mWallHitCheck;
    /// <summary> </summary>
    private DataUiCarModel mDataUiCarModel;
    /// <summary>プレイヤーからの入力</summary>
    private IPlayerInput mPlayerInput;
    private TransformInitializer mInitializer;
    private DirectionFixRay mAIPenaltyRay;
    private GameSceneManager mGameSceneManager;



    //Gizmos
    [SerializeField]
    private bool mGizmoSwitch = true;
    [SerializeField]
    private bool mGUISwitch = true;


    public float EngineSpeed { get { return mEngine.GetEngineSpeed; } }


    private void Awake() { Initialize(); }


    private void OnEnable()
    {
        mRigidbody.velocity = Vector3.zero;
        mInitializer.Reset();
    }

    private void Start()
    {
        /*エンジンの回転から前に力を加える処理
          車体を進行方向に向かせる処理*/
        (this).FixedUpdateAsObservable()
            .Subscribe(_ =>
        {
            if (mGameSceneManager.SceneState != SceneState.GAME) { return; }

            float accel = mPlayerInput.Accel.Value;
            float steer = mPlayerInput.Hundle.Value;
            float brake = mPlayerInput.Brake.Value;
            GearState currentGear = mPlayerInput.GetCurrentGear.Value;

            print("Brake : " + brake);

            //姿勢制御処理
            mAirRide.FixBalance(mRigidbody.velocity);
            //進行方向修正処理
            mDirFix.FixDirection(accel, steer);//傾く
            //ジャンプ中はブレーキとハンドルは制御させない
            if (!mAirRide.IsGround)
            {
                brake = 0.0f;
                steer = 0.0f;
            }

            RaycastHit hitInfo = new RaycastHit();
            mAIPenaltyRay.ForwardBoxCast(out hitInfo);
            //aiと正面方向が衝突したときaccel無効にする
            //アクセルを切っても速度は結構生きているかも?

            steer = SteeringScript.IgnoreLSteer(mWallHitCheck.LHit, steer);
            steer = SteeringScript.IgnoreRSteer(mWallHitCheck.RHit, steer);
            //ペナルティ処理
            mEngine.GiveWallPenalty(mWallHitCheck.LOrRHit);
            mEngine.GiveDriftPenalty(mVehicleMove.GetWheelState == WheelState.DRIFT, transform.forward);
            bool aiPenalty = (hitInfo.transform != null) && hitInfo.transform.gameObject.CompareTag(ConstString.Tag.AI);
            mEngine.GiveAiPenalty(aiPenalty);

            float engineRot = mEngine.UpdateRotateEngine(accel, brake, currentGear);

            steer = SteeringScript.InverseSteer(engineRot, steer);
            //車を動かす処理
            mVehicleMove.UpdateMove(engineRot, steer, brake, mAirRide.IsGround);

            //データ書き込み処理
            WriteToModel(engineRot, currentGear);
        });

        this.OnCollisionStayAsObservable().Subscribe(_ =>
        {
            mEngine.OnCollisionStay();
        });
    }

    private void Initialize()
    {
        AccessToResources();
        //ユーザーの入力を取得
        mPlayerInput = GetComponent<IPlayerInput>();
        mRigidbody = GetComponent<Rigidbody>();
        mDataUiCarModel = FindObjectOfType<DataUiCarModel>();
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        mRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        mRigidbody.useGravity = false;
        mInitializer = new TransformInitializer(transform);
        //エンジン内で使用する計算クラス
        EngineSpeedCalc engineCalc = new EngineSpeedCalc(mVehicleSettings.GetEngineSettings, mVehicleSettings.GetPenaltySettings, mGearParam, mPlayerInput.GetCurrentGear);
        //エンジンを初期化
        mEngine = new Engine(engineCalc, mRigidbody, gameObject.FindObjectOfInterface<IGearManageable>(), mPlayerInput.GetCurrentGear);

        mVehicleMove = new VehicleMove(mRigidbody, transform, mWheelParams,
                                       mVehicleSettings.GetDriftSettings, mVehicleSettings.GetSteerSensitivities);
        mAirRide = new AirRide(transform, mRayConfig.GetAirRideRayConfig, mVehicleSettings.GetAirRideSettings);
        mDirFix = new DirectionFix(transform, mRayConfig.GetDirectionFixRayConfig);
        mAIPenaltyRay = new DirectionFixRay(transform, mRayConfig.GetDirectionFixRayConfig);
        mWallHitCheck = new WallHitCheck(transform, mRayConfig.GetLRRayConfig);
    }

    private void AccessToResources()
    {
        mGearParam = Resources.Load(ConstString.Path.GEAR_PARAM) as GearParam;
        mRayConfig = Resources.Load(ConstString.Path.RAY_CONFIG) as RayConfig;
        mWheelParams = Resources.Load(ConstString.Path.WHEEL_PARAM) as WheelParams;
        mVehicleSettings = Resources.Load(ConstString.Path.VEHICLE_SETTINGS) as VehicleSettings;
    }

    private void WriteToModel(float engineRot, GearState currentGear)
    {
        //データ書き込み処理
        mDataUiCarModel.CurrentSpeed.Value = ConvertUnits.MpsToKmph(mVehicleMove.GetSpeed);
        mDataUiCarModel.CurrentEnginePower.Value = mEngine.GetEngineRpm(engineRot);
        mDataUiCarModel.CurrentGear.Value = currentGear;
    }

    private void OnDrawGizmos()
    {
        if (!mGizmoSwitch) { return; }
        AccessToResources();
        AirRide gizmosAirRide = new AirRide(transform, mRayConfig.GetAirRideRayConfig, mVehicleSettings.GetAirRideSettings);
        DirectionFixRay dirFixRay = new DirectionFixRay(transform, mRayConfig.GetDirectionFixRayConfig);
        dirFixRay.DrawGizmos();
        gizmosAirRide.DrawGizmos();
        LRRay lrRay = new LRRay(transform, mRayConfig.GetLRRayConfig);
        lrRay.DrawRayGizmos();
        DriftScript ds = new DriftScript(mVehicleSettings.GetDriftSettings);
        ds.OnDrawGimos(transform, GetComponent<Rigidbody>());
    }

    private void OnGUI()
    {
        if (!mGUISwitch) { return; }
        string text = "vehicle Info\n";
        text += string.Format("\taccel {0} brake{1} hundle{2}\n", mPlayerInput.Accel, mPlayerInput.Brake, mPlayerInput.Hundle);
        text += mVehicleMove.OnGUITexts();
        text += string.Format("\tground ray hit is {0}\n", mAirRide.IsGround);
        text += string.Format("\tleft wall ray hit is {0} right wall ray hit is {1}\n", mWallHitCheck.LHit, mWallHitCheck.RHit);
        text += string.Format("Engine.EngineSpeed Value is {0}\n", ConvertUnits.MpsToKmph(mEngine.GetEngineSpeed));
        text += string.Format("VehicleMove.EngineSpeed Value is {0}\n", ConvertUnits.MpsToKmph(mVehicleMove.GetSpeed));
        if (mDataUiCarModel) text += string.Format("CurrentGear {0}\n", mDataUiCarModel.CurrentGear);
        GUI.color = Color.white;
        GUI.TextArea(new Rect(0, 0, 420, 150), text);
    }
}

