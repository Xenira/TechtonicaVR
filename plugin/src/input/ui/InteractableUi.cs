using System.Collections.Generic;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public abstract class InteractableUi : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<InteractableUi>();

	private RectTransform rectTransform;
	private Canvas canvas;

	public List<Interactable> interactable = new List<Interactable>();

	protected void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			Logger.LogError($"InteractableUi {gameObject.name} has no canvas");
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
		Logger.LogDebug($"OnEnter {gameObject.name}");
	}
	public void OnExit()
	{
		Logger.LogDebug($"OnExit {gameObject.name}");
	}
	public void OnClick()
	{
		Logger.LogDebug($"OnClick {gameObject.name}");
	}

	private Plane getUiPlane()
	{
		return new Plane(transform.forward, transform.position);
	}
}

public class Interactable
{
	private static PluginLogger Logger = PluginLogger.GetLogger<Interactable>();

	public Rect rect;
	public GameObject gameObject;
	public event InteractableClickEvent OnClick;
	public event InteractableDragEvent OnDrag;
	public event InteractableDropEvent OnDrop;
	public event InteractableCancelDragEvent OnCancelDrag;
	public event InteractableAcceptsDropEvent OnAcceptsDrop;
	public event InteractableReceiveDropEvent OnReceiveDrop;
	public InteractableGetObjectCallback GetObjectCallback;

	public Interactable(Rect rect)
	{
		this.rect = rect;
	}

	// Clickable
	public Interactable(Rect rect, InteractableClickEvent onClick)
	{
		this.rect = rect;
		OnClick += onClick;
	}

	// Draggable
	public Interactable(Rect rect, GameObject gameObject, InteractableGetObjectCallback getObjectCallback, InteractableDragEvent onDrag, InteractableDropEvent onDrop, InteractableCancelDragEvent onCancelDrag)
	{
		this.rect = rect;
		this.gameObject = gameObject;
		OnDrag += onDrag;
		OnDrop += onDrop;
		OnCancelDrag += onCancelDrag;
		GetObjectCallback = getObjectCallback;
	}

	// Clickable and Draggable
	public Interactable(Rect rect, GameObject gameObject, InteractableGetObjectCallback getObjectCallback, InteractableClickEvent onClick, InteractableDragEvent onDrag, InteractableDropEvent onDrop, InteractableCancelDragEvent onCancelDrag)
	{
		this.rect = rect;
		this.gameObject = gameObject;
		OnClick += onClick;
		OnDrag += onDrag;
		OnDrop += onDrop;
		OnCancelDrag += onCancelDrag;
		GetObjectCallback = getObjectCallback;
	}

	// Drop Target
	public Interactable(Rect rect, InteractableAcceptsDropEvent onAcceptsDrop, InteractableReceiveDropEvent onReceiveDrop)
	{
		this.rect = rect;
		OnAcceptsDrop += onAcceptsDrop;
		OnReceiveDrop += onReceiveDrop;
	}

	// Clickable and Drop Target
	public Interactable(Rect rect, InteractableClickEvent onClick, InteractableAcceptsDropEvent onAcceptsDrop, InteractableReceiveDropEvent onReceiveDrop)
	{
		this.rect = rect;
		OnClick += onClick;
		OnAcceptsDrop += onAcceptsDrop;
		OnReceiveDrop += onReceiveDrop;
	}

	// Draggable and Drop Target
	public Interactable(Rect rect, GameObject gameObject, InteractableGetObjectCallback getObjectCallback, InteractableDragEvent onDrag, InteractableDropEvent onDrop, InteractableCancelDragEvent onCancelDrag, InteractableAcceptsDropEvent onAcceptsDrop, InteractableReceiveDropEvent onReceiveDrop)
	{
		this.rect = rect;
		this.gameObject = gameObject;
		OnDrag += onDrag;
		OnDrop += onDrop;
		OnCancelDrag += onCancelDrag;
		OnAcceptsDrop += onAcceptsDrop;
		OnReceiveDrop += onReceiveDrop;
		GetObjectCallback = getObjectCallback;
	}

	// Clickable, Draggable and Drop Target
	public Interactable(Rect rect, GameObject gameObject, InteractableGetObjectCallback getObjectCallback, InteractableClickEvent onClick, InteractableDragEvent onDrag, InteractableDropEvent onDrop, InteractableCancelDragEvent onCancelDrag, InteractableAcceptsDropEvent onAcceptsDrop, InteractableReceiveDropEvent onReceiveDrop)
	{
		this.rect = rect;
		this.gameObject = gameObject;
		OnClick += onClick;
		OnDrag += onDrag;
		OnDrop += onDrop;
		OnCancelDrag += onCancelDrag;
		OnAcceptsDrop += onAcceptsDrop;
		OnReceiveDrop += onReceiveDrop;
		GetObjectCallback = getObjectCallback;
	}

	public bool isClickable()
	{
		return OnClick != null;
	}

	public void click(InteractableUi ui)
	{
		Logger.LogDebug($"click {ui.gameObject.name}");
		OnClick?.Invoke(ui);
	}

	public bool isDraggable()
	{
		return OnDrag != null && GetObjectCallback?.Invoke() != null;
	}

	public void drag(InteractableUi ui)
	{
		Logger.LogDebug($"drag {ui.gameObject.name}");
		OnDrag?.Invoke(ui);
	}

	public bool isDroppable()
	{
		return OnDrop != null;
	}

	public void drop(InteractableUi ui, Interactable source, Interactable target)
	{
		Logger.LogDebug($"drop {ui?.gameObject.name}");
		OnDrop?.Invoke(ui, source, target);
	}

	public void cancelDrag(InteractableUi ui)
	{
		Logger.LogDebug($"cancelDrag {ui.gameObject.name}");
		OnCancelDrag?.Invoke(ui);
	}

	public bool acceptsDrop(InteractableUi ui, Interactable source)
	{
		if (OnAcceptsDrop == null)
		{
			return false;
		}

		var args = new AcceptDropEventArgs { ui = ui, source = source, accept = false };
		OnAcceptsDrop(args);
		return args.accept;
	}

	public void receiveDrop(InteractableUi ui, object source)
	{
		OnReceiveDrop?.Invoke(ui, source);
	}
}

public delegate void InteractableClickEvent(InteractableUi ui);
public delegate void InteractableDragEvent(InteractableUi ui);
public delegate void InteractableDropEvent(InteractableUi ui, Interactable source, Interactable target);
public delegate void InteractableAcceptsDropEvent(AcceptDropEventArgs args);
public delegate void InteractableReceiveDropEvent(InteractableUi ui, object source);
public delegate void InteractableCancelDragEvent(InteractableUi ui);
public delegate object InteractableGetObjectCallback();

public class AcceptDropEventArgs
{
	public InteractableUi ui;
	public Interactable source;
	public bool accept;
}

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
