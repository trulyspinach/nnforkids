using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Demor : MonoBehaviour {

	public Animator animator;
	public Text[] connections;
	public float nextTimeup;

	public void Present(Action callback) {
		gameObject.SetActive(true);
		Time.timeScale = 1;
		StartCoroutine(Seq(callback));
	}

	IEnumerator Seq(Action back) {
		gameObject.SetActive(true);
		
		foreach (var connection in connections) {
			connection.text = Random.Range(-1f, 1f).ToString("F");
		}
		yield return new WaitForSeconds(3);
		
		animator.Play("crossover");
		yield return new WaitForSeconds(4);

		foreach (var connection in connections) {
			if (Random.Range(0, 100) > 50) {
				connection.text = Random.Range(-1f, 1f).ToString("F");
				yield return new WaitForSeconds(0.5f);
			}
		}

		yield return new WaitForSeconds(5);
		
		gameObject.SetActive(false);
		back();
	}
}
