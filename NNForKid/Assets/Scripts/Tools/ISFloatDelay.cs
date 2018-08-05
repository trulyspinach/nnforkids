using System;
using UnityEngine;

[Serializable]
public class ISFloatDelay {

	public float delay = 0;
	
	public float value {
		get { return m_currentValue; }
		set {
			m_targetValue = value;
		}
	}

	private float m_currentValue;
	private float m_targetValue;
	private bool m_sign;
	
	
	public void SetCurrentValue(float v) {
		m_currentValue = v;
	}

	public void Step(float ticks) {
		m_currentValue += (m_targetValue - m_currentValue) * Mathf.Clamp01(ticks / delay);
	}
}
