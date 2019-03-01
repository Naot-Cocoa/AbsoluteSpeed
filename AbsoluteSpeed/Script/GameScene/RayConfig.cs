using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RayConfig", fileName = "RayConfig")]
public class RayConfig : ScriptableObject
{
    /// <summary>左右レイの設定クラス</summary>
    [SerializeField, Tooltip("左右Boxcastの設定項目")]
    private LRRayConfig mLRRayConfig;
    /// <summary>浮遊用レイの設定クラス</summary>
    [SerializeField, Tooltip("下方向Boxcastと四方向レイの設定項目")]
    private AirRideRayConfig mAirRideRayConfig;

    [SerializeField, Tooltip("下方向Boxcastと四方向レイの設定項目")]
    private DirectionFixRayConfig mDirectionFixRayConfig;


    /// <summary>左右レイの設定クラス</summary>
    public LRRayConfig GetLRRayConfig { get { return mLRRayConfig; } }
    /// <summary>浮遊用レイの設定クラス</summary>
    public AirRideRayConfig GetAirRideRayConfig { get { return mAirRideRayConfig; } }

    public DirectionFixRayConfig GetDirectionFixRayConfig { get { return mDirectionFixRayConfig; } }

    [System.Serializable]
    public class LRRayConfig
    {
        [SerializeField, Range(0, 10), Tooltip("左右のレイを飛ばす距離")]
        private float mLRLength = 2.0f;
        [SerializeField, Tooltip("右側レイのオフセット")]
        private Vector3 mROffset = new Vector3(0.0f, 0.0f, 3.0f);
        [SerializeField, Tooltip("左側レイのオフセット")]
        private Vector3 mLOffset = new Vector3(-0.4f, 0.0f, 3.0f);
        [SerializeField, Tooltip("Rayの中心のオフセット")]
        private Vector3 mCenterOffset = new Vector3(-0.2f, 0.0f, 0.5f);
        [SerializeField, Tooltip("左右Boxcastに使用するBoxの大きさ")]
        private Vector3 mBoxcastSize = new Vector3(0.5f, 0.5f, 6.0f);

        /// <summary>左右のレイを飛ばす距離</summary>
        public float LrLength { get { return mLRLength; } }
        /// <summary>右側レイのオフセット </summary>
        public Vector3 ROffset { get { return mROffset; } }
        /// <summary>左側レイのオフセット </summary>
        public Vector3 LOffset { get { return mLOffset; } }
        /// <summary>Rayの中心のオフセット</summary>
        public Vector3 CenterOffset { get { return mCenterOffset; } }
        /// <summary>Boxcastに使用するBoxの大きさ</summary>
        public Vector3 BoxcastSize { get { return mBoxcastSize; } }
    }
    [System.Serializable]
    public class AirRideRayConfig
    {
        [SerializeField, Tooltip("前後のレイの長さ")]
        private float mFBRayLength = 4.5f;
        [SerializeField, Tooltip("左右のレイの長さ")]
        private float mLRRayLength = 4.5f;
        [SerializeField, Tooltip("下方向レイの長さ")]
        private float mUnderRayLength = 2.0f;
        [SerializeField, Tooltip("Rayの中心のオフセット")]
        private Vector3 mCenterOffset = new Vector3(-0.25f, 0.0f, 0.5f);
        [SerializeField, Tooltip("下Boxcastに使用するBoxの大きさ")]
        private Vector3 mBoxSize = new Vector3(2.2f, 0.1f, 5.0f);
        [SerializeField, Tooltip("前後レイの角度。新谷用。")]
        private float mFBRayAngle = 2.0f;
        [SerializeField, Tooltip("左右レイの角度。新谷用。")]
        private float mLRRayAngle = 2.0f;

        /// <summary>前後のレイの長さ</summary>
        public float FBRayLength { get { return mFBRayLength; } }
        /// <summary>左右のレイの長さ</summary>
        public float LRRayLength { get { return mLRRayLength; } }
        /// <summary>下方向レイの長さ</summary>
        public float UnderRayLength { get { return mUnderRayLength; } }
        /// <summary>Rayの中心のオフセット</summary>
        public Vector3 CenterOffset { get { return mCenterOffset; } }
        /// <summary>Boxcastに使用するBoxの大きさ</summary>
        public Vector3 BoxSize { get { return mBoxSize; } }
        public float FBRayAngle { get { return mFBRayAngle; } }
        public float LRRayAngle { get { return mLRRayAngle; } }
    }


    [System.Serializable]
    public class DirectionFixRayConfig
    {
        [SerializeField, Tooltip("触覚の長さ")]
        private float mLRLength = 4.0f;
        [SerializeField]
        private float mForwardBoxcastLength = 1.0f;
        [SerializeField, Tooltip("右触覚の長さ")]
        private Vector3 mRfOffset = new Vector3(0.3f, 0.0f, 2f);
        [SerializeField, Tooltip("左触覚の長さ")]
        private Vector3 mLfOffset = new Vector3(-0.3f, 0.0f, 2f);
        [SerializeField]
        private Vector3 mForwardOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [SerializeField]
        private Vector3 mBoxSize = new Vector3(1.5f, 1.0f, 0.5f);


        public float LRLength { get { return mLRLength; } }
        public float ForwardBoxcastLength { get { return mForwardBoxcastLength; } }
        public Vector3 RfOffset { get { return mRfOffset; } }
        public Vector3 LfOffset { get { return mLfOffset; } }
        public Vector3 ForwardOffset { get { return mForwardOffset; } }
        public Vector3 BoxSize { get { return mBoxSize; } }
    }
}
