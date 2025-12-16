using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticsController : MonoBehaviour
{
    
    private Gamepad[] _gamepads;

    private void Start()
    {
        _gamepads = Gamepad.all.ToArray();
    }

    public void SetRumble(int gamepad, float value)
    {
        Gamepad.current.SetMotorSpeeds(value * 1000.0f, value * 1000.0f);
        _gamepads[gamepad].SetMotorSpeeds(value * 1000, value * 1000);
    }
    
    [ContextMenu("StopRumble")]
    public void StopAllRumble()
    {
        foreach (var gamepad in _gamepads)
        {
            gamepad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

    private void OnDestroy()
    {
        StopAllRumble();
    }
}
