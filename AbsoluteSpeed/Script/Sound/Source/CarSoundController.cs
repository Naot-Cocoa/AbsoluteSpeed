// v 1.1
// - added gearwhine sounds

using System.Linq;
using UniRx;
using UnityEngine;

public class CarSoundController : MonoBehaviour
{

    public struct AudioSamples
    {
        public AudioClip[] powerSamples;
        public AudioClip[] coastSamples;
        public Vector3[] powerBlendRpm;
        public Vector3[] coastBlendRpm;
        public AudioSource[] powerSamplesSource;
        public AudioSource[] coastSamplesSource;
        public int powerSamplesLength;
        public int coastSamplesLength;
        public AudioClip[] gearWhineSamples;
        public AudioSource[] gearWhineSamplesSource;
        public Vector3[] gearWhineBlendRpm;
        public int gearWhineSamplesLength;
    }
    public AudioClip[] powerSamples;
    public AudioClip[] coastSamples;
    public Vector3[] powerTransferRpm = {
        new Vector3(  40.00f,  3000.00f,  5500),
        new Vector3(2000.00f,  5000.00f,  5500),
        new Vector3(4000.00f,  7000.00f,  5500),
        new Vector3(6000.00f,  10000.00f,  5500)
    };
    public Vector3[] coastTransferRpm = {
        new Vector3(  40.00f,  3000.00f,  5500),
        new Vector3(2000.00f,  5000.00f,  5500),
        new Vector3(4000.00f,  7000.00f,  5500),
        new Vector3(6000.00f,  10000.00f,  5500)
    };
    public float powerVolume = 0.6f;
    public float coastVolume = 0.4f;
    public AudioClip[] gearWhineSamples;
    public Vector3[] gearWhineTransferRpm = {
        new Vector3(  40.00f,  2500.00f,  400),
        new Vector3(2000.00f,  10000.00f,  400)
    };
    public float gearWhineTransmissionTorque = 300;
    public float gearWhineVolume = 0.5f;

    public SoundParam mSoundParam;
    // Exterrior audi samples
    public AudioSamples audioSamples = new AudioSamples();

    public AudioClip windRush;
    public AudioClip skid;
    public AudioClip roadRoll;
    public AudioClip brake;
    public AudioClip collision;
    public AudioClip collisionScrape;

    public float maxRpm = 10000;
    public float engineVolume = 0.5f;
    public float chargerVolume = 0.5f;
    public float tireVolume = 0.5f;
    public float localVolume = 0.4f;

    private AudioSource engineOnSource;
    private AudioSource engineOffSource;
    private AudioSource windRushSource;
    //スリップ
    private AudioSource skidSource;
    //道走ってるとき
    private AudioSource roadRollSource;    
    //ブレーキ音
    private AudioSource brakeSource;
    private AudioSource collisionSource;
    private AudioSource collisionScrapeSource;
    
    private GameObject audioHolder;

    private float throttleInput = 0f;
    private float brakeInput = 0f;
    private float rpmInput = 0f, rpmSmooth = 0f;
    private float driveshaftRpmInput = 0f, driveshaftRpmSmooth = 0f; 
    private float torqueInput = 0f;
    private float velocityInput = 0f;

    private IPlayerInput mPlayerInput;
    private DataUiCarModel mDataUiCarModel;
    private Rigidbody mRigidbody;
    private Vector3 mPreVelocity = Vector3.zero;
    private bool mIsInit = false;
    private GameSceneManager mGameSceneManager;
    private GameStateManager mGameStateManager;
    private IPlayerInput mInput;
    private string mAccelString;

    private void Awake()
    {
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        mGameStateManager = FindObjectOfType<GameStateManager>();
        mInput = GetComponent<IPlayerInput>();
        //入力クラスによってAccelに割り当てるキーを変更する
        if(mInput is HundleControllerInput || mInput is ControllerInput)
        {
            mAccelString = ConstString.Input.ACCEL;
        }
        else if(mInput is KeyBoardInput || mInput is MacKeyBoardInput)
        {
            mAccelString = ConstString.Input.VERTICAL;
        }
    }

