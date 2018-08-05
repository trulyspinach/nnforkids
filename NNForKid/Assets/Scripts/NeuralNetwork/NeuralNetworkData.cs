using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkData : object {

	public List<List<Node>> layers = new List<List<Node>>();
	public List<ConnectionGene> connections =  new List<ConnectionGene>();

	private Genome genome;
	
	public NeuralNetworkData(Genome genome) {
		this.genome = genome;
		layers = this.genome.returnNetwork();
		connections = this.genome.genes;
	}


}
