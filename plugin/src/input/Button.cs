using Valve.VR;

namespace TechtonicaVR.Input;

public class Button
{
	public SteamVR_Action_Boolean action;
	bool currentState;
	bool previousState;
	float lastChangeTime;
	float lastDuration;
	private InputState state;

	public event ButtonEventHandler ButtonPressed;
	public event ButtonEventHandler ButtonReleased;

	public Button(SteamVR_Action_Boolean action, InputState state = InputState.None)
	{
		this.action = action;
		this.state = state;

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
			if (currentState)
			{
				ButtonPressed?.Invoke(this, fromSource);
			}
			else
			{
				ButtonReleased?.Invoke(this, fromSource);
			}

			lastDuration = UnityEngine.Time.time - lastChangeTime;
			lastChangeTime = UnityEngine.Time.time;
		}
	}

	public bool IsDown()
	{
		if (state != InputState.None && state != ActiveInputState.state)
		{
			return false;
		}

		return currentState;
	}

	public bool IsUp()
	{
		return !IsDown();
	}

	public bool IsPressed()
	{
		return IsDown() && !previousState;
	}

	public bool IsReleased()
	{
		if (state != InputState.None && state != ActiveInputState.state)
		{
			return false;
		}

		return IsUp() && previousState;
	}

	public bool IsTimedPress(float min)
	{
		return IsDown() && UnityEngine.Time.time - lastChangeTime >= min;
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
		return IsDown() && UnityEngine.Time.time - lastChangeTime >= min;
	}

	public bool IsTimedPressDown(float min, float max)
	{
		if (IsUp() || state != InputState.None && state != ActiveInputState.state)
		{
			return false;
		}

		var timePressed = UnityEngine.Time.time - lastChangeTime;
		return timePressed >= min && timePressed <= max;
	}
}

public delegate void ButtonEventHandler(object sender, SteamVR_Input_Sources source);
