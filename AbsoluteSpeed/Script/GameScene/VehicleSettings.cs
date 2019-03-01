using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "VehicleSettings", fileName = "VehicleSettings")]
public class VehicleSettings : ScriptableObject
{
    [SerializeField]
    private EngineSettings mEngineSettings;
    public EngineSettings GetEngineSettings { get { return mEngineSettings; } }
    [SerializeField]
    private PenaltySettings mPenaltySettings;
    public PenaltySettings GetPenaltySettings { get { return mPenaltySettings; } }
    [SerializeField]
    private DriftSettings mDriftSettings;
    public DriftSettings GetDriftSettings { get { return mDriftSettings; } }
    [SerializeField, Tooltip("方向転換のしやすさ")]
    private SteerSensitivities mSteerSensitivities;
    public SteerSensitivities GetSteerSensitivities { get { return mSteerSensitivities; } }

    [SerializeField]
    private AirRideSettings mAirRideSettings;
    public AirRideSettings GetAirRideSettings { get { return mAirRideSettings; } }




    [System.Serializable]
    public class EngineSettings
    {
        /// <summary>最低エンジン回転数。</summary>
        public const float LOWEST_ENGINE_SPEED = 0.001f;//0にすると処理上動かなくなる為。

        [SerializeField, Range(0, 1), Tooltip("ギアを下げたときに速度がどのくらいの速さで逓減するか")]
        private float mDecreaseSpeed = 0.5f;
        [SerializeField, Range(0, 10), Tooltip("停止時状態でアクセルを踏んだ時に加算する速度")]
        private float mStartDash = 2.0f;
        [SerializeField, Range(0, 20), Tooltip("ブレーキをべた踏みした状態で1秒間に何回転エンジン回転数(速度)を落とすか")]
        private float mBrakePower = 3.0f;

        /// <summary>ギアを下げたときに速度がどのくらいの速さで逓減するか</summary>
        public float DecreaseSpeed { get { return mDecreaseSpeed; } }
        /// <summary>停止時状態でアクセルを踏んだ時に加算する速度</summary>
        public float StartDash { get { return mStartDash; } }
        /// <summary>ブレーキをべた踏みした状態で1秒間に何回転エンジン回転数(速度)を落とすか</summary>
        public float BrakePower { get { return mBrakePower; } }

    }

    [System.Serializable]
    public class PenaltySettings
    {
        [SerializeField, Range(0, 1), Tooltip("壁衝突時にどこまで速度を下げるか。1だと下がらない")]
        private float mWallPenalty = 0.7f;
        [SerializeField, Range(0, 1), Tooltip("壁衝突時どのくらいの速さで速度を逓減させるか。1が最速")]
        private float mWallPenaltyDecreaseSpeed = 0.5f;
        [SerializeField, Range(0, 1), Tooltip("ドリフト中最大どのくらいまで速度が下がるか。1だと下がらない")]
        private float mDriftPenalty = 0.7f;
        [SerializeField, Range(0, 45), Tooltip("値が0だとドリフト状態に入った瞬間速度下がる。値が大きすぎると殆ど変化なくなる")]
        private float mMaxSlipAngle = 15.0f;
        [SerializeField, Range(0, 1), Tooltip("ドリフト時どのくらいの速さで速度を逓減させるか。1が最速")]
        private float mDriftPenaltyDecreaseSpeed = 0.05f;
        [SerializeField, Range(0, 1), Tooltip("AIと車の正面が衝突時,最大どのくらいまで速度が下がるか。1だと下がらない")]
        private float mAiPenalty = 0.7f;
        [SerializeField, Range(0, 1), Tooltip("AIと車の正面が衝突中時,どのくらいの速さで速度を逓減させるか。1が最速")]
        private float mAiPenaltyDecreaseSpeed = 0.05f;

