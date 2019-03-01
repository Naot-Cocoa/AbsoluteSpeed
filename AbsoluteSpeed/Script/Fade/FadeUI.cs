using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// UIのみをフェードさせる
/// </summary>
[RequireComponent (typeof(RawImage))]
[RequireComponent (typeof(Mask))]
public class FadeUI : MonoBehaviour, IFade
{

	[SerializeField, Range (0, 1), Tooltip("マスク閾値")]
	private float mCutoutThreshold;
    [SerializeField, Tooltip("適用させるマテリアル")]
    private Material mMaterial = null;
    [SerializeField, Tooltip("適用させるRenderTexture")]
    private RenderTexture mRenderTexture = null;
    [SerializeField, Tooltip("ルール画像")]
    private Texture mRuleTexture = null;

    /// <summary>
    /// マスク閾値プロパティ
    /// </summary>
	public float Threshold
    {
		get {
			return mCutoutThreshold;
		}
		set {
            mCutoutThreshold = value;
			UpdateMaskCutout(mCutoutThreshold);
		}
	}

    /// <summary>
    /// 遷移中のカットアウトを更新
    /// </summary>
    /// <param name="threshold"></param マスク閾値>
	private void UpdateMaskCutout (float threshold)
	{
        //マテリアルに値を適用
        mMaterial.SetFloat ("_Range", threshold);
		
        //設定したテクスチャをレンダリングするテクスチャへと変換
		UnityEngine.Graphics.Blit (mRuleTexture, mRenderTexture, mMaterial);
		
        //マスクで画面表示を制限
		var mask = GetComponent<Mask> ();
		mask.enabled = false;
		mask.enabled = true;
	}
}