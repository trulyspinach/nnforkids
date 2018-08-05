using UnityEngine;
using System.Collections.Generic;

public class ConnectionGene {
    public int invID = 0;
    public Node fromNode;
    public Node toNode;
    public double weight;
    public bool enabled = true;

    public ConnectionGene(Node from, Node to, double w, int id) {
        fromNode = from;
        toNode = to;
        weight = w;
        invID = id;
    }
    //changes the weight
    public void mutateWeight() {
        double rand2 = Random.Range(0f, 1f);
        if (rand2 < 0.1) { //10% of the time completely change the weight
            weight = Random.Range(-1f, 1f);
        } else { //otherwise slightly change it
            weight += stdNormal() / 50;
            if(weight > 1){
                weight = 1;
            }
            if(weight < -1){
                weight = -1;
                
            }
        }
    }

    public static double stdNormal(){
        double u1 = 1.0 - Random.Range(0f, 1f); //uniform(0,1] random doubles
        double u2 = 1.0-Random.Range(0f, 1f);
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); 
        return randStdNormal;
    }

    public void rearrange(){
        if (fromNode.layer > toNode.layer){
            Node temp = fromNode;
            fromNode = toNode;
            toNode = temp;
        }
        if (fromNode.layer == toNode.layer){
            Debug.LogError("broke");
        }
    }

    //returns a copy of this connectionGene
    public ConnectionGene clone(Node from, Node to) {
        ConnectionGene clone = new ConnectionGene(from, to, weight, invID);
        clone.enabled = enabled;
        return clone;
    }
}