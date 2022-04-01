using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree
{
    // Root node, aka the full dungeon that hasn't been partitioned
    private BSPNode root { get; set; }
    // Keep a list of leafNodes for building the dungeon
    private List<BSPNode> leafNodes { get; set; } = new List<BSPNode>();
    // Depth of the tree (expected to be updated)
    private int depth = 0;

    public void AddRootNode(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        // Add the root node, the full dungeon without partitioning
        BSPNode newNode = new BSPNode(bottomLeft, bottomRight, topLeft, topRight);
        //Tree is empty
        if (this.root == null)
            this.root = newNode;
    }

    public BSPNode AddChildNode(BSPNode parent, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        // Add children for partitioning
        BSPNode childNode = new BSPNode(bottomLeft, bottomRight, topLeft, topRight);

        if(parent.leftNode == null) {
            // If left child does not exist, add new node to left branch
            childNode.parent = parent;
            // Good to append 'A' at the name of the child node from the parent, to keep track for debugging.
            childNode.AppendToName("A");
            // Set the depth of the child node (at which depth is it at the tree)
            childNode.UpdateDepth();
            // Set the parents left node to the child node.
            parent.leftNode = childNode;
        }
        else if(parent.rightNode == null) {
            // If right child does not exist, add new node to right branch   
            childNode.parent = parent;
            // Good to append 'B' at the name of the child node from the parent, to keep track for debugging.
            childNode.AppendToName("B");
            // Set the depth of the child node (at which depth is it at the tree)
            childNode.UpdateDepth();
            // Set the parents right node to the child node.
            parent.rightNode = childNode;
        }

        return childNode;
    }

    public BSPNode GetLeftChild(BSPNode parent) {
        // Get the left child of a node.
        return parent.leftNode;
    }

    public BSPNode GetRightChild(BSPNode parent) {
        // Get the right child of a node.
        return parent.rightNode;
    }

    public void UpdateLeafNodesFromRoot() {
        // If root does not exist
        if (root == null)
            return;
        // Update the leaf nodes (method expected to be used after partitioning)
        leafNodes = new List<BSPNode>();
        SearchLeafNodes(root, leafNodes);
        this.leafNodes = leafNodes;
    }

    public void UpdateMaxDepth() {
        // Update the max depth, expected to be used after partitioning
        this.depth = GetMaxDepth(this.root) - 1;
    }

    public List<BSPNode> GetLeafNodes(BSPNode node) {
        // Return the leaf nodes from a particular node (not just the root)
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
        // Get all nodes at a partiular depth of the tree.
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
