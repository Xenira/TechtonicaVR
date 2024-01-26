using System;
using System.Collections.Generic;
using System.Linq;
using TechtonicaVR.UI;
using TechtonicaVR.Util;
using UnityEngine;
using UnityEngine.UI;

namespace TechtonicaVR.Input.Ui;

public abstract class InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<InteractableUi>();
	public List<Interactable> interactable = new List<Interactable>();
	public UiEnterEvent OnEnterEvent;
	public UiExitEvent OnExitEvent;
	public Menu menu;
	public Func<List<Interactable>> getInteractables;

	public Transform transform;
	public float zIndex = 0;
	protected ScrollRect scrollRect;
	protected RectTransform rectTransform;
	private Canvas canvas;

	public InteractableUi(GameObject gameObject)
	{
		this.transform = gameObject.transform;
		this.rectTransform = gameObject.GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			Logger.LogError($"InteractableUi {gameObject.name} has no canvas");
			return;
		}
		canvas = gameObject.GetComponentInParent<Canvas>();

		LaserPointer.interactables.Add(this);
	}

	public UiRaycastHit Raycast(Ray ray)
	{
		if (!canvas.enabled)
		{
			return null;
		}

		if (menu?.isOpen() == false)
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

			if (scrollRect != null)
			{
				if (scrollRect.verticalScrollbar)
				{
					if (localPoint.x < localRect.x + 12)
					{
						scrollRect.verticalScrollbar.value += scrollRect.verticalScrollbar.stepSize;
					}
					else if (localPoint.x > localRect.width - 12)
					{
						scrollRect.verticalScrollbar.value -= scrollRect.verticalScrollbar.stepSize;
					}
				}
				if (scrollRect.horizontalScrollbar)
				{
					if (localPoint.y < localRect.y + 12)
					{
						scrollRect.horizontalScrollbar.value -= scrollRect.horizontalScrollbar.stepSize;
					}
					else if (localPoint.y > localRect.height - 12)
					{
						scrollRect.horizontalScrollbar.value += scrollRect.horizontalScrollbar.stepSize;
					}
				}
			}

			var scrollOffset = scrollRect?.m_PrevPosition ?? Vector2.zero;
			return new UiRaycastHit(this, distance + zIndex, point, localPoint + scrollOffset);
		}

		return null;
	}

	public Interactable getInteractable(Vector2 point)
	{
		// Logger.LogDebug($"getInteractable {transform.gameObject.name} {point}");
		return interactable.Where(i => i.isHit(point) && i.gameObject.activeInHierarchy).FirstOrDefault();
	}

	public bool rebuildInteractables()
	{
		if (getInteractables == null)
		{
			return false;
		}

		interactable = getInteractables();
		return true;
	}

	public void recalculateInteractablePositions()
	{
		interactable.ForEach(i => i.recalculate());
	}

	public void OnEnter()
	{
		Logger.LogDebug($"OnEnter {transform.gameObject.name}");
		AsyncGameObject.Instance.timeoutFrames(() =>
		{
			if (!rebuildInteractables())
			{
				recalculateInteractablePositions();
			}
		}, 1);
		OnEnterEvent?.Invoke();
	}
	public void OnExit()
	{
		Logger.LogDebug($"OnExit {transform.gameObject.name}");
		OnExitEvent?.Invoke();
	}
	public void OnClick()
	{
		Logger.LogDebug($"OnClick {transform.gameObject.name}");
	}

	private Plane getUiPlane()
	{
		return new Plane(transform.forward, transform.position);
	}

	protected Rect getRect(RectTransform rectTransform)
	{
		var rect = new Rect(rectTransform.rect);

		var relativeLocalPosition = ObjectPosition.addLocalPositions(rectTransform, this.rectTransform);

		rect.x += relativeLocalPosition.x;
		rect.y += relativeLocalPosition.y;
		Logger.LogDebug($"getRect {rectTransform.gameObject.name} {rect} {relativeLocalPosition}");
		return rect;
	}
}

public delegate void UiEnterEvent();
public delegate void UiExitEvent();

public class InteractableBuilder
{
	private InteractableUi ui;
	private Rect rect;
	private GameObject gameObject;
	private InteractableClickEvent onClickEvent;
	private InteractableDragEvent onDragEvent;
	private InteractableDropEvent onDropEvent;
	private InteractableCancelDragEvent onCancelDragEvent;
	private InteractableAcceptsDropEvent onAcceptsDropEvent;
	private InteractableReceiveDropEvent onReceiveDropEvent;
	private InteractableHoverEnterEvent onHoverEnterEvent;
	private InteractableHoverExitEvent onHoverExitEvent;

	private InteractableIsHitCallback isHitCallback;
	private InteractableGetObjectCallback getObjectCallback;
	private Func<Rect> recalculateCallback;

	public InteractableBuilder(InteractableUi ui, Rect rect, GameObject gameObject)
	{
		this.ui = ui;
		this.rect = rect;
		this.gameObject = gameObject;
	}

	public Interactable build()
	{
		var interactable = new Interactable(ui, rect, gameObject, isHitCallback, getObjectCallback, onClickEvent, onDragEvent, onDropEvent, onCancelDragEvent, onAcceptsDropEvent, onReceiveDropEvent, onHoverEnterEvent, onHoverExitEvent);

		if (recalculateCallback != null)
		{
			interactable.recalculateCallback = recalculateCallback;
		}

		return interactable;
	}