    /*各AudioClipごとのGameObjectを作り最初に作った親オブジェクト配下にする*/
    private void Initialized()
    {
        //初期化済みなら値のリセットのみ行う
        if (mIsInit)
        {
            windRushSource.volume = 0f;
            windRushSource.pitch = 1f;
            skidSource.volume = 0f;
            skidSource.pitch = 1f;
            brakeSource.volume = 0f;
            brakeSource.pitch = 1f;
            roadRollSource.volume = 0f;
            roadRollSource.pitch = 1f;
            collisionSource.volume = 0f;
            collisionSource.pitch = 1f;
            collisionScrapeSource.volume = 0f;
            collisionScrapeSource.pitch = 1f;
            foreach(var x in audioSamples.powerSamplesSource.Concat(audioSamples.coastSamplesSource))
            {
                x.volume = 0f;
                x.pitch = 1f;
            }
            return;
        }
        mIsInit = true;
        mRigidbody = GetComponent<Rigidbody>();
        mPreVelocity = mRigidbody.velocity;
        audioHolder = new GameObject("_Audio");
        audioHolder.transform.parent = transform;
        mPlayerInput = GetComponent<IPlayerInput>();
        mDataUiCarModel = FindObjectOfType<DataUiCarModel>();
        if(windRush)windRushSource = CreateAudioSource(windRush);
        if(skid)skidSource = CreateAudioSource(skid);
        if(roadRoll)roadRollSource = CreateAudioSource(roadRoll);      
        if(brake)brakeSource = CreateAudioSource(brake);
        if(collision)collisionSource = CreateAudioSource(collision);
        if (collisionSource) collisionSource.loop = false;
        collisionScrapeSource = CreateAudioSource(collisionScrape);

        // Samples Set1 
        audioSamples.coastBlendRpm = coastTransferRpm;
        audioSamples.powerBlendRpm = powerTransferRpm;
        audioSamples.gearWhineBlendRpm = gearWhineTransferRpm;
        audioSamples.coastSamples = coastSamples;
        audioSamples.powerSamples = powerSamples;
        audioSamples.gearWhineSamples = gearWhineSamples;

        // Create onSampleSources
        if (powerSamples != null)
        {
            audioSamples.powerSamplesLength = powerSamples.Length;
            audioSamples.powerSamplesSource = new AudioSource[audioSamples.powerSamplesLength];
            for (int i = 0; i < audioSamples.powerSamplesLength; i++)
                audioSamples.powerSamplesSource[i] = CreateAudioSource(audioSamples.powerSamples[i]);
        }
        // Create onSampleSources
        if (coastSamples != null)
        {
            audioSamples.coastSamplesLength = coastSamples.Length;
            audioSamples.coastSamplesSource = new AudioSource[audioSamples.coastSamplesLength];
            for (int i = 0; i < audioSamples.coastSamplesLength; i++)
                audioSamples.coastSamplesSource[i] = CreateAudioSource(audioSamples.coastSamples[i]);
        }

        // Create gearwhineSources
        if (gearWhineSamples != null)
        {
            audioSamples.gearWhineSamplesLength = gearWhineSamples.Length;
            audioSamples.gearWhineSamplesSource = new AudioSource[audioSamples.gearWhineSamplesLength];
            for (int i = 0; i < audioSamples.gearWhineSamplesLength; i++)
                audioSamples.gearWhineSamplesSource[i] = CreateAudioSource(audioSamples.gearWhineSamples[i]);
        }
    }

    /*アプリ終了時生成したAudioClipを全て破棄する*/

    private void OnApplicationQuit()
    {
        RemoveAudioSource(ref windRushSource);
        RemoveAudioSource(ref skidSource);
        RemoveAudioSource(ref roadRollSource);
        RemoveAudioSource(ref brakeSource);
        RemoveAudioSource(ref collisionSource);
        RemoveAudioSource(ref collisionScrapeSource);

        if (powerSamples != null)
        {
            for (int i = 0; i < audioSamples.powerSamplesLength; i++)
                RemoveAudioSource(ref audioSamples.powerSamplesSource[i]);
        }

        if (coastSamples != null)
        {
            for (int i = 0; i < audioSamples.coastSamplesLength; i++)
                RemoveAudioSource(ref audioSamples.coastSamplesSource[i]);
        }

        if (gearWhineSamples != null)
        {
            for (int i = 0; i < audioSamples.gearWhineSamplesLength; i++)
                RemoveAudioSource(ref audioSamples.gearWhineSamplesSource[i]);
        }
    }

    /*衝突時に鳴る音のボリュームの初期化*/

    private void OnEnable()
    {
        Initialized();
        if (collisionSource) collisionSource.volume = 0f;
        if (collisionScrapeSource) collisionScrapeSource.volume = 0f;
    }

    /*衝突時に一定速度以上なら音を鳴らす*/

    // Collision impact sound	
    private void OnCollisionEnter(Collision collision)
    {
        if (mRigidbody == null) return;
        var contactVelocity = collision.relativeVelocity.magnitude;
        if (collisionSource)
        {
            if (contactVelocity > mSoundParam.JudgeCrashSpeed)
            {
                collisionSource.volume = Mathf.Abs(contactVelocity) / mSoundParam.CollisionVolumeRate * localVolume;
                collisionSource.pitch = 1f + mSoundParam.CollisionPitchAdd;
                collisionSource.Play();
            }           
            collisionScrapeSource.Play();
        }
    }

