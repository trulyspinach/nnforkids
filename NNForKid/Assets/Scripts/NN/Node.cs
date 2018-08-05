using System.Collections.Generic;
using UnityEngine;
using System;

public class Node {
    public int number;
    public double inputSum = 0;//current sum i.e. before activation
    public double outputValue = 0; //after activation function is applied
    public List<ConnectionGene> outputConnections = new List<ConnectionGene>();
    public int layer = 0;
    public double x;
    public double y;
    public Node(int no) {
        number = no;
    }
    public void engage() {
        if (layer != 0) {//no activator for the inputs and bias
            outputValue = tanhActivator(inputSum);
        }
        for (int i = 0; i < outputConnections.Count; i++) { //for each connection
            if (outputConnections[i].enabled) { //dont do shit if not enabled
                outputConnections[i].toNode.inputSum += outputConnections[i].weight * outputValue;
                //add the weighted output to the sum of the inputs of whatever node this node is connected to
            }
        }
    }
    double stepActivator(double x) {
        if (x < 0) {
            return 0;
        } else {
            return 1;
        }
    }
    double sigmoidActivator(double x) {
        var y = 1 / (1 + Math.Pow(Math.E, -4.9 * x));
        return y;
    }
    double tanhActivator(double x) {
        var y = System.Math.Tanh(x);
        return y;
    }

    //returns whether this node connected to the parameter node
    //used when adding a new connection 
    public bool isConnectedTo(Node node) {
        if (node.layer == layer) {
            //nodes in the same layer cannot be connected
            return false;
        }
        if (node.layer < layer) {
            for (int i = 0; i < node.outputConnections.Count; i++) {
                if (node.outputConnections[i].toNode == this) {
                    return true;
                }
            }
        } else {
            for (int i = 0; i < outputConnections.Count; i++) {
                if (outputConnections[i].toNode == node) {
                    return true;
                }
            }
        }
        return false;
    }
    public Node clone() {
        Node clone = new Node(number);
        clone.layer = layer;
        return clone;
    }
}