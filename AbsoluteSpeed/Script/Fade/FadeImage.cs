using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 画面全体をフェードアウト
/// </summary>
public class FadeImage : UnityEngine.UI.Graphic , IFade
{
	[SerializeField, Tooltip("マスクに使用するテクスチャ")]
	private Texture maskTexture = null;

	[SerializeField, Range (0, 1), Tooltip("マスクの閾値")]
	private float mCutoutThreshold;

    /// <summary>
    /// 閾値プロパティ
    /// </summary>
	public float Threshold
    {
		get {
			return mCutoutThreshold;
		}
		set {
            mCutoutThreshold = value;
			UpdateMaskCutout (mCutoutThreshold);
		}
	}

    /// <summary>
    /// フェードアウト開始
    /// </summary>
	protected override void Start ()
	{
		base.Start ();
		UpdateMaskTexture (maskTexture);
	}

    /// <summary>
    /// 遷移中のカットアウトを更新
    /// </summary>
    /// <param name="threshold"></param 閾値>
	private void UpdateMaskCutout (float threshold)
	{
		enabled = true;
		material.SetFloat ("_Range", 1 - threshold);

		if (threshold <= 0) {
			this.enabled = false;
		}
	}

    /// <summary>
    /// マスクのテクスチャを更新
    /// </summary>
    /// <param name="texture"></param マスクとして使用するテクスチャ>
	public void UpdateMaskTexture (Texture texture)
	{
		material.SetTexture ("_MaskTex", texture);
		material.SetColor ("_Color", color);
	}

    /// <summary>
    /// Unityエディター上の値を更新する
    /// </summary>
	#if UNITY_EDITOR
	protected override void OnValidate ()
	{
		base.OnValidate ();
		UpdateMaskCutout (mCutoutThreshold);
		UpdateMaskTexture (maskTexture);
	}
	#endif
}
