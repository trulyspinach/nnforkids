using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ISUIOnClickHandler : MonoBehaviour, IPointerClickHandler {

	private Action cb;

	public void Setup(Action callback) {
		cb = callback;
	}
	
	public void OnPointerClick(PointerEventData eventData) {
		cb();
	}
}
