using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public struct LogRecord<I, T>
{
    public I Value { get; }
    public T Count { get; }
    public LogRecord(I input, T count)
    {
        Value = input;
        Count = count;
    }
}

public class RePlayCalc
{
    private readonly float mInterval;
    private readonly Transform mTransform;
    private int mIndex;
    private float mLerpTimer;
    private float mRecTimer;
    private float mRePlayTimer;
    private float mIntervalTimer;
    private bool mIsRec;
    private bool mIsRePlay;
    private Vector3 mInitialPos;
    private Quaternion mInitialRot;
    private List<LogRecord<Vector3, float>> mPosLog;
    private List<LogRecord<Quaternion, float>> mRotLog;
    public RePlayCalc(Transform transform, float interval) { mTransform = transform; mInterval = interval; }

    /// <summary>
    /// 戻る秒数分logを削除して残ったlogの中で最新のデータを返す
    /// </summary>
    /// <returns>The recovery.</returns>
    public RecoveryData InsertRecovery()
    {
        int deleateIndexCount = RecoveryBoy.CalcRecoveryIndex(mInterval);
        for (int i = 0; i < deleateIndexCount; i++)
        {
            if (mPosLog.Count == 1) { break; }
            if (mRotLog.Count == 1) { break; }
            //最新のログから消していく
            mPosLog.Remove(mPosLog[(mPosLog.Count - 1)]);
            mRotLog.Remove(mRotLog[(mRotLog.Count - 1)]);
        }
        return new RecoveryData(mPosLog[mPosLog.Count-1].Value, mRotLog[mRotLog.Count-1].Value);
    }

    /// <summary>
    /// 録画開始準備
    /// </summary>
    public void StartRec()
    {
        if (mIsRec) { return; }
        mIsRec = true;
        mIndex = 0;
        mRecTimer = 0.0f;
        mInitialPos = mTransform.position;
        mInitialRot = mTransform.rotation;
        mPosLog = new List<LogRecord<Vector3, float>>();
        mRotLog = new List<LogRecord<Quaternion, float>>();
    }
    /// <summary>
    /// Updateで呼ぶ録画処理
    ///再生中でない場合、
    /// 読んでいる間データを保存し続ける
    /// もう一回再生し直す場合、再生を停止してReRec()を呼ぶ
    /// </summary>
    public void UpdateRec()//再生中呼ばれない
    {
        StartRec();
        if (!mIsRec) { return; }
        if (mIsRePlay) { return; }//再録画するには再生をやめる
        mRecTimer += Time.deltaTime;
        mIntervalTimer += Time.deltaTime;
        if (mIntervalTimer < mInterval) { return; }
        mIntervalTimer = 0.0f;
        AddLog();
    }

    ///<summary>データ追加処理</summary>
    public void AddLog()
    {
        if (!mIsRec) { return; }
        if (mIsRePlay) { return; }
        mPosLog.Add(new LogRecord<Vector3, float>(mTransform.position, mRecTimer));
        mRotLog.Add(new LogRecord<Quaternion, float>(mTransform.rotation, mRecTimer));
    }

    /// <summary>
    /// リプレイ開始準備
    /// UpdateRePlayで最初の1回だけ呼ぶ
    /// </summary>
    private void StartRePlay()
    {
        if (mIsRePlay) { return; }
        mIsRePlay = true;
        mIndex = 0;
        mRePlayTimer = 0.0f;
        mLerpTimer = 0.0f;
        mTransform.position = mInitialPos;
        mTransform.rotation = mInitialRot;
    }
    /// <summary>
    /// Updateで呼ぶリプレイ処理
    /// </summary>
    public void UpdateRePlay()
    {
        StartRePlay();
        if (!mIsRePlay) { return; }

        mRePlayTimer += Time.deltaTime;
        mTransform.position += CalcAddValue(mPosLog);
        mTransform.rotation = CalcRot(mRotLog);

        if (mRePlayTimer <= mPosLog[mIndex].Count) { return; }
        if (mIndex >= mPosLog.Count - 1) { return; }
        mTransform.position = mPosLog[mIndex + 1].Value;
        mTransform.rotation = mRotLog[mIndex + 1].Value;
        mIndex++;
        mLerpTimer = 0.0f;
    }
    /// <summary>
    /// positionのログを補完する処理
    /// </summary>
    /// <returns>The add value.</returns>
    /// <param name="log">Log.</param>
    private Vector3 CalcAddValue(List<LogRecord<Vector3, float>> log)
    {
        if (log.Count < 2) { throw new Exception("List length is insufficient!"); }
        if (mIndex >= mPosLog.Count - 2) { return Vector3.zero; }
        Vector3 diff = log[mIndex + 1].Value - log[mIndex].Value;
        float interval = log[mIndex + 1].Count - log[mIndex].Count;
        diff = (diff / interval) * Time.deltaTime;
        return diff;
    }
    /// <summary>
    /// rotationのログを補完する処理
    /// </summary>
    /// <returns>The rot.</returns>
    /// <param name="log">Log.</param>
    private Quaternion CalcRot(List<LogRecord<Quaternion, float>> log)
    {
        if (log.Count < 2) { throw new Exception("List length is insufficient!"); }
        if (mIndex >= mPosLog.Count - 2) { return Quaternion.identity; }
        float interval = log[mIndex + 1].Count - log[mIndex].Count;
        mLerpTimer += Time.deltaTime / interval;
        if (mLerpTimer > 1) { mLerpTimer = 1.0f; }
        Quaternion rot = Quaternion.Slerp(log[mIndex].Value, log[mIndex + 1].Value, mLerpTimer);
        return rot;
    }

    // 戻りたい時間/interval = 廃棄するログ
    // mIsRePlayをfalseにするとstartReplayから走り直させるので最初から再生される
    public void ReRePlay() { mIsRePlay = false; }
    /// <summary>
    /// リプレイを再生していない場合再録画する
    /// </summary>
    public void ReRec() { mIsRePlay = false; mIsRec = false; }

    public string OnGUITexts()
    {
        //string text = string.Format("\tPos Count {0}\n", mPosLog.Count);
        //text += string.Format("\tIndex{0}\n", mIndex);
        return "";  //return text;
    }
}
//停止時サンプリングする必要ない
//同じrotationの間サンプリングする必要ない
//最初に必要な量の配列を生成。足りない分は増やしていく。

