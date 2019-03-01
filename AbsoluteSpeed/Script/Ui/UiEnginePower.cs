using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UiEnginePower : UiSuperClass
{
    [SerializeField, Tooltip("回転数を表示するUIのImageコンポーネント")]
    private Image mUiEnginePowerImage;

    private const float mMaxEnginePower = 1.0f;    //最大回転数
    private const float mMinEnginePower = 0.23f;    //最小回転数
    private const float RED_ZONE = 1.0f;
    private const float COLOR_CHANGE_ZONE = 0.9f;//超えたあたりから色の変化を開始する

    private float mPreRPM = 0.0f;
    private Color mSafeColor;
    private Color mDangerColor = Color.red;

    private GameStateManager mGameStateManager;
    private float mReadyRPM = 0.0f;
    protected override void Awake()
    {
        base.Awake();
        mSafeColor = mUiEnginePowerImage.color;
        mGameStateManager = FindObjectOfType<GameStateManager>();
    }

    private void OnEnable()
    {
        mUiEnginePowerImage.color = mSafeColor;mUiEnginePowerImage.fillAmount = 0.0f;
        mReadyRPM = 0.0f;

        //空ぶかしの時UIを動かす
        //InGameStateがREADYでなくなった時ストリームを破棄する
        this.UpdateAsObservable()
            .TakeUntil(this.OnDisableAsObservable())
            .TakeWhile(_ => mGameStateManager.CurrentGameState.Value == InGameState.READY)
            .Subscribe(_ => UiUpdateReady(), () => { mReadyRPM = 0.0f; UiUpdatePlay(mReadyRPM); });

    }

    /// <summary>
    /// InGameStateがREADYでない時のみ渡された値に応じたUi更新処理を呼ぶ
    /// </summary>
    /// <param name="currentEnginePowerValue"></param>
    public override void UiUpdate(float currentEnginePowerValue)
    {
        if(mGameStateManager.CurrentGameState.Value != InGameState.READY)
        {
            UiUpdatePlay(currentEnginePowerValue);
        }
    }

    /// <summary>
    /// 空ぶかしの時UIを動かすための計算を行う
    /// </summary>
    private void UiUpdateReady()
    {
        var accelInput = Input.GetAxisRaw(ConstString.Input.ACCEL);
        var verticalInput = Input.GetAxisRaw(ConstString.Input.VERTICAL);
        if (accelInput >= 0.5f)
        {
            mReadyRPM += accelInput * Time.deltaTime;
        }
        else if (verticalInput >= 0.5f)
        {
            mReadyRPM += verticalInput * Time.deltaTime;
        }
        else if(mReadyRPM > 0)
        {
            mReadyRPM -= Time.deltaTime;
        }
        UiUpdatePlay(Mathf.Clamp01(mReadyRPM));
    }

    /// <summary>
    /// 元々のUiUpdate
    /// </summary>
    /// <param name="currentEnginePowerValue"></param>
    private void UiUpdatePlay(float currentEnginePowerValue)
    {
        var tmpValue = currentEnginePowerValue;
        tmpValue = Mathf.Clamp(tmpValue, mMinEnginePower, mMaxEnginePower);

        var rate = (currentEnginePowerValue - COLOR_CHANGE_ZONE) / (RED_ZONE - COLOR_CHANGE_ZONE);
        rate = Mathf.Clamp01(rate);
        mUiEnginePowerImage.color = Color.Lerp(mSafeColor, mDangerColor, rate);

        var fillAmount = Mathf.Lerp(mPreRPM, tmpValue, 0.5f);
        if (!float.IsNaN(fillAmount)) mUiEnginePowerImage.fillAmount = Mathf.Lerp(mPreRPM, tmpValue, 0.5f);
        mPreRPM = currentEnginePowerValue;
    }
}