    /*衝突時に鳴る音を止める*/

    private void OnCollisionExit(Collision collision)
    {
        if (mRigidbody == null) return;
        // play a colllision exit sound!
        if (collisionScrapeSource)
        {
            collisionScrapeSource.Stop();
        }
    }

    /*衝突している間速度に応じて音量とピッチを変更する*/

    private void OnCollisionStay(Collision collision)
    {
        if (mRigidbody == null) return;
        var contactVelocity = collision.relativeVelocity.magnitude;
        if (collisionScrapeSource)
        {
            collisionScrapeSource.volume = Mathf.Abs(contactVelocity) / mSoundParam.CollisionScrapeVolumeRate;
            collisionScrapeSource.pitch = 1 + (Mathf.Abs(contactVelocity) / mSoundParam.CollisionScrapePitchRate);
        }
    }

    /// <summary>
    /// 現在の車の状況更新
    /// </summary>
    private void ParseCar()
    {
        //GAMEシーンでなければ音を鳴らさない
        if(mGameSceneManager.SceneState != SceneState.GAME)
        {
            throttleInput = 0f;
            brakeInput = 0f;
            torqueInput = 0f;
            velocityInput = 0f;
            rpmInput = 0f;
            return;
        }
        throttleInput = mPlayerInput.Accel.Value;
        brakeInput = mPlayerInput.Brake.Value;
        torqueInput = Mathf.Abs(mPlayerInput.Hundle.Value);
        velocityInput = mDataUiCarModel.CurrentSpeed.Value;       
        rpmInput = mDataUiCarModel.CurrentEnginePower.Value;
        rpmInput *= 8000f;  //最高回転数
        if (float.IsNaN(rpmInput)) rpmInput = 100.0f;
        if (rpmInput < 100.0f) rpmInput = 100.0f;
    }

    /// <summary>
    /// 音量初期化
    /// </summary>
    private void UpdateInit()
    {
        if (roadRollSource) roadRollSource.volume = 0f;      
        if (brakeSource) brakeSource.volume = 0f;
    }

    private void UpdateSound()
    {
        if (mGameSceneManager.SceneState != SceneState.GAME) return;
        UpdateWindSound();
        UpdateRoadSound();
        UpdateBrakeSound();
        UpdateSlipSound();
        UpdateEngineSound();
    }
    private void UpdateRoadSound()
    {
        if (!roadRollSource) return;
        roadRollSource.volume = Mathf.Abs(velocityInput / mSoundParam.RoadRollVolumeRate) * tireVolume * localVolume;
        roadRollSource.pitch = 0.1f + Mathf.Abs(velocityInput / mSoundParam.RoadRollPitchRate);
    }

    private void UpdateBrakeSound()
    {
        if (!brakeSource) return;
        brakeSource.pitch = 1f + 0.5f * Mathf.Abs(velocityInput) / mSoundParam.BrakePitchRate;
        brakeSource.volume = brakeInput * Mathf.Abs(velocityInput) / mSoundParam.BrakeVolumeRate * localVolume;
    }

    private void UpdateSlipSound()
    {
        if (!skidSource) return;
        //速度が0ならスリップしようがない
        if (velocityInput <= 0) return;
        var vector = transform.forward;
        var angleRate = Vector3.Angle(vector, mPreVelocity) / mSoundParam.MaxSlipAngle;
        //スリップ音が最大になる角度より大きければ鳴らさない(バックしたときとか)
        if (angleRate > 1f)
        {
            skidSource.volume = 0f;
            skidSource.pitch = 1f;
            return;
        }   
        angleRate = Mathf.Clamp01(angleRate);
        skidSource.volume = angleRate * mSoundParam.MaxSlipVolume;
        skidSource.pitch = 1f + angleRate * mSoundParam.MaxSlipPitchRAdd;      
        mPreVelocity = mRigidbody.velocity;
    }

    private void UpdateEngineSound()
    {
        //スタート前の空ぶかし
        if(mGameStateManager.CurrentGameState.Value == InGameState.READY)
        {
            if(Input.GetAxisRaw(mAccelString) >= 0.5f && audioSamples.powerSamplesSource[0].pitch <= 2.3)
            {
                audioSamples.powerSamplesSource[0].volume += Input.GetAxisRaw(mAccelString) * Time.deltaTime;
                audioSamples.powerSamplesSource[0].pitch += Input.GetAxisRaw(mAccelString) * Time.deltaTime;
            }
            else if(audioSamples.powerSamplesSource[0].pitch > 1f)
            {
                audioSamples.powerSamplesSource[0].volume -= Time.deltaTime;
                audioSamples.powerSamplesSource[0].pitch -= Time.deltaTime;
            }           
            return;
        }
        rpmSmooth = Mathf.Lerp(rpmSmooth, rpmInput, Time.deltaTime / 0.1f);
        // Engine	
        if (powerSamples != null)
        {
            CalcRamps(
                audioSamples.powerSamplesSource,
                audioSamples.powerBlendRpm,
                0,
                rpmSmooth,
                maxRpm,
                1,
                throttleInput * powerVolume * engineVolume * localVolume);
        }

        if (coastSamples != null)
        {
            CalcRamps(
                audioSamples.coastSamplesSource,
                audioSamples.coastBlendRpm,
                0,
                rpmSmooth,
                maxRpm,
                1,
                (1 - throttleInput) * coastVolume * engineVolume * localVolume);
        }

    }

