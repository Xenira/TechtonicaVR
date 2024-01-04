namespace TechtonicaVR.Input;

public enum InputState
{
	None,
	Default,
	Ui,
}

public class ActiveInputState
{
	private static InputState initialState = InputState.Default;
	public static InputState state = InputState.Default;

	internal static void reset()
	{
		state = initialState;
	}

	public static void setState(InputState newState)
	{
		state = newState;
	}
}
