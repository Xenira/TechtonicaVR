using UnityEngine;
using Valve.VR;
using System.Collections.Generic;
using TechtonicaVR.Input.Ui;
using System.Linq;
using TechtonicaVR.Util;

namespace TechtonicaVR.Input;
public class LaserPointer : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<LaserPointer>();

	private static List<LaserPointer> pointers = new List<LaserPointer>();
	public static List<InteractableUi> interactables = new List<InteractableUi>();
	public static event PointerEventHandler PointerIn;
	public static event PointerEventHandler PointerOut;
	public static event PointerEventHandler PointerClick;
	public static event PointerEventHandler PointerGrab;
	public static event PointerEventHandler PointerDrop;
	public static event PointerEventHandler PointerCancelDrag;

	public bool active = true;
	public bool laserUiOnly = ModConfig.laserUiOnly.Value;
	public Color color = ModConfig.laserColor.Value;
	public Color clickColor = ModConfig.laserClickColor.Value;
	public Color validColor = ModConfig.laserValidColor.Value;
	public Color invalidColor = ModConfig.laserInvalidColor.Value;
	public float thickness = ModConfig.laserThickness.Value;
	public float maxDistance = 100.0f;
	public Vector3 direction = Vector3.forward;
	public GameObject holder;
	public GameObject pointer;
	bool isActive = false;
	public Transform reference;
	public SteamVR_Input_Sources inputSource;
	public Button interactButton;
	public GameObject dragCanvasObject;
	Interactable draggedInteractable;
	InteractableUi previousContact = null;
	Interactable previousInteractable = null;

	private void Start()
	{
		SteamVR_Actions.default_Interact.actionSet.Activate();
		holder = new GameObject();
		holder.transform.parent = this.transform;
		holder.transform.localPosition = Vector3.zero;
		holder.transform.localRotation = Quaternion.LookRotation(direction);

		pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pointer.transform.parent = holder.transform;
		pointer.transform.localScale = new Vector3(thickness, thickness, maxDistance);
		pointer.transform.localPosition = new Vector3(0f, 0f, maxDistance / 2f);
		pointer.transform.localRotation = Quaternion.identity;
		BoxCollider collider = pointer.GetComponent<BoxCollider>();
		if (collider)
		{
			Object.Destroy(collider);
		}

		Material newMaterial = new Material(Shader.Find("Unlit/Color"));
		newMaterial.color = color;
		pointer.GetComponent<MeshRenderer>().material = newMaterial;
		pointer.GetComponent<MeshRenderer>().sortingOrder = 200;

		dragCanvasObject = new GameObject("DragCanvas");
		dragCanvasObject.transform.parent = holder.transform;
		dragCanvasObject.transform.localPosition = (Vector3.forward + Vector3.up) * 0.1f;
		var canvas = dragCanvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.sortingOrder = 100;

		interactButton.ButtonReleased += new ButtonEventHandler(DoClickUp);

		pointers.Add(this);
	}

	private void DoClickUp(object sender, SteamVR_Input_Sources source)
	{
		if (!active && source == inputSource)
		{
			pointers.ForEach(p => p.deactivate());
			active = true;
		}
	}

	public virtual void OnPointerIn(PointerEventArgs e)
	{
		if (PointerIn != null)
			PointerIn(this, e);
	}

	public virtual void OnPointerClick(PointerEventArgs e)
	{
		Logger.LogDebug($"OnPointerClick: {e.target.transform.name}");
		e.interactable?.click(e.target);
		PointerClick?.Invoke(this, e);
	}

	public virtual void OnPointerOut(PointerEventArgs e)
	{
		if (PointerOut != null)
			PointerOut(this, e);
	}

	public virtual void OnGrab(PointerEventArgs e)
	{
		Logger.LogDebug($"OnGrab: {e.target.transform.name}");
		var dragIcon = Instantiate(e.interactable.gameObject).GetComponent<RectTransform>();
		dragIcon.SetParent(dragCanvasObject.transform, false);
		dragIcon.localPosition = Vector3.zero;
		dragIcon.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		dragIcon.localRotation = Quaternion.Euler(90f, 0f, 0f);
		draggedInteractable = e.interactable;
		draggedInteractable.drag(e.target);

		if (PointerGrab != null)
			PointerGrab(this, e);
	}

	public virtual void OnDrop(PointerEventArgs e)
	{
		Logger.LogDebug($"OnDrop: {e.target?.transform.name}");
		dragCanvasObject.transform.DestroyAllChildren();
		draggedInteractable.drop(e.target, draggedInteractable, e.interactable);
		draggedInteractable = null;

		if (PointerDrop != null)
			PointerDrop(this, e);
	}

	public virtual void OnCancelDrag(PointerEventArgs e)
	{
		Logger.LogDebug($"OnCancelDrag: {e.target?.transform.name}");
		dragCanvasObject.transform.DestroyAllChildren();
		draggedInteractable.cancelDrag(e.target);
		draggedInteractable = null;

		if (PointerCancelDrag != null)
			PointerCancelDrag(this, e);
	}

	private void LateUpdate()
	{
		if (!isActive)
		{
			isActive = true;
			holder.SetActive(true);
		}

		float dist = maxDistance;

		Ray raycast = new Ray(transform.position, holder.transform.forward);
		var hit = interactables.Select(i => i.Raycast(raycast)).Where(i => i != null).OrderBy(i => i.distance).FirstOrDefault();
		var bHit = hit != null;
		var interactable = hit?.ui.getInteractable(hit.localPoint) ?? null;

		if (previousContact != null && previousContact != hit?.ui)
		{
			PointerEventArgs args = new PointerEventArgs
			{
				distance = 0f,
				flags = 0,
				target = previousContact,
				interactable = interactable
			};
			OnPointerOut(args);
			previousContact = null;
		}
		if (bHit && previousContact != hit?.ui)
		{
			PointerEventArgs argsIn = new PointerEventArgs
			{
				distance = hit.distance,
				flags = 0,
				target = hit.ui,
				interactable = interactable
			};
			OnPointerIn(argsIn);
			previousContact = hit.ui;
		}

		if (previousInteractable != null && previousInteractable != interactable)
		{
			previousInteractable.hoverExit(hit?.ui);
			previousInteractable = null;
		}
		if (interactable != null && previousInteractable != interactable)
		{
			interactable.hoverEnter(hit?.ui);
			previousInteractable = interactable;
		}

		if (!bHit)
		{
			previousContact = null;
		}
		if (bHit)
		{
			dist = hit.distance;
		}

		if (bHit || draggedInteractable != null)
		{
			ActiveInputState.setState(InputState.Ui);
			holder.SetActive(true);
		}
		else
		{
			ActiveInputState.reset();
			if (laserUiOnly)
			{
				holder.SetActive(false);
				return;
			}
		}

		handleInteraction(hit, interactable);

		updateLaserPointer(hit?.ui, interactable, dist);
	}

	public void deactivate()
	{
		active = false;
		isActive = false;
	}

	private void handleInteraction(UiRaycastHit hit, Interactable interactable)
	{
		if (draggedInteractable != null && interactButton.IsUp())
		{
			PointerEventArgs argsDrop = new PointerEventArgs
			{
				distance = hit?.distance ?? maxDistance,
				flags = 0,
				target = hit?.ui,
				interactable = interactable
			};

			if (hit?.ui == null || (interactable?.acceptsDrop(hit.ui, draggedInteractable) ?? false))
			{
				OnDrop(argsDrop);
			}
			else
			{
				OnCancelDrag(argsDrop);
			}
			return;
		}
		else if (draggedInteractable != null)
		{
			return;
		}

		if (interactable == null)
		{
			return;
		}

		var isDraggable = interactable.isDraggable();

		if (isDraggable ? interactButton.IsTimedPressUp(0, ModConfig.clickTime.Value) : interactButton.IsReleased())
		{
			PointerEventArgs argsClick = new PointerEventArgs
			{
				distance = hit.distance,
				flags = 0,
				target = hit.ui,
				interactable = interactable
			};
			OnPointerClick(argsClick);
			return;
		}

		if (isDraggable && interactButton.IsTimedPressDown(ModConfig.clickTime.Value))
		{
			PointerEventArgs argsGrab = new PointerEventArgs
			{
				distance = hit.distance,
				flags = 0,
				target = hit.ui,
				interactable = interactable
			};
			OnGrab(argsGrab);
			return;
		}
	}

	private void updateLaserPointer(InteractableUi ui, Interactable interactable, float dist)
	{
		if (interactButton.IsDown())
		{
			var thickness = this.thickness * ModConfig.laserClickThicknessMultiplier.Value;
			pointer.transform.localScale = new Vector3(thickness, thickness, dist);
			pointer.GetComponent<MeshRenderer>().material.color = clickColor;
		}
		else
		{
			pointer.transform.localScale = new Vector3(thickness, thickness, dist);
		}

		pointer.GetComponent<MeshRenderer>().material.color = getLaserColor(ui, interactable);
		pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
	}

	private Color getLaserColor(InteractableUi ui, Interactable interactable)
	{
		if (draggedInteractable != null)
		{
			return interactable?.acceptsDrop(ui, draggedInteractable) ?? false ? validColor : invalidColor;
		}

		if (interactable == null)
		{
			return color;
		}

		if (interactable.isClickable() || interactable.isDraggable())
		{
			return validColor;
		}

		return invalidColor;
	}
}

public struct PointerEventArgs
{
	public uint flags;
	public float distance;
	public InteractableUi target;
	public Interactable interactable;
}

public delegate void PointerEventHandler(object sender, PointerEventArgs e);
