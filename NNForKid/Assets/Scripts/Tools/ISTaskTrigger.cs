using System;

public class ISTaskTrigger {

	private readonly Action m_action;
	private readonly Func<bool> m_predict;
	private bool m_triggered;

	public ISTaskTrigger(Action action, Func<bool> predict) {
		m_action = action;
		m_predict = predict;
	}
	
	public void Tick() {
		if (m_triggered && !m_predict()) m_triggered = false;
		if (m_triggered || !m_predict()) return;
		m_action();
		m_triggered = true;
	}
}
