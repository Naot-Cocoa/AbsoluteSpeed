using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class FFB : MonoBehaviour
{
    private Rigidbody mRigidbody;
    private bool forceFeedbackEnabled = false;
    //ぶつかりながらさらにぶつかって振動しまくるのを防ぐ
    private bool mIsHit = false;
    private bool mCanVibration = true;
    //ぶつかったりした時の振動の繰り返し間隔
    //0.2秒ぐらいがちょうどいいと思った
    private readonly float mVibrationTime = 0.2f;
    //ジョイスティックが接続されているか
    private bool mIsConnect;
    //Vibrationコルーチンを取得
    private Coroutine mVibration;
    [DllImport("user32")]
    private static extern int GetForegroundWindow();

    // Import functions from DirectInput c++ wrapper dll
    [DllImport("MoDyEnPhysics64")]
    private static extern int InitDirectInput(int HWND);

    [DllImport("MoDyEnPhysics64")]
    private static extern void Aquire();

    //指定した力をそれぞれの方向にかける
    [DllImport("MoDyEnPhysics64")]
    private static extern int SetDeviceForcesXY(int x, int y);

    [DllImport("MoDyEnPhysics64")]
    private static extern bool StartEffect();

    [DllImport("MoDyEnPhysics64")]
    private static extern bool StopEffect();

    [DllImport("MoDyEnPhysics64")]
    private static extern bool SetAutoCenter(bool autoCentre);

    [DllImport("MoDyEnPhysics64")]
    private static extern void LedsPlay(float currentRPM, float rpmFirstLedTurnsOn, float rpmRedLine);

    //デフォルトの入力状態に戻す
    //再び振動させるためには再初期化が必要？
    [DllImport("MoDyEnPhysics64")]
    private static extern void FreeDirectInput();

    [SerializeField, Tooltip("振動パラメータ")]
    private VibrationParam mVibrationParam;

    [SerializeField, Tooltip("テスト用　テストしたいパラメータの番号を入れる")]
    private int mVibrationParamNum = 0;

    private GameSceneManager mGameSceneManager;

    private void Awake()
    {
        mIsConnect = Input.GetJoystickNames().Length > 0;
        //ジョイスティックがつながっていなければこのコンポーネントを削除する
        //つながっていない状態で振動させようとするとUnityがクラッシュするから
        if (!mIsConnect) Destroy(this);
        if (GetComponent<HundleControllerInput>() == null) Destroy(this);
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
    }

    private void OnEnable()
    {
        if (mGameSceneManager.SceneState != SceneState.GAME) return;
        var core = GetComponent<VehicleCore>();
        mRigidbody = GetComponent<Rigidbody>();
        mIsHit = false;
        forceFeedbackEnabled = false;
        mCanVibration = true;
        //壁にぶつかったときの振動処理
        this.OnCollisionEnterAsObservable()
            .Where(x => !mIsHit)
            .TakeUntil(this.OnDisableAsObservable())
            .ThrottleFirst(System.TimeSpan.FromSeconds(1f))
            .Subscribe(x => Hit(x))
            .AddTo(gameObject);

        //インスペクタ上で設定した値の振動をテストする
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.T))
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ => TestVibration())
            .AddTo(gameObject);
        //強制的に振動を止めて入力を初期状態に戻す
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ => HandleVibration(0, 0f))
            .AddTo(gameObject);

        //強制的に振動を止めて入力を初期状態に戻す
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.PageDown))
            .TakeUntil(this.OnDisableAsObservable())
            .Subscribe(_ => HandleVibration(0, 0f))
            .AddTo(gameObject);
    }

    /// <summary>
    /// 指定した力と時間でハンドルを揺らす
    /// </summary>
    /// <param name="power">かける力</param>
    /// <param name="time">力をかける時間間隔(短いと細かく長いと大きく揺れる)</param>
    public void Vibration(int power,float time)
    {
        if (mGameSceneManager.SceneState != SceneState.GAME) return;
        //HandleVibration(power,time);
        if(power == 0 && time == 0f)
        {
            StopFFB();
            return;
        }
        HitFFB(power, time);
    }

    /// <summary>
    /// テスト用
    /// 指定したパラメータの振動を起こす
    /// </summary>
    private void TestVibration()
    {
        if (mVibrationParamNum >= mVibrationParam.mVibrationParamList.Count)
        {
            Debug.LogError("配列要素が無いよ");
            return;
        }
        var param = mVibrationParam.mVibrationParamList[mVibrationParamNum];
        HandleVibration(param.Power, param.AddPowerTime);
    }

    /// <summary>
    /// 壁にぶつかったときの振動
    /// </summary>
    /// <param name="collision"></param>
    private void Hit(Collision collision)
    {
        if (mGameSceneManager.SceneState != SceneState.GAME) return;
        if (mIsHit) return;
        var velocityForce = CalcVibrationForce(collision.relativeVelocity);
        var point = collision.contacts.First().point;
        var pos = transform.position - point;
        if (pos.x < 0) HitFFB(-velocityForce, mVibrationTime);
        else if (pos.x >= 0) HitFFB(velocityForce, mVibrationTime);
    }

    /// <summary>
    /// 渡された値から実際に振動に使用する力を計算して返す
    /// </summary>
    /// <param name="force">振動させる力</param>
    /// <returns></returns>
    private int CalcVibrationForce(Vector3 force)
    {
        //今現状の最高速度でぶつかったときが5000ぐらいだったので2倍している
        return Mathf.RoundToInt(Mathf.Clamp(force.sqrMagnitude * 2, -10000f, 10000f));
    }

    /// <summary>
    /// 壁にぶつかったときの振動
    /// </summary>
    /// <param name="force">振動させる力</param>
    /// <param name="vibrationTime">振動させる時間</param>
    public void HitFFB(int force, float vibrationTime)
    {
        if (mGameSceneManager.SceneState != SceneState.GAME) return;
        mIsHit = true;
        UpdateFFB(-force);
        Observable.Timer(System.TimeSpan.FromSeconds(vibrationTime))
            .Subscribe(_ => { StopFFB(); mIsHit = false; })
            .AddTo(gameObject);
    }

    /// <summary>
    /// 振動を止める
    /// </summary>
    private void StopFFB()
    {
        FreeDirectInput();
        forceFeedbackEnabled = false;
    }

    public void UpdateLeds(float currentRPM, float rpmFirstLedTurnsOn, float rpmRedLine)
    {
        if (mIsConnect) LedsPlay(currentRPM, rpmFirstLedTurnsOn, rpmRedLine);
    }

    public void UpdateFFB(int ffforce)
    {
        if (mGameSceneManager.SceneState != SceneState.GAME) return;
        //初期化されていなければ初期化する
        if (!forceFeedbackEnabled) InitialiseForceFeedback();
        SetDeviceForcesXY(ffforce, 0);
    }

    public void OnApplicationQuit()
    {
        if (mIsConnect) ShutDownForceFeedback();
    }

    private void InitialiseForceFeedback()
    {
        if (forceFeedbackEnabled) return;
        int hwnd = GetForegroundWindow();
        InitDirectInput(hwnd);
        Aquire();
        StartEffect();
        forceFeedbackEnabled = true;
    }

    private void ShutDownForceFeedback()
    {
        StopEffect();
        if (forceFeedbackEnabled) FreeDirectInput();        
    }

    /// <summary>
    /// 振動を起こす命令を呼びます。止めるときは両方に0を入れて呼んでください
    /// </summary>
    /// <param name="force">振動の強さです</param>
    /// <param name="vibrationTime">振動の間隔です</param>
    public void HandleVibration(int force, float vibrationTime)
    {
        if (force == 0 && vibrationTime == 0)//止めます
        {
            StopFFB();
            StopAllCoroutines();
            mCanVibration = false;
        }
        else
        {
            mCanVibration = true;
            InitialiseForceFeedback();
            var waitTime = new WaitForSeconds(vibrationTime);
            mVibration = StartCoroutine(Vibration(force, waitTime));
        }
    }

    /// <summary>
    /// 振動起こしています
    /// </summary>
    /// <returns></returns>
    private IEnumerator Vibration(int force, WaitForSeconds waitTime)
    {
        while (mCanVibration)
        {
            UpdateFFB(force *= -1);
            yield return waitTime;
        }
    }
}
