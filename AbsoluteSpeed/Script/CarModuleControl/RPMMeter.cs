using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RPMMeter : MonoBehaviour
{
    private float mPreRote;
    private GameStateManager mGameStateManager;
    private GameSceneManager mGameSceneManager;
    private DataUiCarModel mDataUiCarModel;
    private float mReadyRPM = 0.0f;
    [SerializeField]
    private float mLerpValue = 0.5f;
    [SerializeField]
    private float mStartZ = 1.907f;
    [SerializeField]
    private float mEndZ = -266.75f;

    private void Awake()
    {
        mGameStateManager = FindObjectOfType<GameStateManager>();
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        mDataUiCarModel = FindObjectOfType<DataUiCarModel>();
    }

    private void OnEnable()
    {
        mPreRote = mStartZ;
        mReadyRPM = 0.0f;
       
        this.UpdateAsObservable()
            .TakeUntil(this.OnDisableAsObservable())
            .Where(_=> mGameStateManager.CurrentGameState.Value != InGameState.READY)
            .Subscribe(_ => CalcRote(mDataUiCarModel.CurrentEnginePower.Value));

        //空ぶかしの時メーターを動かす
        //SceneStateがREPLAYになったときストリームを破棄する
        //InGameStateがREADYでなくなった時ストリームを破棄する
        this.UpdateAsObservable()
            .TakeUntil(this.OnDisableAsObservable())
            .TakeWhile(_ => mGameSceneManager.SceneState != SceneState.REPLAY)
            .TakeWhile(_ => mGameStateManager.CurrentGameState.Value == InGameState.READY)
            .Subscribe(_ => CalcRoteReady(),() => { mReadyRPM = 0.0f; CalcRote(mReadyRPM); });
    }

    /// <summary>
    /// メーターを動かす
    /// </summary>
    /// <param name="rpm"></param>
    private void CalcRote(float rpm)
    {
        if (float.IsNaN(rpm)) { return; }
        var rote = Mathf.Lerp(mStartZ, mEndZ, rpm);
        var roteLerp = Mathf.Lerp(mPreRote, rote, mLerpValue);
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, roteLerp);
        mPreRote = roteLerp;
    }

    /// <summary>
    /// 空ぶかしの時メーターを動かすための計算を行う
    /// </summary>
    private void CalcRoteReady()
    {
        var accelInput = Input.GetAxisRaw(ConstString.Input.ACCEL);
        var verticalInput = Input.GetAxisRaw(ConstString.Input.VERTICAL);
        if (accelInput >= 0.5f)
        {
            mReadyRPM += accelInput * Time.deltaTime;
        }
        else if(verticalInput >= 0.5f)
        {
            mReadyRPM += verticalInput * Time.deltaTime;
        }
        else if (mReadyRPM > 0)
        {
            mReadyRPM -= Time.deltaTime;
        }
        CalcRote(Mathf.Clamp01(mReadyRPM));
    }
}
