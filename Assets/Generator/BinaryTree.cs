using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree
{
    private BSPNode root { get; set; }
    private List<BSPNode> leafNodes { get; set; } = new List<BSPNode>();
    private int depth = 0;

    public void AddRootNode(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        BSPNode newNode = new BSPNode(bottomLeft, bottomRight, topLeft, topRight);
        //Tree is empty
        if (this.root == null)
            this.root = newNode;
    }

    public BSPNode AddChildNode(BSPNode parent, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        BSPNode childNode = new BSPNode(bottomLeft, bottomRight, topLeft, topRight);

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

    public BSPNode GetLeftChild(BSPNode parent) {
        return parent.leftNode;
    }

    public BSPNode GetRightChild(BSPNode parent) {
        return parent.rightNode;
    }

    public void UpdateLeafNodesFromRoot() {
        // If root does not exist
        if (root == null)
            return;
        leafNodes = new List<BSPNode>();
        SearchLeafNodes(root, leafNodes);
        this.leafNodes = leafNodes;
    }

    public void UpdateMaxDepth() {
        this.depth = GetMaxDepth(this.root) - 1;
    }

    public List<BSPNode> GetLeafNodes(BSPNode node) {
        List<BSPNode> leafNodesFromParent = new List<BSPNode>();
        SearchLeafNodes(node, leafNodesFromParent);
        return leafNodesFromParent;
    }

    private void SearchLeafNodes(BSPNode parent, List<BSPNode> list) {
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

    public List<BSPNode> GetNodesAtDepth(int level) {
        List<BSPNode> list = new List<BSPNode>();
        this.SearchNodesAtDepth(this.root, level, list);
        return list;
    }

    private void SearchNodesAtDepth(BSPNode node, int level, List<BSPNode> list) {
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
    
    private int GetMaxDepth(BSPNode root) {
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

    public BSPNode GetRoot() {
        return this.root;
    }

    public List<BSPNode> GetLeafNodes() {
        return this.leafNodes;
    }

    public int GetMaxDepth() {
        return this.depth;
    }
}
