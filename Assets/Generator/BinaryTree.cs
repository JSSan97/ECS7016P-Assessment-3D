using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree
{
    private Node root { get; set; }
    private List<Node> leafNodes { get; set; } = new List<Node>();
    private int depth = 0;

    public void AddRootNode(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        Node newNode = new Node(bottomLeft, bottomRight, topLeft, topRight);
        //Tree is empty
        if (this.root == null)
            this.root = newNode;
    }

    public Node AddChildNode(Node parent, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        Node childNode = new Node(bottomLeft, bottomRight, topLeft, topRight);

        if(parent.leftNode == null) {
            // If left child does not exist, add new node to left branch
            childNode.parent = parent;
            childNode.AppendToName("A");
            childNode.UpdateDepth();
            parent.leftNode = childNode;
        }
        else if(parent.rightNode == null) {
            // If right child does not exist, add new node to right branch   
            childNode.parent = parent;
            childNode.AppendToName("B");
            childNode.UpdateDepth();
            parent.rightNode = childNode;
        }

        return childNode;
    }

    public Node GetLeftChild(Node parent) {
        return parent.leftNode;
    }

    public Node GetRightChild(Node parent) {
        return parent.rightNode;
    }

    public void UpdateLeafNodesFromRoot() {
        // If root does not exist
        if (root == null)
            return;
        leafNodes = new List<Node>();
        SearchLeafNodes(root, leafNodes);
        this.leafNodes = leafNodes;
    }

    public void UpdateMaxDepth() {
        this.depth = GetMaxDepth(this.root) - 1;
    }

    public List<Node> GetLeafNodes(Node node) {
        List<Node> leafNodesFromParent = new List<Node>();
        SearchLeafNodes(node, leafNodesFromParent);
        return leafNodesFromParent;
    }

    private void SearchLeafNodes(Node parent, List<Node> list) {
        // If node is leaf node (no left or right child)
        if(parent.leftNode == null && parent.rightNode == null)
            list.Add(parent);

        // If left child exists, check leaf recursively
        if(parent.leftNode != null)
            SearchLeafNodes(parent.leftNode, list);

        // If right child exists, check leaf recursively
        if(parent.rightNode != null)
            SearchLeafNodes(parent.rightNode, list);
    }

    public List<Node> GetNodesAtDepth(int level) {
        List<Node> list = new List<Node>();
        this.SearchNodesAtDepth(this.root, level, list);
        return list;
    }

    private void SearchNodesAtDepth(Node node, int level, List<Node> list) {
        // If node is leaf node (no left or right child)
        if(node.depth == level) {
            list.Add(node);
            // Return as we don't need to go anymore further
            return;
        } 

        // If left child exists, check leaf recursively
        if(node.leftNode != null)
            SearchNodesAtDepth(node.leftNode, level, list);

        // If right child exists, check leaf recursively
        if(node.rightNode != null)
            SearchNodesAtDepth(node.rightNode, level, list);
    }
    
    private int GetMaxDepth(Node root) {
        if(root == null) {
            return 0;
        }
        // Get depth of the left and right subtree using recursion
        int leftDepth = GetMaxDepth(root.leftNode);
        int rightDepth = GetMaxDepth(root.rightNode);

        // Choose larger one and add the root to it
        if (leftDepth > rightDepth) {
            return leftDepth + 1;
        } else {
            return rightDepth + 1;
        }
    }

    public Node GetRoot() {
        return this.root;
    }

    public List<Node> GetLeafNodes() {
        return this.leafNodes;
    }

    public int GetMaxDepth() {
        return this.depth;
    }
}
