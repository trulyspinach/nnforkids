using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UINeural : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerClickHandler {

	public RectTransform rootCanvas;

	private RectTransform m_rectTransform;
	private Vector2 dragOffset;
	private int button_id;

	private bool avoidClick = false;
	private Action<int, PointerEventData> clickCallback;
	
	private void Awake() {
		m_rectTransform = GetComponent<RectTransform>();
	}

	public int GetID() {
		return button_id;
	}
	
	public void Setup(int id, RectTransform c, Vector2 mp, Action<int, PointerEventData> onClick) {
		rootCanvas = c;
//		var pos = new Vector2(mp.x / rootCanvas.localScale.x,
//			mp.y / rootCanvas.localScale.y);
		m_rectTransform.anchoredPosition = mp;
		button_id = id;
		clickCallback = onClick;
	}
	
	public void OnDrag(PointerEventData eventData) {
		
		var pos = new Vector2(eventData.position.x / rootCanvas.localScale.x,
			eventData.position.y / rootCanvas.localScale.y);

		m_rectTransform.anchoredPosition = pos + dragOffset;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		var pos = new Vector2(eventData.position.x / rootCanvas.localScale.x,
			eventData.position.y / rootCanvas.localScale.y);
		
		dragOffset = m_rectTransform.anchoredPosition - pos;
		avoidClick = true;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (avoidClick) {
			avoidClick = false;
			return;
		}
		clickCallback(button_id, eventData);
	}
}
