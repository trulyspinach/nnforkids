using UnityEngine;

[System.Serializable]
public class ISCooldownTimer{

	public float cooldown;

	private float lastTrigger = 0;

	public void SetLastTrigger(float time) {
		lastTrigger = time;
	}
	
	public bool Trigger() {
		if (!(Time.time >= lastTrigger + cooldown)) return false;
		lastTrigger = Time.time;
		return true;
	}
}
