using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UINeuralNetworkEditor : MonoBehaviour {

	public RectTransform root;
	public RectTransform neuralRoot;

	public RectTransform inputCol;
	public RectTransform outputCol;
	public float nerualSpacing;

	public Text movesLabel;
	public int moves = 3;
	
	public RectTransform connectionsRoot;
	public GameObject backgroundObj;

	
	public GameObject neuralPrefab;
	public GameObject neuralConnectionPrefab;
	
	public List<UINeural> neuralsObjects = new List<UINeural>();
	public List<GameObject> connectionObjects = new List<GameObject>();

	public Genome data;
	public Genome[] toOperate;

	private Action doneCallback;
	
	public void Open(Genome genome, Genome[] genomes, Action done) {
		gameObject.SetActive(true);
		SetData(genome);
		toOperate = genomes;
		doneCallback = done;
		moves = 3;
		movesLabel.text = $"Available Moves: {moves}";
	}

	static public int a = 0;
	public void Close() {
		gameObject.SetActive(false);
		a = 1;
//		foreach (var neuralsObject in neuralsObjects) {
//			NodeByID(neuralsObject.GetID()).x = neuralsObject.GetComponent<RectTransform>().anchoredPosition.x;
//			NodeByID(neuralsObject.GetID()).y = neuralsObject.GetComponent<RectTransform>().anchoredPosition.y;
//			
//
//		}
		
		doneCallback();
	}
	
	public void SetData(Genome newData) {
		ClearGraph();
		data = newData;

		for (int i = 0; i < data.inputNum; i++) {
			var pos = inputCol.anchoredPosition + Vector2.down * nerualSpacing * i;
			NewNeuralAt(pos, i);
		}
		
		for(int i = 0; i < data.outputNum; i++) {
			var pos = outputCol.anchoredPosition + Vector2.down * nerualSpacing * i;
			NewNeuralAt(pos, i + data.inputNum);
		}

		for (int i = data.inputNum + data.outputNum; i < data.nodes.Count; i++) {
			var node = data.nodes[i];
			NewNeuralAt(new Vector2((float)node.x, (float)node.y), node.number);
		}
		UpdateConnectionGraph();
	}

	public Node NodeByID(int id) {
		foreach (var node in data.nodes) {
			if (node.number == id) return node;
		}

		return null;
	}

	public UINeural NodeGraphicByID(int id) {
		foreach (var node in neuralsObjects) {
			if (node.GetID() == id) return node;
		}

		return null;
	}
	
	public void ClearGraph() {
		neuralsObjects.ForEach(Destroy);
		connectionObjects.ForEach(Destroy);
		
		neuralsObjects.Clear();
		connectionObjects.Clear();
	}
	
	public void NewNeuralAt(Vector3 pos, int id) {
		var obj = Instantiate(neuralPrefab, neuralRoot);
		var n = obj.GetComponent<UINeural>();
		
		n.Setup(id, root, pos, (bid, e) => {
			CheckConnect(NodeGraphicByID(bid));
		});
		
		neuralsObjects.Add(n);
	}

	private UINeural connectingFrom = null;
	
	private void CheckConnect(UINeural n) {
		if (connectingFrom) {
			var from = NodeByID(connectingFrom.GetID());
			var to = NodeByID(n.GetID());
//			data.addConnection(from, to);
//			data.printGenome();
			foreach (var genome in toOperate) {
				genome.addConnection(from,to);
			}
			
			UpdateConnectionGraph();
			connectingFrom = null;

			moves--;
			movesLabel.text = $"Available Moves: {moves}";
		}
		else {
			connectingFrom = n;
		}
	}
	
	private void UpdateConnectionGraph() {
		connectionObjects.ForEach(Destroy);
		connectionObjects.Clear();

		foreach (var connection in data.genes) {
			var obj = Instantiate(neuralConnectionPrefab, connectionsRoot);
			obj.GetComponent<UILineConnector>().transforms = new[] {
				NodeGraphicByID(connection.fromNode.number).GetComponent<RectTransform>(),
				NodeGraphicByID(connection.toNode.number).GetComponent<RectTransform>()
			};
		}
	}
	
	private void Awake() {
		backgroundObj.AddComponent<ISUIOnClickHandler>().Setup(() => {
			var node = data.addNode(1);
			
			var pos = new Vector2(Input.mousePosition.x / neuralRoot.localScale.x,
				Input.mousePosition.y / neuralRoot.localScale.y);
			
			NewNeuralAt(pos + neuralRoot.anchoredPosition, node.number);

			for (int i = 1; i < toOperate.Length; i++) {
				toOperate[i].addNode(1);
			}
			
		});
	}
}