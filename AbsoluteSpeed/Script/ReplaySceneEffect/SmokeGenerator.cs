using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class SmokeGenerator : MonoBehaviour
{
    /// <summary>slip angleが値以上の時に煙を出す</summary>
    private const float SMOKE_ANGLE = 10.0f;
    /// <summary>slip angleが値以上の時は煙を出さない</summary>
    private const float IGNORE_ANGLE = 90.0f;
    /// <summary>1フレームの間に進む距離が値以下の場合無視する</summary>
    private const float IGNORE_LENGTH = 0.5f;
    [SerializeField,Tooltip("白煙パーティクルを入れてね")]
    private ParticleSystem[] mSmokes;

    private void Awake()
    {
        GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
        BoolReactiveProperty isDrift = new BoolReactiveProperty(false);
        Vector3 prePos = Vector3.zero;
        Vector3 direction = Vector3.zero;

        if (mSmokes.Length <= 0) { return; }

        this.UpdateAsObservable()
            .Where(_ => gameSceneManager.SceneState == SceneState.REPLAY)
            .Subscribe(_ =>
            {
                direction = transform.position - prePos;
                if (direction.magnitude < IGNORE_LENGTH)
                {
                    isDrift.Value = false;
                    return;
                }
                prePos = transform.position;
                float slipAngle = Vector3.Angle(direction, transform.forward);
                isDrift.Value = slipAngle > SMOKE_ANGLE && slipAngle < IGNORE_ANGLE;
            })
            .AddTo(gameObject);

        this.OnEnableAsObservable().Subscribe(_ => StopParticles(mSmokes));

        isDrift
            .Where(_ => gameSceneManager.SceneState == SceneState.REPLAY)
            .Where(_ => _)
            .Subscribe(_ => { PlayParticles(mSmokes); })
            .AddTo(gameObject);

        isDrift
            .Where(_ => gameSceneManager.SceneState == SceneState.REPLAY)
            .Where(_ => !_)
            .Subscribe(_ => { StopParticles(mSmokes); })
            .AddTo(gameObject);
    }

    private void StopParticles(ParticleSystem[] particles)
    {
        foreach (ParticleSystem particle in particles) { particle.Stop(); }
    }

    private void PlayParticles(ParticleSystem[] particles)
    {
        foreach (ParticleSystem particle in particles) { particle.Play(); }
    }

}
