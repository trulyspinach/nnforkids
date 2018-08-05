using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Genome {
    public List<ConnectionGene> genes = new List<ConnectionGene>();//a list of connections between nodes which represent the NN
    public List<Node> nodes = new List<Node>();//list of nodes
    public int inputNum;
    public int outputNum;
    public int layers = 3;
    private int nextNode = 0;
    public double fitness = 0;
    List<Node> network = new List<Node>();//a list of the nodes in the order that they need to be considered in the NN
    
    public Genome(int a, int b) {
        //set input number and output number
        inputNum = a;
        outputNum = b;
        //create input nodes
        for (var i = 0; i < inputNum; i++) {
            nodes.Add(new Node(i));
            nextNode++;
            nodes[i].layer = 0;
        }
        //create output nodes
        for (int i = 0; i < outputNum; i++) {
            nodes.Add(new Node(i + inputNum));
            nodes[i + inputNum].layer = 2;
            nextNode++;
        }
    }
    
    Genome(int inpu, int outpu, bool crossover) {
        //set input number and output number
        inputNum = inpu; 
        outputNum = outpu;
    }
    public static void calculateFitness() {
        //fitness = fitness;
    }
    public Node getNode(int nodeNumber) {
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].number == nodeNumber) {
                return nodes[i];
            }
        }
        return null;
    }
    public void connectNodes() {
        for (int i = 0; i< nodes.Count; i++) {//clear the connections
            nodes[i].outputConnections.Clear();
        }
        for (int i = 0; i < genes.Count; i++) {//for each ConnectionGene 
            genes[i].rearrange();
            genes[i].fromNode.outputConnections.Add(genes[i]);//Add it to node
        }
    }
    public double[] feedForward(double[] inputValues) {
        //set the outputs of the input nodes
        moved();
        for (int i = 0; i < inputNum; i++) {
            nodes[i].outputValue = inputValues[i];
        }

        for (int i = 0; i < network.Count; i++) {
            // feed forward layer by layer
            network[i].engage();
        }

        double[] outs = new double[outputNum];
        for (int i = 0; i < outputNum; i++) {
            outs[i] = nodes[inputNum + i].outputValue;
        }

        for (int i = 0; i < nodes.Count; i++) { //reset all the nodes for the next feed forward
            nodes[i].inputSum = 0;
        }

        return outs;
    }
    public void generateNetwork() {
        connectNodes();
        network = new List<Node>();
        //for each layer Add the node in that layer, since layers cannot connect to themselves there is no need to order the nodes within a layer

        for (int l = 0; l < layers; l++) { // for each layer
            for (int i = 0; i < nodes.Count; i++) {// for each node
                if (nodes[i].layer == l) { // if that node is in that layer
                    network.Add(nodes[i]);
                }
            }
        }
    }
    public void moved(){
//        List<Node> unsortedNode = new List<Node>();
//        for (int i = inputNum + outputNum + 1; i < nodes.Count; i++){
//            unsortedNode.Add(nodes[i]);
//        }
//        unsortedNode = unsortedNode.OrderBy(x => x.x).ToList();
//        int t = 0;
//        for (int i = 0; i < unsortedNode.Count; i++){
//            unsortedNode[i].layer = i + 1;
//            t = i;
//        }
//        for (int j = 0; j < outputNum; j++){
//            nodes[j + inputNum].layer = t + 1;
//        }
//        layers = t+2;
        generateNetwork();
    }
    
    public Node addNode(int x) {
        int newNodeNo = nextNode;
        Node n = new Node(newNodeNo);
        n.x = x;
        nodes.Add(n);
        nextNode++;
        // orphaned node
        getNode(newNodeNo).layer = x;
        generateNetwork();
        return n;
    }
    
    public void addConnection(int n1, int n2) {
        addConnection(nodes[n1], nodes[n2]);
    }
    
    public void addConnection(Node n1, Node n2){
        Node temp;
        if (n1.layer > n2.layer) {//if the first random node is after the second then switch
            temp = n2;
            n2 = n1;
            n1 = temp;
        }        
        genes.Add(new ConnectionGene(n1, n2, Random.Range(-1f, 1f), 0));//changed this so if error here
        generateNetwork();
    }
    public static List<Genome> mutateWeights(List<Genome> pop) {
        List<Genome> returnPop = new List<Genome>();
        foreach (Genome genome in pop){
            Genome newGenome = genome.clone();
            Genome.mutateWeight(newGenome);
            returnPop.Add(newGenome);
        }
        return returnPop;
    }
    public static void mutateWeight(Genome original){
        for (int i = 0; i < original.genes.Count; i++) {
            double rand1 = Random.Range(0f, 1f);
            if (rand1 < 0.8) { // 80% of the time mutate weights
                original.genes[i].mutateWeight();
            }
        }
    }
    public static List<Genome> crossoverAll(List<Genome> original){
        int originalLength = original.Count;
        List<Genome> sorted = original.OrderBy(x => -x.fitness).ToList();

        // oof
        if (sorted.Count > 2) {
            for (int i = sorted.Count / 2; i < sorted.Count; i++) {
                sorted.RemoveAt(i); 
                i--;
            }
        }

        double fitnessSum = 0;
        for (int i = 0; i < sorted.Count; i++) {
            fitnessSum += sorted[i].fitness;
        }
        List<Genome> results = new List<Genome>(sorted);
        for (int i = 0; i < originalLength - sorted.Count; i++){
            Genome baby;
            if (Random.Range(0f, 1f) < 0.25) {
                //25% of the time there is no crossover and the child is simply a clone of a random(ish) player
                baby = selectGenome(fitnessSum, sorted).clone();
            } else {
                //75% of the time do crossover 
                //get 2 random(ish) parents 
                Genome parent1 = selectGenome(fitnessSum, sorted);
                Genome parent2 = selectGenome(fitnessSum, sorted);
                //the crossover function expects the highest fitness parent to be the object and the lowest as the argument
                if (parent1.fitness < parent2.fitness) {
                    baby = parent2.crossover(parent1);
                } else {
                    baby = parent1.crossover(parent2);
                }
            }
            results.Add(baby);
        }
        return results;
    }
    private static Genome selectGenome(double fitnessSum, List<Genome> sorted) {
        double rand = Random.Range(0f, (float)fitnessSum);
        double runningSum = 0;
        for (int i = 0; i < sorted.Count; i++) {
            runningSum += sorted[i].fitness; 
            if (runningSum > rand) {
                return sorted[i];
            }
        }
        //unreachable code to make the parser happy
        return sorted[0];
    }
    int getSameConnection(List<ConnectionGene> genes, ConnectionGene target){
        Node targetFrom = target.fromNode;
        Node targetEnd = target.toNode;
        for (int i = 0; i < genes.Count; i++){
            if (genes[i].fromNode.number == targetFrom.number && genes[i].toNode.number == targetEnd.number){
                return i;
            }
        }
        return -1;
    }    //called when this Genome is better that the other parent
    public Genome crossover(Genome parent2) {
        Genome child = new Genome(inputNum, outputNum, true);
        child.genes.Clear();
        child.nodes.Clear();
        child.layers = layers;
        child.nextNode = nextNode;
        List<ConnectionGene> childGenes = new List<ConnectionGene>();//list of genes to be inherrited form the parents
        List<bool> isEnabled = new List<bool>(); 
        //all inherrited genes
        for (int i = 0; i < genes.Count; i++) {
            bool setEnabled = true;//is this node in the chlid going to be enabled
            int parent2Gene = getSameConnection(parent2.genes, genes[i]);
            if (parent2Gene != -1){
                if (!genes[i].enabled || !parent2.genes[i].enabled) {//if either of the matching genes are disabled
                    if (Random.Range(0f, 1f) < 0.75) {//75% of the time disabel the childs gene
                        setEnabled = false;
                    }
                }
                double rand = Random.Range(0f, 1f);
                if (rand < 0.5) {
                    childGenes.Add(genes[i]);
                } else {
                    childGenes.Add(parent2.genes[parent2Gene]);
                }
            }else{
                childGenes.Add(genes[i]);
                setEnabled = genes[i].enabled;
            }
            isEnabled.Add(setEnabled);
        }

        for (int i = 0; i < nodes.Count; i++) {
            child.nodes.Add(nodes[i].clone());
        }
        //clone all the connections so that they connect the childs new nodes
        for ( int i = 0; i<childGenes.Count; i++) {
            child.genes.Add(childGenes[i].clone(child.getNode(childGenes[i].fromNode.number), child.getNode(childGenes[i].toNode.number)));
            child.genes[i].enabled = isEnabled[i];
        }
        child.generateNetwork();
        return child;
    }
    public void printGenome() {
//        Debug.Log("Print genome layers:" + layers);    
//        Debug.Log("nodes");
//        for (int i = 0; i < nodes.Count; i++) {
//            Debug.Log(nodes[i].number + ",");
//        }
//        Debug.Log("Genes");
//        for (int i = 0; i < genes.Count; i++) {//for each ConnectionGene 
//            Debug.Log("Gene from node " + genes[i].fromNode.number + " to node " + genes[i].toNode.number + 
//                " is enabled " + genes[i].enabled + " from layer " + genes[i].fromNode.layer + " to layer " + genes[i].toNode.layer + " weight: " + genes[i].weight);
//        }
//        Debug.Log("");
    }
    public Genome clone() {
        Genome clone = new Genome(inputNum, outputNum, true);

        for (int i = 0; i < nodes.Count; i++) {//copy nodes
            clone.nodes.Add(nodes[i].clone());
        }

        //copy all the connections so that they connect the clone new nodes
        for ( int i = 0; i < genes.Count; i++) {//copy genes
            clone.genes.Add(genes[i].clone(clone.getNode(genes[i].fromNode.number), clone.getNode(genes[i].toNode.number)));
        }
        clone.layers = layers;
        clone.nextNode = nextNode;
        clone.generateNetwork();
        return clone;
    }
    public List<List<Node>> returnNetwork() {
        List<List<Node>> allNodes = new List<List<Node>>();
        //split the nodes into layers
        for (int i = 0; i < layers; i++) {
            List<Node> temp = new List<Node>();
            for (int j = 0; j < nodes.Count; j++) {//for each node 
                if (nodes[j].layer == i ) {//check if it is in this layer
                    temp.Add(nodes[j]); //Add it to this layer
                }
            }
            allNodes.Add(temp);//Add this layer to all nodes
        }
        return allNodes;
    }
}