    private void UpdateWindSound()
    {
        // Wind Rush
        if (windRushSource)
        {
            windRushSource.volume = Mathf.Abs(velocityInput / mSoundParam.WindRushVolumeRate) * localVolume;
            windRushSource.pitch = 1f + Mathf.Abs(velocityInput / mSoundParam.WindRushPitchRate);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        audioHolder.transform.parent = transform;
        audioHolder.transform.position = transform.position;
        ParseCar();
        UpdateInit();
        UpdateSound();

        // Transmission		
        if (gearWhineSamples != null)
        {

            driveshaftRpmSmooth = Mathf.Lerp(driveshaftRpmSmooth, driveshaftRpmInput, Time.deltaTime / 0.1f);
            var transmissionVolume = Mathf.Clamp01(torqueInput / gearWhineTransmissionTorque);
            CalcRamps(
                audioSamples.gearWhineSamplesSource,
                audioSamples.gearWhineBlendRpm,
                0,
                driveshaftRpmSmooth,
                maxRpm,
                1,
                transmissionVolume * gearWhineVolume * localVolume);
        }
    }

    private void CalcRamps(AudioSource[] samples, Vector3[] transfer, float initialRpm, float rpm, float maxRpm, float reactionInput, float volumeInput)
    {
        if (samples.Length <= 0) return;
        for (int i = 0; i < samples.Length; i++)
        {
            var pitchInput = (rpm + initialRpm) / mSoundParam.PitchInputRPM;  //maxRpm;
            var gain = 0f;
            if (i > 0 && i < samples.Length - 1) gain = BlendVolume(rpm, false, false, transfer[i - 1].y, transfer[i].x, transfer[i].y, transfer[i + 1].x);
            else if (i < 1) gain = BlendVolume(rpm, true, false, 0, transfer[i].x, transfer[i].y, transfer[i + 1].x); // start sample					
            else if (i < samples.Length && i > 0) gain = BlendVolume(rpm, false, true, transfer[i - 1].y, transfer[i].x, transfer[i].y, transfer[i].y);   // end sample	
            if (samples[i] == null) continue;

                       samples[i].pitch = Mathf.Abs(pitchInput * reactionInput + (maxRpm - transfer[i].z) / maxRpm);
            samples[i].volume = gain * volumeInput * mSoundParam.EngineVolumeRate;
            samples[i].volume = Mathf.Clamp01(samples[i].volume);
            if (samples[i].pitch < 0) samples[i].pitch = 0;
            if (samples[i].volume <= 0.0f) samples[i].Stop();
            else if (!samples[i].isPlaying) samples[i].Play();
        }
    }

    private float BlendVolume(float currentRPM, bool first, bool last, float prevMaxRPM, float minRPM, float maxRPM, float nextMinRPM)
    {
        var fadeInRPM = 0f;
        var fadeOutRPM = 0f;
        if (first) fadeInRPM = minRPM;
        else fadeInRPM = prevMaxRPM;
        if (last) fadeOutRPM = maxRPM;
        else fadeOutRPM = nextMinRPM;
        var vol = 0f;
        if (currentRPM < fadeInRPM) vol = (currentRPM - minRPM) / (fadeInRPM - minRPM);
        else if (currentRPM > fadeOutRPM) vol = 1.0f - ((currentRPM - fadeOutRPM) / (maxRPM - fadeOutRPM));
        else vol = 1.0f;
        vol = Mathf.Clamp01(vol);
        return (vol);
    }

    public AudioSource CreateAudioSource(AudioClip clip)
    {
        if (clip == null) return null;
        var go = new GameObject(clip.name);
        if (audioHolder) go.transform.parent = audioHolder.transform;
        else go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        var goAudioSource = go.AddComponent<AudioSource>();
        goAudioSource.clip = clip;
        goAudioSource.loop = true;
        goAudioSource.volume = 0;
        goAudioSource.rolloffMode = AudioRolloffMode.Linear;
        goAudioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
        goAudioSource.Play();
        return goAudioSource;
    }

    private static void RemoveAudioSource(ref AudioSource source)
    {
        if (source)
        {
            Destroy(source.gameObject);
            source = null;
        }
    }
}
