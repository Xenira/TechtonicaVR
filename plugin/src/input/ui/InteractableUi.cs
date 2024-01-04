using System.Collections.Generic;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public abstract class InteractableUi : MonoBehaviour
{
	private RectTransform rectTransform;
	private Canvas canvas;

	public List<Interactable> interactable = new List<Interactable>();

	protected void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			Plugin.Logger.LogError($"InteractableUi {gameObject.name} has no canvas");
			Destroy(this);
			return;
		}
		canvas = GetComponentInParent<Canvas>();

		LaserPointer.interactables.Add(this);
	}

	public UiRaycastHit Raycast(Ray ray)
	{
		if (!canvas.enabled)
		{
			return null;
		}

		if (getUiPlane().Raycast(ray, out var distance))
		{
			var point = ray.GetPoint(distance);
			var localPoint3 = transform.InverseTransformPoint(point);
			var localPoint = new Vector2(localPoint3.x, localPoint3.y);

			var localRect = new Rect(-rectTransform.pivot.x * rectTransform.rect.size.x, -rectTransform.pivot.y * rectTransform.rect.size.y, rectTransform.rect.width, rectTransform.rect.height);
			if (!localRect.Contains(localPoint))
			{
				return null;
			}

			return new UiRaycastHit(this, distance, point, localPoint);
		}

		return null;
	}

	public Interactable getInteractable(Vector2 point)
	{
		foreach (var i in interactable)
		{
			if (i.rect.Contains(point))
			{
				return i;
			}
		}

		return null;
	}

	public void OnEnter()
	{
		Plugin.Logger.LogDebug($"OnEnter {gameObject.name}");
	}
	public void OnExit()
	{
		Plugin.Logger.LogDebug($"OnExit {gameObject.name}");
	}
	public void OnClick()
	{
		Plugin.Logger.LogDebug($"OnClick {gameObject.name}");
	}

	private Plane getUiPlane()
	{
		return new Plane(transform.forward, transform.position);
	}
}

public class Interactable
{
	public Rect rect;
	public event InteractableClickEvent OnClick;
	public event InteractableDragEvent OnDrag;
	public event InteractableDropEvent OnDrop;

	public Interactable(Rect rect)
	{
		this.rect = rect;
	}

	public Interactable(Rect rect, InteractableClickEvent onClick)
	{
		this.rect = rect;
		OnClick += onClick;
	}

	public Interactable(Rect rect, InteractableDragEvent onDrag, InteractableDropEvent onDrop)
	{
		this.rect = rect;
		OnDrag += onDrag;
		OnDrop += onDrop;
	}

	public Interactable(Rect rect, InteractableClickEvent onClick, InteractableDragEvent onDrag, InteractableDropEvent onDrop)
	{
		this.rect = rect;
		OnClick += onClick;
		OnDrag += onDrag;
		OnDrop += onDrop;
	}

	public bool isClickable()
	{
		return OnClick != null;
	}

	public void click(InteractableUi ui)
	{
		OnClick?.Invoke(ui);
	}

	public bool isDraggable()
	{
		return OnDrag != null;
	}

	public void drag(InteractableUi ui)
	{
		OnDrag?.Invoke(ui);
	}

	public bool isDroppable()
	{
		return OnDrop != null;
	}

	public void drop(InteractableUi ui, Interactable target)
	{
		OnDrop?.Invoke(ui, target);
	}
}

public delegate void InteractableClickEvent(InteractableUi ui);
public delegate void InteractableDragEvent(InteractableUi ui);
public delegate void InteractableDropEvent(InteractableUi ui, Interactable target);

public class UiRaycastHit
{
	public InteractableUi ui;
	public float distance;
	public Vector3 point;
	public Vector2 localPoint;

	public UiRaycastHit(InteractableUi ui, float distance, Vector3 point, Vector2 localPoint)
	{
		this.ui = ui;
		this.distance = distance;
		this.point = point;
		this.localPoint = localPoint;
	}
}
