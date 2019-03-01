using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LREffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem mParticle;

    private void Awake()
    {
        GameSceneManager mGameSceneManager = FindObjectOfType<GameSceneManager>();
        RayConfig.LRRayConfig rayConfig = ((RayConfig)Resources.Load(ConstString.Path.RAY_CONFIG)).GetLRRayConfig;
        LRRay lRRay = new LRRay(transform, rayConfig);
        RaycastHit lHitInfo;
        RaycastHit rHitInfo;
        BoolReactiveProperty lHit = new BoolReactiveProperty();
        BoolReactiveProperty rHit = new BoolReactiveProperty();

        this.UpdateAsObservable()
            .Where(_ => mGameSceneManager.SceneState == SceneState.REPLAY)
            .Subscribe(_ =>
            {
                lHit.Value = lRRay.LBoxRay(out lHitInfo);
                rHit.Value = lRRay.RBoxRay(out rHitInfo);
            });

        this.OnCollisionEnterAsObservable()
            .Where(_ => mGameSceneManager.SceneState == SceneState.REPLAY)
            .Subscribe(_ =>
            {
                foreach (ContactPoint point in _.contacts) { Instantiate(mParticle, point.point, Quaternion.identity); }
            });
    }

}
