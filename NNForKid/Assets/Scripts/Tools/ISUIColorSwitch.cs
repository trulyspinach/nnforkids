using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ISUIColorSwitch : MonoBehaviour {

	public float speed;
	public Color[] colors;
	public float timeup;
	public Image target;

	private int m_currentTargetColor = 0;

	private void Start() {
		StartCoroutine(Switch());
	}

	private void Update() {
		target.color = Color.Lerp(target.color, colors[m_currentTargetColor], speed * Time.deltaTime);
	}

	private IEnumerator Switch() {
		while (true) {
			m_currentTargetColor = (m_currentTargetColor + 1) % colors.Length;
			yield return new WaitForSeconds(timeup);
		}
	}
}
