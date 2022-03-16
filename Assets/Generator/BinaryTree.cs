using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree
{
    private Node root { get; set; }
    private List<Node> leafNodes { get; set; } = new List<Node>();

    public void addRootNode(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        Node newNode = new Node(bottomLeft, bottomRight, topLeft, topRight);
        //Tree is empty
        if (this.root == null)
            this.root = newNode;
    }

    public Node addChildNode(Node parent, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        Node childNode = new Node(bottomLeft, bottomRight, topLeft, topRight);

        if(parent.leftNode == null) {
            // If left child does not exist, add new node to left branch
            childNode.parent = parent;
            childNode.appendToName("A");
            parent.leftNode = childNode;
        }
        else if(parent.rightNode == null) {
            // If right child does not exist, add new node to right branch   
            childNode.parent = parent;
            childNode.appendToName("B");
            parent.rightNode = childNode;

        }

        return childNode;
    }

    public Node getLeftChild(Node parent) {
        return parent.leftNode;
    }

    public Node getRightChild(Node parent) {
        return parent.rightNode;
    }

    public void updateLeafNodes(Node parent) {
        // If root does not exist
        if (root == null)
            return;

        // If node is leaf node (no left or right child)
        if(parent.leftNode == null && parent.rightNode == null)
            leafNodes.Add(parent);

        // If left child exists, check leaf recursively
        if(parent.leftNode != null)
            updateLeafNodes(parent.leftNode);

        // If right child exists, check leaf recursively
        if(parent.rightNode != null)
            updateLeafNodes(parent.rightNode);
    }

    public Node getRoot() {
        return this.root;
    }

    public List<Node> getLeafNodes() {
        return this.leafNodes;
    }
}