        /// <summary>1のときペナルティ無し</summary>
        public float WallPenalty { get { return mWallPenalty; } }
        /// <summary>1のときペナルティ無し</summary>
        public float DriftPenalty { get { return mDriftPenalty; } }
        /// <summary>1のときペナルティ無し</summary>
        public float MaxSlipAngle { get { return mMaxSlipAngle; } }
        /// <summary>壁衝突時どのくらいの速さで速度を逓減させるか</summary>
        public float WallPenaltyDecreaseSpeed { get { return mWallPenaltyDecreaseSpeed; } }
        /// <summary>ドリフト時どのくらいの速さで速度を逓減させるか</summary>
        public float DriftPenaltyDecreaseSpeed { get { return mDriftPenaltyDecreaseSpeed; } }
        /// <summary>AIと車の正面が衝突時,最大どのくらいまで速度が下がるか。1だと下がらない</summary>
        public float AiPenalty { get { return mAiPenalty; }}
        /// <value>AIと車の正面が衝突中時,どのくらいの速さで速度を逓減させるか。1が最速</value>
        public float AiPenaltyDecreaseSpeed { get { return mAiPenaltyDecreaseSpeed; }}
    }

    [System.Serializable]
    public class DriftSettings
    {
        [SerializeField, Range(0, 250), Tooltip("ドリフト状態に入る速度")]
        private float mDriftSpeed = 100.0f;
        [SerializeField, Range(0, 1), Tooltip("ドリフト時値以下のステアリング入力を無視する")]
        private float mIgnoreSteerInput = 0.5f;
        [SerializeField, Range(0, 40), Tooltip("進行方向と正面方向の角度が値以内の場合、ドリフト状態から復帰する")]
        private float mDriftCancelAngle = 20.0f;//切り出す
        [SerializeField, Range(0, 2), Tooltip("mDriftCancelAngle内に値秒収まった場合、ドリフト状態から復帰する")]
        private float mDriftCancelTime = 0.1f;
        /// <summary>値以上のステアリング入力があった場合ドリフトを考慮する(0~1) /// </summary>
        public float IgnoreSteerInput { get { return mIgnoreSteerInput; } }
        /// <summary>値以上の速度のときドリフトを考慮する</summary>
        public float DriftSpeed { get { return mDriftSpeed; } }
        ///<summary>進行方向と正面方向の角度が値以内の場合、ドリフト状態から復帰する</summary>
        public float DriftCancelAngle { get { return mDriftCancelAngle; } }//切り出す
        ///<summary>mDriftCancelAngle内に値秒収まった場合、ドリフト状態から復帰する</summary>
        public float DriftCancelTime { get { return mDriftCancelTime; } }
    }


    [System.Serializable]
    public class SteerSensitivities
    {
        [SerializeField, Range(0, 5), Tooltip("通常時方向転換の感度")]
        private float mGrip = 1.5f;
        [SerializeField, Range(0, 5), Tooltip("ドリフト時方向転換の感度")]
        private float mDrift = 2.0f;
        [SerializeField, Range(0, 5), Tooltip("滑走時方向転換の感度")]
        private float mSlick = 2.0f;
        public float Grip { get { return mGrip; } }
        public float Drift { get { return mDrift; } }
        public float Slick { get { return mSlick; } }

        public float GetValue(WheelState wheelState)
        {
            switch (wheelState)
            {
                case WheelState.GRIP: return Grip;
                case WheelState.DRIFT: return Drift;
                case WheelState.SLICK: return Slick;
                default: return 0.0f;
            }
        }
    }

    [System.Serializable]
    public class AirRideSettings
    {
        [SerializeField, Range(0, 1), Tooltip("地面にあわせて体勢を整える速度。割合")]
        private float mGroundFollowSpeed = 0.5f;
        [SerializeField, Range(0, 2), Tooltip("地面から浮遊する高さ")]
        private float mHeight = 1.3f;

        ///<summary>地面にあわせて体勢を整える速度。割合</summary>
        public float GroundFollowSpeed { get { return mGroundFollowSpeed; } }
        /// <summary>地面から浮遊する高さ</summary>
        public float Height { get { return mHeight; } }

    }
}
