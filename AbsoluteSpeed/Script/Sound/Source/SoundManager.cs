using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Linq;
/// <summary>
/// 車の情報を保持する構造体
/// </summary>
public struct CarSoundParam
{
    public float ThrottleInput;
    public float BrakeInput;
    public float TorqueInput;
    public float VelocityInput;
    public float RpmInput;
    public Vector3 PreVelocity;
    public float HitVelocity;
    public float HitContinueVelocity;
}
[System.Serializable]
public struct AudioParam
{
    public AudioSource source;
    [Range(0f, 1f)]
    public float volume;
    [Range(-3f, 3f)]
    public float pitch;
}
public class SoundManager : MonoBehaviour
{
    private DataUiCarModel mDataUiCarModel;
    private IPlayerInput mInput;
    private GameObject mAudioParent;
    private Rigidbody mRigidbody;
    private BaseSoundParam[] mSounds;
    private CarSoundParam mCarParam = new CarSoundParam();
    public AudioParam[] mAudioParam;
    public bool mIsAudioCheck = false;
    [SerializeField]
    private float MaxRPM = 8000f;
    [SerializeField]
    private float MinRPM = 100f;

    private void OnEnable()
    {       
        mInput = GetComponent<IPlayerInput>();
        mRigidbody = GetComponent<Rigidbody>();
        mDataUiCarModel = FindObjectOfType<DataUiCarModel>();
        mCarParam.PreVelocity = mRigidbody.velocity;
        //初回起動
        if (!mAudioParent)
        {
            var obj = Resources.LoadAll("SoundParam");
            mSounds = new BaseSoundParam[obj.Length];
            mSounds = obj.Select(x => (BaseSoundParam)x).ToArray();
            mAudioParent = new GameObject("Audio");
            mAudioParent.transform.parent = transform;
            mAudioParent.transform.localPosition = Vector3.zero;
            mAudioParent.transform.localRotation = Quaternion.identity;
            foreach (var x in mSounds) x.Init(mAudioParent.transform);
        }
        //2回目以降
        else if (mAudioParent) foreach (var x in mSounds) x.Init();
        if (!mIsAudioCheck) return;
        var rpmAudio = mAudioParent.Descendants().Where(x => x.name.Contains("RPM"));
        mAudioParam = new AudioParam [rpmAudio.Count()];
        foreach (var x in rpmAudio.Select((x, i) => new { x, i }))
        {
            mAudioParam[x.i].source = x.x.GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        UpdateAudioParam();
        UpdateCarParam();
        foreach (var x in mSounds) x.SoundUpdate(mCarParam);
        mCarParam.PreVelocity = mRigidbody.velocity;
    }

    private void UpdateAudioParam()
    {
        if (!mIsAudioCheck) return;
        for (int i = 0;i < mAudioParam.Length; i++)
        {
            mAudioParam[i].volume = mAudioParam[i].source.volume;
            mAudioParam[i].pitch = mAudioParam[i].source.pitch;
        }
    }

    /// <summary>
    /// 現在の車の状況更新
    /// </summary>
    private void UpdateCarParam()
    {
        mCarParam.ThrottleInput = mInput.Accel.Value;
        mCarParam.BrakeInput = mInput.Brake.Value;
        mCarParam.TorqueInput = Mathf.Abs(mInput.Hundle.Value);
        mCarParam.VelocityInput = mDataUiCarModel.CurrentSpeed.Value;
        mCarParam.RpmInput = mDataUiCarModel.CurrentEnginePower.Value;
        mCarParam.RpmInput *= MaxRPM;  //最高回転数
        if (mCarParam.RpmInput < MinRPM) mCarParam.RpmInput = MinRPM;
    }

    /// <summary>
    /// 衝突時クラッシュ音をならす
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        mCarParam.HitVelocity = collision.relativeVelocity.magnitude;
    }

    /// <summary>
    /// ぶつかり続けている場合の音を鳴らす
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        mCarParam.HitContinueVelocity = collision.relativeVelocity.magnitude;
    }

    /// <summary>
    /// 衝突関連の音を止める
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        mCarParam.HitVelocity = 0f;
        mCarParam.HitContinueVelocity = 0f;
    }
}
