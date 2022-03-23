using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree
{
    private Node root { get; set; }
    private List<Node> leafNodes { get; set; } = new List<Node>();
    private int depth = 0;

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
            childNode.updateDepth();
            parent.leftNode = childNode;
        }
        else if(parent.rightNode == null) {
            // If right child does not exist, add new node to right branch   
            childNode.parent = parent;
            childNode.appendToName("B");
            childNode.updateDepth();
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

    public void updateLeafNodesFromRoot() {
        // If root does not exist
        if (root == null)
            return;
        leafNodes = new List<Node>();
        searchLeafNodes(root, leafNodes);
        this.leafNodes = leafNodes;
    }

    public void updateMaxDepth() {
        this.depth = getMaxDepth(this.root) - 1;
    }

    public List<Node> getLeafNodes(Node node) {
        List<Node> leafNodesFromParent = new List<Node>();
        searchLeafNodes(node, leafNodesFromParent);
        return leafNodesFromParent;
    }

    private void searchLeafNodes(Node parent, List<Node> list) {
        // If node is leaf node (no left or right child)
        if(parent.leftNode == null && parent.rightNode == null)
            list.Add(parent);

        // If left child exists, check leaf recursively
        if(parent.leftNode != null)
            searchLeafNodes(parent.leftNode, list);

        // If right child exists, check leaf recursively
        if(parent.rightNode != null)
            searchLeafNodes(parent.rightNode, list);
    }

    public List<Node> getNodesAtDepth(int level) {
        List<Node> list = new List<Node>();
        this.searchNodesAtDepth(this.root, level, list);
        return list;
    }

    private void searchNodesAtDepth(Node node, int level, List<Node> list) {
        // If node is leaf node (no left or right child)
        if(node.depth == level) {
            list.Add(node);
            // Return as we don't need to go anymore further
            return;
        } 

        // If left child exists, check leaf recursively
        if(node.leftNode != null)
            searchNodesAtDepth(node.leftNode, level, list);

        // If right child exists, check leaf recursively
        if(node.rightNode != null)
            searchNodesAtDepth(node.rightNode, level, list);
    }
    
    private int getMaxDepth(Node root) {
        if(root == null) {
            return 0;
        }
        // Get depth of the left and right subtree using recursion
        int leftDepth = getMaxDepth(root.leftNode);
        int rightDepth = getMaxDepth(root.rightNode);

        // Choose larger one and add the root to it
        if (leftDepth > rightDepth) {
            return leftDepth + 1;
        } else {
            return rightDepth + 1;
        }
    }

    public Node getRoot() {
        return this.root;
    }

    public List<Node> getLeafNodes() {
        return this.leafNodes;
    }

    public int getMaxDepth() {
        return this.depth;
    }
}
