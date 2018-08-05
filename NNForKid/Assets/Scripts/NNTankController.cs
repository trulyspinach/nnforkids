using System.Collections;
using System.Collections.Generic;
using ArtificialTankDriver_by_QI;
using UnityEngine;

public class NNTankController : MonoBehaviour {

	public Tank target;
	public Genome brain;

	public float raderRange;

	private void FixedUpdate() {
		DoSomethingUseful();
	}

	public void CollectionReward() {
//		brain.fitness = (double)target.score / (double)target.shootCount + target.shootCount * 0.1;
		brain.fitness = target.score;
	}
	
	public void DoSomethingUseful() {
		// calculate all input features

		var inputs = new double[20];
		var closestEnemy = target.ClosestEnemy(raderRange);

		//assuming that closest one is always the one it trying to attack.


//		for (int i = 0; i < enemies.Length; i++) {
//			var e = enemies[i].transform;
//			inputs[2*i] = Vector3.Distance(transform.position, e.position) / raderRange;
//			inputs[2 * i + 1] = Vector3.Dot(transform.right, (e.position - transform.position).normalized);
//		}
//		//distance between enemy.
		inputs[0] = closestEnemy != null ? Vector3.Distance(transform.position, closestEnemy.position) / raderRange : 1d;
		//cos to enemy.
		inputs[1] = closestEnemy != null ? Vector3.Dot(transform.right, (closestEnemy.position - transform.position).normalized) : 1d;
		//is weapon ready ?
		inputs[2] = target.weaponReady ? 1d : 0d;
		// current speed.
		inputs[3] = target.rigidbody.velocity.magnitude / target.maxSpeed;
		// current torque.
		inputs[4] = target.rigidbody.angularVelocity.magnitude / target.maxTorque;
////		Debug.Log(inputs[1]);
//		//feedforward
		var output = brain.feedForward(inputs);
//
//		foreach (var d in inputs) {
//			Debug.Log(d);
//		}
		//drive
		target.SetMove((float)output[0]);
		target.SetRotate((float)output[1]);
		if(output[2] > 0) target.Shoot();
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position,raderRange);
	}
}
