using PiUtils.UI;

namespace TechtonicaVR.UI;

public class RecipePickerMenuWrapper : Menu
{

	private RecipePickerUI menu;

	public RecipePickerMenuWrapper(RecipePickerUI menu)
	{
		this.menu = menu;
	}

	public bool isOpen()
	{
		return menu.myCanvas.enabled;
	}
}
