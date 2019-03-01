using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static FanStaticController;

/// <summary>
/// サーキュレーターをコントロールするクラス
/// </summary>
public class FanManager
{
    //風量を０にするスピード
    public float mStopFanSpeed { get; private set; }
    //風量中にするスピード
    public float mMiddleFanSpeed { get; private set; }
    //風量強にするスピード
    public float mHighFanSpeed { get; private set; }

    /// <summary>
    /// プロセスの開始と速度の閾値を設定できるコンストラクタ
    /// </summary>
    /// <param name="stopSpeed">サーキュレーターを止める速度</param>
    /// <param name="middleSpeed">風量を中にする速度</param>
    /// <param name="highSpeed">風量を強にする速度</param>
    public FanManager(float stopSpeed = 0f, float middleSpeed = 50f, float highSpeed = 100f)
    {
        AppController.StartProcess();
        SetChangeFanSpeedValue(stopSpeed, middleSpeed, highSpeed);
    }

    public void GetProcess()
    {
        AppController.GetProcess();
    }

    //~FanManager()
    //{
    //    KillProcess();
    //}

    /// <summary>
    /// サーキュレーターの風量切り替えの閾値を設定する
    /// </summary>
    /// <param name="stopSpeed">サーキュレーターを止める速度</param>
    /// <param name="middleSpeed">風量を中にする速度</param>
    /// <param name="highSpeed">風量を強にする速度</param>
    public void SetChangeFanSpeedValue(float stopSpeed, float middleSpeed, float highSpeed)
    {
        mStopFanSpeed = stopSpeed;
        mMiddleFanSpeed = middleSpeed;
        mHighFanSpeed = highSpeed;
    } 

    /// <summary>
    /// 電源をつける
    /// </summary>
    public void PowerOn()
    {
        ChangePowerSwitchOn();
    }

    /// <summary>
    /// 電源を切る
    /// </summary>
    public void PowerOff()
    {
        ChangePowerSwitchOff();
    }

    /// <summary>
    /// スピードに応じて風量を切り替える
    /// </summary>
    /// <param name="speed"></param>
    public void ChangeAirFlow(float speed)
    {
        if(speed < mStopFanSpeed) PowerOff();
        else if(speed < mMiddleFanSpeed) ChangeAirFlowStatic(FanPower.ONE);
        else if(speed < mHighFanSpeed) ChangeAirFlowStatic(FanPower.TWO);
        else ChangeAirFlowStatic(FanPower.THREE);
    }

    /// <summary>
    /// プロセスを終了する
    /// </summary>
    public void KillProcess()
    {
        PowerOff();
        AppController.KillProcess();
    }
}
