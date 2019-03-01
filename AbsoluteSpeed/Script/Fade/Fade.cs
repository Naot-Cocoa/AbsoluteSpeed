using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UniRx;

public class Fade : MonoBehaviour
{
	private IFade mFade;
    private float mCutoutThreshold = 1;
    public float CutoutThreshold { get { return mCutoutThreshold; } private set { } }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake ()
	{
		Init ();
		mFade.Threshold = mCutoutThreshold;
	}

    /// <summary>
    /// 初期化
    /// </summary>
	private void Init ()
	{

		mFade = GetComponent<IFade> ();
	}

    /// <summary>
    /// UnityEditor上の値を更新
    /// </summary>
	private void OnValidate ()
	{
		Init ();
		mFade.Threshold = mCutoutThreshold;
	}

    /// <summary>
    /// フェードアウト処理
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    /// <returns></returns>
	private IEnumerator FadeoutCoroutine (float time, System.Action action)
	{
		float endTime = Time.timeSinceLevelLoad + time * (mCutoutThreshold);

		var endFrame = new WaitForEndOfFrame ();

        //フェードアウト処理
		while (Time.timeSinceLevelLoad <= endTime) {
            mCutoutThreshold = (endTime - Time.timeSinceLevelLoad) / time;
			mFade.Threshold = mCutoutThreshold;
			yield return endFrame;
		}
        mCutoutThreshold = 0;
		mFade.Threshold = mCutoutThreshold;

		if (action != null) {
			action ();
		}
	}

    /// <summary>
    /// フェードイン処理
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator FadeinCoroutine (float time, System.Action action)
	{
		float endTime = Time.timeSinceLevelLoad + time * (1 - mCutoutThreshold);
		
        //レンダリング後に再開させるための返り値
		var endFrame = new WaitForEndOfFrame ();

		while (Time.timeSinceLevelLoad <= endTime) {
            mCutoutThreshold = 1 - ((endTime - Time.timeSinceLevelLoad) / time);
			mFade.Threshold = mCutoutThreshold;
            
			yield return endFrame;
		}
        mCutoutThreshold = 1;
		mFade.Threshold = mCutoutThreshold;

		if (action != null) {
			action ();
		}
	}

    /// <summary>
    /// フェードアウト処理起動
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    /// <returns></returns>
	public Coroutine FadeOut (float time, System.Action action)
	{
		StopAllCoroutines ();
		return StartCoroutine (FadeoutCoroutine (time, action));
	}

    /// <summary>
    /// フェードアウト処理起動
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
	public Coroutine FadeOut (float time)
	{
        return FadeOut (time, null);
	}

    /// <summary>
    /// フェードイン処理起動
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    /// <returns></returns>
	public Coroutine FadeIn (float time, System.Action action)
	{
		StopAllCoroutines ();
		return StartCoroutine (FadeinCoroutine (time, action));
	}

    /// <summary>
    /// フェードイン処理起動
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
	public Coroutine FadeIn (float time)
	{
		return FadeIn (time, null);
	}
}