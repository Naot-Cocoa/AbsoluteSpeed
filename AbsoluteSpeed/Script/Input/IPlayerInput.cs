using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IPlayerInput 
{
    IReadOnlyReactiveProperty<float> Accel { get;}
    IReadOnlyReactiveProperty<float> Brake { get;}
    IReadOnlyReactiveProperty<float> Hundle { get;}
    IReadOnlyReactiveProperty<GearState> GetCurrentGear { get;}
}