	public InteractableBuilder withRecalculate(Func<Rect> recalculateCallback)
	{
		this.recalculateCallback = recalculateCallback;
		return this;
	}

	public InteractableBuilder withIsHit(InteractableIsHitCallback isHitCallback)
	{
		this.isHitCallback = isHitCallback;
		return this;
	}

	public InteractableBuilder withClick(InteractableClickEvent onClick)
	{
		onClickEvent = onClick;
		return this;
	}

	public InteractableBuilder withDrag(InteractableGetObjectCallback getObjectCallback, InteractableDragEvent onDrag, InteractableDropEvent onDrop, InteractableCancelDragEvent onCancelDrag)
	{
		onDragEvent = onDrag;
		onDropEvent = onDrop;
		onCancelDragEvent = onCancelDrag;
		this.getObjectCallback = getObjectCallback;
		return this;
	}

	public InteractableBuilder withDrop(InteractableAcceptsDropEvent onAcceptsDrop, InteractableReceiveDropEvent onReceiveDrop)
	{
		onAcceptsDropEvent = onAcceptsDrop;
		onReceiveDropEvent = onReceiveDrop;
		return this;
	}

	public InteractableBuilder withHoverEnter(InteractableHoverEnterEvent onHoverEnter)
	{
		onHoverEnterEvent = onHoverEnter;
		return this;
	}

	public InteractableBuilder withHoverExit(InteractableHoverExitEvent onHoverExit)
	{
		onHoverExitEvent = onHoverExit;
		return this;
	}
}

public class Interactable
{
	private static PluginLogger Logger = PluginLogger.GetLogger<Interactable>();

	public InteractableUi ui;
	public Rect rect;
	public GameObject gameObject;
	public event InteractableClickEvent OnClick;
	public event InteractableDragEvent OnDrag;
	public event InteractableDropEvent OnDrop;
	public event InteractableCancelDragEvent OnCancelDrag;
	public event InteractableAcceptsDropEvent OnAcceptsDrop;
	public event InteractableReceiveDropEvent OnReceiveDrop;
	public event InteractableHoverEnterEvent OnHoverEnter;
	public event InteractableHoverExitEvent OnHoverExit;

	public InteractableIsHitCallback IsHitCallback;
	public InteractableGetObjectCallback GetObjectCallback;
	public Func<Rect> recalculateCallback;

	// Clickable, Draggable and Drop Target
	internal Interactable(InteractableUi ui, Rect rect, GameObject gameObject, InteractableIsHitCallback isHitCallback, InteractableGetObjectCallback getObjectCallback, InteractableClickEvent onClick, InteractableDragEvent onDrag, InteractableDropEvent onDrop, InteractableCancelDragEvent onCancelDrag, InteractableAcceptsDropEvent onAcceptsDrop, InteractableReceiveDropEvent onReceiveDrop, InteractableHoverEnterEvent onHoverEnter, InteractableHoverExitEvent onHoverExit)
	{
		this.ui = ui;
		this.rect = rect;
		this.gameObject = gameObject;
		OnClick += onClick;
		OnDrag += onDrag;
		OnDrop += onDrop;
		OnCancelDrag += onCancelDrag;
		OnAcceptsDrop += onAcceptsDrop;
		OnReceiveDrop += onReceiveDrop;
		OnHoverEnter += onHoverEnter;
		OnHoverExit += onHoverExit;

		IsHitCallback = isHitCallback;
		GetObjectCallback = getObjectCallback;
	}

	public void recalculate()
	{
		if (recalculateCallback != null)
		{
			rect = recalculateCallback();
		}
	}

	public bool isHit(Vector2 point)
	{
		if (IsHitCallback != null)
		{
			return IsHitCallback(point);
		}

		return rect.Contains(point);
	}

	public bool isClickable()
	{
		return OnClick != null;
	}

	public void hoverEnter(InteractableUi ui)
	{
		Logger.LogDebug($"hoverEnter {ui?.transform.gameObject.name}");
		OnHoverEnter?.Invoke(ui);
	}

	public void hoverExit(InteractableUi ui)
	{
		Logger.LogDebug($"hoverExit {ui?.transform.gameObject.name}");
		OnHoverExit?.Invoke(ui);
	}

	public void click(InteractableUi ui)
	{
		Logger.LogDebug($"click {ui.transform.gameObject.name}");
		OnClick?.Invoke(ui);
	}

	public bool isDraggable()
	{
		return OnDrag != null && GetObjectCallback?.Invoke() != null;
	}

	public void drag(InteractableUi ui)
	{
		Logger.LogDebug($"drag {ui.transform.gameObject.name}");
		OnDrag?.Invoke(ui);
	}

	public bool isDroppable()
	{
		return OnDrop != null;
	}

	public void drop(InteractableUi ui, Interactable source, Interactable target)
	{
		Logger.LogDebug($"drop {ui?.transform.gameObject.name}");
		OnDrop?.Invoke(ui, source, target);
	}

	public void cancelDrag(InteractableUi ui)
	{
		Logger.LogDebug($"cancelDrag {ui.transform.gameObject.name}");
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
public delegate void InteractableHoverEnterEvent(InteractableUi ui);
public delegate void InteractableHoverExitEvent(InteractableUi ui);

public delegate bool InteractableIsHitCallback(Vector2 point);
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
