namespace TechtonicaVR.UI;

public class UIMenuWrapper : Menu
{

	private UIMenu menu;

	public UIMenuWrapper(UIMenu menu)
	{
		this.menu = menu;
	}

	public bool isOpen()
	{
		return menu.isOpen && !UIManager.instance.recipePickerUI.myCanvas.enabled;
	}
}
