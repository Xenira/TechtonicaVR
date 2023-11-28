using System;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using Valve.VR;

namespace TechtonicaVR.Input;

public class Button
{
    bool currentState;
    bool previousState;
    float lastChangeTime = UnityEngine.Time.time;
    float lastDuration = 0f;

    public Button(SteamVR_Action_Boolean action)
    {
        action.AddOnUpdateListener(HandleUpdate, SteamVR_Input_Sources.Any);
        lastChangeTime = UnityEngine.Time.time;
        lastDuration = 0f;
    }

    private void HandleUpdate(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        previousState = currentState;
        currentState = newState;
        if (currentState != previousState)
        {
            lastDuration = UnityEngine.Time.time - lastChangeTime;
            lastChangeTime = UnityEngine.Time.time;
        }
    }

    public bool IsDown()
    {
        return currentState;
    }

    public bool IsUp()
    {
        return !currentState;
    }

    public bool IsPressed()
    {
        return currentState && !previousState;
    }

    public bool IsReleased()
    {
        return !currentState && previousState;
    }

    public bool IsTimedPress(float min)
    {
        return currentState && lastChangeTime + min >= UnityEngine.Time.time;
    }

    public bool IsTimedPressUp(float min)
    {
        return IsReleased() && lastDuration >= min;
    }

    public bool IsTimedPressUp(float min, float max)
    {
        return IsReleased() && lastDuration >= min && lastDuration <= max;
    }

    public bool IsTimedPressDown(float min)
    {
        return currentState && lastChangeTime + min >= UnityEngine.Time.time;
    }

    public bool IsTimedPressDown(float min, float max)
    {
        if (!currentState)
        {
            return false;
        }

        var timePressed = UnityEngine.Time.time - lastChangeTime;
        return timePressed >= min && timePressed <= max;
    }
}
