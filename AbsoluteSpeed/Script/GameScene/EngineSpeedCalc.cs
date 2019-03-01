using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class EngineSpeedCalc
{
    //値切り出す
    /// <summary>1のときペナルティ無し</summary>
    private float mWallPenalty = 1.0f;
    private float mDriftPenalty = 1.0f;
    private float mAiPenalty = 1.0f;
    private readonly VehicleSettings.EngineSettings mEngineSettings;
    private readonly VehicleSettings.PenaltySettings mPenaltySettings;
    /// <summary>車が出せるもっとも低い速度</summary>
    private readonly float mLowestSpeed;//clampの下限に使用する
    /// <summary>表示用エンジン回転数を計算クラス</summary>
    private readonly EngineRpmCalculator mEngineRpmCalculator;
    /// <summary>1秒あたりに加算する速度</summary>
    private float mAccPow;
    /// <summary>アクセルの入力が無いとき1秒あたりに減速する速度</summary>
    private float mEngineBrakePow;
    /// <summary>ギアを上げるのに必要な速度 </summary>
    private float mNeedSpeed;
    /// <summary>ギアで設定された最大回転数</summary>
    private float mMaxSpeed;
    /// <summary>1つ前のギアの設定された最大回転数</summary>
    private float mPreMaxSpeed;
    /// <summary>現在の最大回転数</summary>
    private float mCurrentMaxSpeed;
    /// <summary>現在のギア</summary>
    private IReadOnlyReactiveProperty<GearState> mCurrentGear;

    /// <summary>
    /// ■brakePower:ブレーキの強さ
    /// </summary>
    public EngineSpeedCalc(VehicleSettings.EngineSettings engineSettings, VehicleSettings.PenaltySettings penaltySettings,
                           GearParam gearParam, IReadOnlyReactiveProperty<GearState> gear)
    {
        mEngineSettings = engineSettings;
        mPenaltySettings = penaltySettings;
        mCurrentGear = gear;
        mLowestSpeed = gearParam.GetGearData(GearState.REVERSE).MaxSpeed;
        mEngineRpmCalculator = new EngineRpmCalculator(gearParam, gear);
        mCurrentGear
            .Buffer(2, 1)
            .Subscribe(x =>
        {
            //現在のギアデータ
            GearData gearData = gearParam.GetGearData(x.Last());
            //1つ前のギアデータ
            GearData lateGearData = gearParam.GetGearData(x.First());
            //現在のギアに必要な回転数を取得
            mNeedSpeed = gearData.NeedSpeed;
            //現在のギアの最大回転数を取得
            mMaxSpeed = gearData.MaxSpeed;
            //ギア変更前の最大回転数を取得
            mPreMaxSpeed = lateGearData.MaxSpeed;
            //エンジンブレーキの強さを取得
            mEngineBrakePow = gearData.EngineBrake;
            //1秒あたりのエンジン回転数を取得
            mAccPow = gearData.EngineRot;
        });
    }

    /*■■■PUBLIC■■■*/
    /// <summary>
    /// 速度を増減させる処理
    /// </summary>
    /// <param name="engineSpeed"></param>
    /// <param name="accel"></param>
    /// <returns></returns>
    public float CalcEngineSpeed(float engineSpeed, float accel)
    {
        accel = Mathf.Clamp01(accel);
        bool isNeutral = (mCurrentGear.Value == GearState.NEUTRAL);
        bool isReverse = (mCurrentGear.Value == GearState.REVERSE);
        bool isStop = (Mathf.Approximately(engineSpeed, 0.0f));
        if (isNeutral) { accel = 0.0f; }

        /*ギア変更後速度が現在の最高速度より高い場合、accelを0にする*/
        if (!isReverse && engineSpeed > mMaxSpeed) { accel = 0.0f; }
        /*ギアがニュートラル"でない"かつ、エンジン回転数が0以下だった場合、
          値を代入する。0だと動かなくなる為*/
        if (!isNeutral && isStop) { engineSpeed = VehicleSettings.EngineSettings.LOWEST_ENGINE_SPEED; }

        //回転数上げる処理。リバースギアのときはアクセル踏むと逆に進む
        if (!isReverse) { engineSpeed += SpeedCalculator.CalcRotToAdd(engineSpeed, mAccPow, accel, mNeedSpeed); }
        else { engineSpeed -= SpeedCalculator.CalcRotToAdd(engineSpeed, mAccPow, accel, mNeedSpeed); }

        //アクセルにあわせて回転数の限界を調節する処理
        /*リバースギアに入れた時に減速が早すぎる為。
          engine brakeの値を0にするとリバースギアで下がったら止まらなくなる*/
        if (!(isReverse && (engineSpeed > 0.0f))) { engineSpeed = SpeedLimitController.EngineBrake(engineSpeed, accel, mMaxSpeed, mEngineBrakePow); }

        //ギアを下げたとき最大回転数をlerpで逓減する処理
        mCurrentMaxSpeed = SpeedLimitController.DecreaseMaxSpeed(mMaxSpeed, mPreMaxSpeed, mCurrentMaxSpeed, mEngineSettings.DecreaseSpeed);

        //ペナルティ用
        Penalty();

        mCurrentMaxSpeed = Mathf.Abs(mCurrentMaxSpeed);
        //範囲内に収める処理
        return Mathf.Clamp(engineSpeed, mLowestSpeed, mCurrentMaxSpeed);
    }


    public float CalcEngineSpeedAT(float engineSpeed, float accel, float brake)
    {
        accel = Mathf.Clamp01(accel);
        bool isNeutral = (mCurrentGear.Value == GearState.NEUTRAL);
        bool isReverse = (mCurrentGear.Value == GearState.REVERSE);
        bool isStop = (Mathf.Approximately(engineSpeed, 0.0f));
        if (isNeutral) { accel = 0.0f; }


        /*ギアがニュートラル"でない"かつ、エンジン回転数が0以下だった場合、
          値を代入する。0だと動かなくなる為*/
        if (!isNeutral && isStop) { engineSpeed = VehicleSettings.EngineSettings.LOWEST_ENGINE_SPEED; }

        //回転数上げる処理。リバースギアのときはアクセル踏むと逆に進む
        if (!isReverse) { engineSpeed += SpeedCalculator.CalcRotToAdd(engineSpeed, mAccPow, accel, mNeedSpeed); }
        else { engineSpeed += SpeedCalculator.CalcRotToAdd(engineSpeed, mAccPow, accel, mNeedSpeed); }
        //アクセルにあわせて回転数の限界を調節する処理
        /*リバースギアに入れた時に減速が早すぎる為。
          engine brakeの値を0にするとリバースギアで下がったら止まらなくなる*/
        if (!(isReverse && (engineSpeed > 0.0f))) { engineSpeed = SpeedLimitController.EngineBrake(engineSpeed, accel, mMaxSpeed, mEngineBrakePow); }

        //ギアを下げたとき最大回転数をlerpで逓減する処理
        mCurrentMaxSpeed = SpeedLimitController.DecreaseMaxSpeed(mMaxSpeed, mPreMaxSpeed, mCurrentMaxSpeed, mEngineSettings.DecreaseSpeed);

        //ペナルティ用
        Penalty();

        mCurrentMaxSpeed = Mathf.Abs(mCurrentMaxSpeed);
        //範囲内に収める処理
        return Mathf.Clamp(engineSpeed, mLowestSpeed, mCurrentMaxSpeed);
    }


    /// <summary>
    /// スピードにブレーキをかける処理
    /// ■brake:入力
    /// ■engineSpeed:速度
    /// </summary>
    /// <param name="brake"></param>
    /// <param name="engineSpeed"></param>
    /// <returns></returns>
    public float Brake(float brake, float engineSpeed)
    {
        //reverse時のブレーキを有効にする為、符号を取ってから速度を減らしてる
        float sign = Mathf.Sign(engineSpeed);
        engineSpeed = Mathf.Abs(engineSpeed);
        engineSpeed += SpeedCalculator.CalcBrakeAmt(brake, mEngineSettings.BrakePower);
        engineSpeed = Mathf.Clamp(engineSpeed, 0.0f, Mathf.Infinity);
        engineSpeed *= sign;
        return engineSpeed;
    }


    public float BrakeAT(float brake, float engineSpeed)
    {
        float val = 1.0f;
        if (engineSpeed <= 0.0f) { val = 8.0f; }//エンジンブレーキが0に近づける力にブレーキが負ける為かける
        engineSpeed += SpeedCalculator.CalcBrakeAmt(brake, mEngineSettings.BrakePower) * val;
        engineSpeed = Mathf.Clamp(engineSpeed, mLowestSpeed, Mathf.Infinity);

        return engineSpeed;
    }



    public void Penalty()
    {
        if (Mathf.Approximately(mWallPenalty, mDriftPenalty) && Mathf.Approximately(mAiPenalty, mDriftPenalty)) { return; }
        if (mCurrentGear.Value == GearState.REVERSE) { return; }
        float penalty = new List<float> { mWallPenalty, mAiPenalty, mDriftPenalty }.OrderBy(x => x).First();

        float penaltyMax = penalty * mMaxSpeed;
        if (Mathf.Approximately(penalty, mWallPenalty))
        {
            mCurrentMaxSpeed = SpeedLimitController.DecreaseMaxSpeed(penaltyMax, mCurrentMaxSpeed, mCurrentMaxSpeed, mPenaltySettings.WallPenaltyDecreaseSpeed);
        }
        else if (Mathf.Approximately(penalty, mDriftPenalty))
        {
            mCurrentMaxSpeed = SpeedLimitController.DecreaseMaxSpeed(penaltyMax, mCurrentMaxSpeed, mCurrentMaxSpeed, mPenaltySettings.DriftPenaltyDecreaseSpeed);
        }
        else if (Mathf.Approximately(penalty, mAiPenalty))
        {
            mCurrentMaxSpeed = SpeedLimitController.DecreaseMaxSpeed(penaltyMax, mCurrentMaxSpeed, mCurrentMaxSpeed, mPenaltySettings.AiPenaltyDecreaseSpeed);
        }
    }

    /// <summary>
    /// 停止時から素早い加速をする処理
    /// </summary>
    /// <param name="accel"></param>
    /// <param name="engineSpeed"></param>
    /// <returns></returns>
    public float AccelarateStart(float accel, float engineSpeed, GearState currentGear)
    {
        if (currentGear == GearState.NEUTRAL) { return engineSpeed; }
        if (currentGear == GearState.REVERSE) { return engineSpeed; }
        //アクセルを踏んでないとき返す
        if (accel < 0.1f) { return engineSpeed; }
        if (engineSpeed > 5.0f) { return engineSpeed; }
        if (engineSpeed < 0.0f) { return engineSpeed; }

        engineSpeed += mEngineSettings.StartDash;
        return engineSpeed;
    }

    /// <summary>
    /// 表示用エンジン回転数を取得
    /// ■speed:車の速度
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float GetEngineRpm(float speed) { return mEngineRpmCalculator.CalcEngineRpm(speed); }
    /*■■■PUBLIC■■■*/

    public void GiveWallPenalty(bool give)
    {
        if (give) { mWallPenalty = mPenaltySettings.WallPenalty; }
        else { mWallPenalty = 1.0f; }
    }

    public void GiveDriftPenalty(bool isDrift, float slipAngle)
    {
        if (isDrift)
        {
            //slipを比に変える
            float slipRate = slipAngle / mPenaltySettings.MaxSlipAngle;
            // slipRate = Mathf.Clamp01(slipRate);
            //float inverseSlipRate = 1f - slipRate;//slip angleが大きい時drift penaltyを最大にしたい為
            // float penalty = inverseSlipRate * mEngineSettings.DriftPenalty;
            float penalty = 1f - (slipRate * (1f - mPenaltySettings.DriftPenalty));//DriftPenalty以下にはならないようにした
            mDriftPenalty = penalty;
        }
        else { mDriftPenalty = 1.0f; }
    }

    public void GiveAiPenalty(bool isCollision)
    {
        if (isCollision) { mAiPenalty = mPenaltySettings.AiPenalty; }
        else { mAiPenalty = 1.0f; }

    }
}
/*option shift command left 折りたたみ*/
