
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundParam",fileName = "SoundParam")]
public class SoundParam : ScriptableObject
{
    public float CollisionVolumeRate = 50f;
    public float CollisionPitchAdd = 0f;
    public float CollisionScrapeVolumeRate = 10f;
    public float CollisionScrapePitchRate = 20f;
    public float RoadRollVolumeRate = 200f;
    public float RoadRollPitchRate = 300f;
    public float BrakeVolumeRate = 50f;
    public float BrakePitchRate = 5000f;
    public float WindRushVolumeRate = 200f;
    public float WindRushPitchRate = 300f;
    public float PitchInputRPM = 5000f;
    public float MaxSlipVolume = 1f;
    public float MaxSlipPitchRAdd = 1f;
    public float MaxSlipAngle = 90f;  
    public float JudgeCrashSpeed = 10f;
    public float EngineVolumeRate = 1f;
}