using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using ArtificialTankDriver_by_QI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TrainingController : MonoBehaviour {

	public Button genalapButton;
	public Slider speedSlider;
	public Image processIndicator;
	public Demor demor;
	public UINeuralNetworkEditor neuralNetworkEditorBlue;
	public UINeuralNetworkEditor neuralNetworkEditorRed;
	
	public TeamPopulation[] teams;

	private void Start() {
		foreach (var team in teams) {
			team.InitializeBrains();
		}
//		START();
		genalapButton.interactable = false;
		neuralNetworkEditorBlue.Open(teams[0].brains[0], teams[0].brains,() => {
			neuralNetworkEditorRed.Open(teams[1].brains[0], teams[1].brains, () => {
				START();
				EvolveAll();
			});
		});
	}

	public void START() {
		foreach (var team in teams) {
			team.ReadyAll();
		}

		genalapButton.interactable = true;
		started = true;
//		Time.timeScale = 8;
//		StartCoroutine(WaitAndEvolve());
	}

	private bool genALAP = false;
	private int maxStep = 1500;
	private bool started = false;
	private int frameCount = 0;
	private void FixedUpdate() {
		if (!started) return;
		frameCount++;

		processIndicator.fillAmount = (float)frameCount / (float)maxStep;
		
		if (frameCount > maxStep) {
			started = false;
			foreach (var team in teams) {
				team.ResetAll();
			}

			if (genALAP) {
				EvolveAll();
				START();
			}
			else {
				demor.Present(() => {
					neuralNetworkEditorBlue.Open(teams[0].brains[0], teams[0].brains,() => {
						neuralNetworkEditorRed.Open(teams[1].brains[0], teams[1].brains, () => {
							EvolveAll();
							START();
						});

					});
				});
			}

//			EvolveAll();
			frameCount = 0;
		}
		
	}

	public void DoGenALAP() {
		genALAP = true;
		neuralNetworkEditorRed.gameObject.SetActive(false);
		EvolveAll();
		START();
	}
	
	public void SpeedSliderUpdate() {
		Time.timeScale = speedSlider.value * 8;
	}
	
	
	public void EvolveAll() {
		foreach (var team in teams) {
			team.CollectFitness();
			team.群交();
			team.变异();
			team.ResetAll();
		}
	}
	
}

[System.Serializable]
public class TeamPopulation {
	public NNTankController[] tanks;
	public Genome[] brains;

	public void InitializeBrains() {
		brains = new Genome[tanks.Length];
		for (var i = 0; i < tanks.Length; i++) {
			var g = new Genome(5,3);
			
			//g.printGenome();
//			g.addConnection(0,5);
//			g.addConnection(1,5);
//			g.addConnection(2,5);
//			g.addConnection(3,5);
//			g.addConnection(0,6);
//			g.addConnection(1,6);
//			g.addConnection(2,6);
//			g.addConnection(3,6);
//			g.addConnection(0,7);
//			g.addConnection(1,7);
//			g.addConnection(2,7);
//			g.addConnection(3,7);
//			g.moved();
			
//			g.printGenome();
			brains[i] = g;
			tanks[i].brain = g;
		}
	}

	public void SetBrain(Genome[] b) {
		for (int i = 0; i < brains.Length; i++) {
			brains[i] = b[i];
			tanks[i].brain = b[i];
		}
	}

	public void ResetAll() {
		foreach (var tank in tanks) {
			tank.target.RePosAtStart();
			tank.target.Setup();
		}
	}
	
	public void ReadyAll() {
		foreach (var tank in tanks) {
			tank.target.Setup();
		}
	}

	public void CollectFitness() {
		foreach (var tank in tanks) {
			tank.CollectionReward();
		}
	}
	
	public void 群交() {
		brains = Genome.crossoverAll(new List<Genome>(brains)).ToArray();
		SetBrain(brains);
	}

	public void 变异() {
		brains = Genome.mutateWeights(new List<Genome>(brains)).ToArray();
		SetBrain(brains);
	}
}