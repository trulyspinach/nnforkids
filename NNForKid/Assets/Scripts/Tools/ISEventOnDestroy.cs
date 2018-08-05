using System;
using UnityEngine;

public class ISEventOnDestroy : MonoBehaviour {

	Action callback;

	public void Subscribe(Action action)
	{
		callback += action;
	}

	public void FireEvent()
	{
		if (callback.GetInvocationList().Length > 0) callback();
	}

	void OnDestroy()
	{
		FireEvent();
	}
}
