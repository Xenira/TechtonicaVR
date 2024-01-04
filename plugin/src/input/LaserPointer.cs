using UnityEngine;
using Valve.VR;
using System.Collections.Generic;
using TechtonicaVR.Input.Ui;
using System.Linq;
using TechtonicaVR.Util;

namespace TechtonicaVR.Input;
public class LaserPointer : MonoBehaviour
{
	private static Logger<LaserPointer> Logger = new Logger<LaserPointer>();
	private static List<LaserPointer> pointers = new List<LaserPointer>();
	public static List<InteractableUi> interactables = new List<InteractableUi>();
	public static event PointerEventHandler PointerIn;
	public static event PointerEventHandler PointerOut;
	public static event PointerEventHandler PointerClick;

	public bool active = true;
	public Color color = ModConfig.laserColor.Value;
	public Color clickColor = ModConfig.laserClickColor.Value;
	public Color inactiveColor = ModConfig.laserInactiveColor.Value;
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
	public Button grabButton;

	InteractableUi previousContact = null;

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
		pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
		pointer.transform.localRotation = Quaternion.identity;
		BoxCollider collider = pointer.GetComponent<BoxCollider>();
		if (collider)
		{
			Object.Destroy(collider);
		}

		Material newMaterial = new Material(Shader.Find("Unlit/Color"));
		newMaterial.SetColor("_Color", color);
		pointer.GetComponent<MeshRenderer>().material = newMaterial;

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
		Logger.LogDebug($"OnPointerClick: {e.target.name}");
		e.interactable?.click(e.target);
		PointerClick?.Invoke(this, e);
	}

	public virtual void OnPointerOut(PointerEventArgs e)
	{
		if (PointerOut != null)
			PointerOut(this, e);
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

		if (previousContact && previousContact != hit?.ui)
		{
			PointerEventArgs args = new PointerEventArgs
			{
				distance = 0f,
				flags = 0,
				target = previousContact,
				interactable = interactable
			};
			ActiveInputState.reset();
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
			ActiveInputState.setState(InputState.Ui);
			OnPointerIn(argsIn);
			previousContact = hit.ui;
		}
		if (!bHit)
		{
			previousContact = null;
		}
		if (bHit)
		{
			dist = hit.distance;
		}

		if (interactable != null && interactButton.IsReleased())
		{
			PointerEventArgs argsClick = new PointerEventArgs
			{
				distance = hit.distance,
				flags = 0,
				target = hit.ui,
				interactable = interactable
			};
			OnPointerClick(argsClick);
		}

		if (interactButton.IsDown())
		{
			var thickness = this.thickness * ModConfig.laserClickThicknessMultiplier.Value;
			pointer.transform.localScale = new Vector3(thickness, thickness, dist);
			pointer.GetComponent<MeshRenderer>().material.color = clickColor;
		}
		else
		{
			pointer.transform.localScale = new Vector3(thickness, thickness, dist);
			if (interactable != null)
			{
				pointer.GetComponent<MeshRenderer>().material.color = interactable.isClickable() || interactable.isDraggable() ? validColor : invalidColor;
			}
			else
			{
				pointer.GetComponent<MeshRenderer>().material.color = bHit ? color : inactiveColor;
			}
		}

		pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
	}

	public void deactivate()
	{
		active = false;
		isActive = false;
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
