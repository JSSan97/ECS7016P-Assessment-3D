using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // Use dungeon in the Hierarchy
    public GameObject baseDungeon;
    // Main Camera of the hierarchy
    public GameObject mainCamera;

    // Set size of the whole dungeon
    public float baseDungeonWidth;
    public float baseDungeonHeight;

    // Set size of corridor
    public float corridorWidth;

    // Wall Height 
    public float wallHeight;

    // Min & Max number of Splits to split tree, chosen randomly between (more variation)
    public int minSplits;
    public int maxSplits;

    // Minimal Acceptable Size for Rooms
    public float roomMinHeightAcceptance;
    public float roomMinWidthAcceptance;

    // Tree data structure to record nodes and branches
    private BinaryTree BSPTree = new BinaryTree();
    // Keep track of the number of iterations in the algorithm
    private int currentSplits;

    // Dungeon Drawer
    private DungeonDrawer dungeonDrawer;

    // Choose number of splits between min and max
    private int randomSplits;

    // Start is called before the first frame update
    void Start()
    {
        this.dungeonDrawer = new DungeonDrawer(this.gameObject, this.corridorWidth, this.wallHeight);
        // Setup Dungeon from parameters
        SetupBaseDungeon();
        // Build Tree
        BuildBSP();
        // Build Parition Quads
        BuildPartitions();
        // Build Rooms
        BuildRooms();
        // Build Corridors
        BuildCorridors();
        // Populate Rooms
        PopulateRooms();
    }

    void SetupBaseDungeon()
    {
        randomSplits = Random.Range(minSplits, maxSplits + 1);
        // Scale and Orientate Dungeon, bottom left corner is (0, 0, 0) -> Easier to do maths
        baseDungeon.transform.localScale = new Vector3(baseDungeonWidth, baseDungeonHeight, 1);
        baseDungeon.transform.position = new Vector3(baseDungeonWidth/2, 0, baseDungeonHeight/2);

        // Bring Camera to Centre of Dungeon
        mainCamera.transform.position = new Vector3(baseDungeonWidth/2, 40, baseDungeonHeight/2);

        // Add Root Node to Tree including the positions of each corner
        Vector3 bottomLeft, bottomRight, topLeft, topRight;
        bottomLeft = new Vector3(0, 0, 0);
        bottomRight = new Vector3(baseDungeonWidth, 0, 0);
        topLeft = new Vector3(0, 0, baseDungeonHeight);
        topRight = new Vector3(baseDungeonWidth, 0, baseDungeonHeight);
        BSPTree.AddRootNode(bottomLeft, bottomRight, topLeft, topRight);
    }
    
    private void BuildBSP()
    {
        // Record a queue of nodes that need to be partitioned
        LinkedList<Node> queue = new LinkedList<Node>();

        // Start with parent node
        queue.AddFirst(BSPTree.GetRoot());

        while(currentSplits < randomSplits) {
            if (queue.Count == 0)
                break;

            // Get first item in queue
            Node parent = queue.First.Value;
            queue.RemoveFirst();

            // Choose partition direction
            int splitDirection = getPartitionDirection(parent);

            if(isAcceptableSize(parent, splitDirection)) {
                // Get allowed split position given the chosen direction
                float splitPosition = getPartitionPosition(splitDirection, parent);

                // Paritition parent dungeon
                partitionCell(parent, splitDirection, splitPosition);

                // Add left and right child to queue to be partitioned
                queue.AddLast(BSPTree.GetLeftChild(parent));
                queue.AddLast(BSPTree.GetRightChild(parent));

                currentSplits += 1;
            }
        }
        // Find all leaf nodes from root and update the leaf nodes list in BSPTree object.
        BSPTree.UpdateLeafNodesFromRoot();
        // Update Max Depth, need for drawing corridors later
        BSPTree.UpdateMaxDepth();

    }

    private void BuildPartitions()
    {
        // Display all leaf quads/partitions
        foreach (Node node in BSPTree.GetLeafNodes()) {
            dungeonDrawer.DrawPartition(node);
        }
    }

    private void BuildRooms() 
    {
        // Draw all leaf node rooms
        foreach (Node node in BSPTree.GetLeafNodes()) {
            node.UpdateRoomSpace();
            dungeonDrawer.DrawRoom(node);
        }
    }

    private void BuildCorridors()
    {
        int currentDepth = BSPTree.GetMaxDepth() - 1;
        // Draw all corridors of children, starting from the lowest level
        List<Node> parents;
        parents = BSPTree.GetNodesAtDepth(currentDepth);
        // Starting from the lowest level
        foreach (Node node in parents) {
            if(node.leftNode != null && node.rightNode != null) {
                dungeonDrawer.DrawCorridors(node.leftNode, node.rightNode);
            }
        }
        currentDepth = currentDepth - 1;

        while(currentDepth > -1) {
            parents = BSPTree.GetNodesAtDepth(currentDepth);
            foreach (Node node in parents) {
                if(node.leftNode != null && node.rightNode != null) {
                    // Get left and right leaf children of the node
                    List<Node> leftChildren = BSPTree.GetLeafNodes(node.leftNode);
                    List<Node> rightChildren = BSPTree.GetLeafNodes(node.rightNode);

                    Node node1 = null;
                    Node node2 = null;
                    float minDistance = 0;

                    foreach(Node leftNode in leftChildren) {
                        foreach(Node rightNode in rightChildren) {
                            float tempDistance = Vector3.Distance(leftNode.GetRoomCentre(), rightNode.GetRoomCentre());

                            if((node1 == null) || (tempDistance < minDistance) ) {
                                node1 = leftNode;
                                node2 = rightNode;
                                minDistance = tempDistance;
                            }
                        }
                    }
                    dungeonDrawer.DrawCorridors(node1, node2);
                }
            }
            currentDepth = currentDepth - 1;
        }
    }

    private bool isAcceptableSize(Node node, int splitDirection)
    {
        if(splitDirection == 1) {
            // Get height of the room
            float height = Vector3.Distance(node.topRight, node.bottomRight);
            return (height > roomMinHeightAcceptance) ? true : false;
        }
        else {
            // Get width of the room
            float width = Vector3.Distance(node.bottomLeft, node.bottomRight);
            return (width > roomMinWidthAcceptance) ? true : false;
        }
    }

    private int getPartitionDirection(Node node)
    {
        float height = Vector3.Distance(node.topRight, node.bottomRight);
        float width = Vector3.Distance(node.bottomLeft, node.bottomRight);
        // 1 = Horizontal, 2 = Vertical
        int splitDirection;
        if(width < height) {
            splitDirection = 1;
        } else if (width > height) {
            splitDirection = 2;
        }
        else {
            // Choose Randomly if equal height and width. 
            splitDirection = Random.Range(1, 3);
        }
    
        return splitDirection;
    }

    private float getPartitionPosition(int splitDirection, Node node) 
    {
        float splitPosition;
        // Offset as the position could result in extremely narrow partitions without it
        float offset;
        // Get Split Position, either horizontally or vertically
        if (splitDirection == 1) {
            // Split on the y axis. I.e. y = splitPosition for horiztonal partition
            offset = (node.topLeft.z - node.bottomLeft.z) / 4;
            splitPosition = Random.Range(node.bottomLeft.z + offset, node.topLeft.z - offset);
        } else {
            // Split on the x axis. I.e. x = splitPosition for vertical partition
            offset = (node.bottomRight.x - node.bottomLeft.x) / 4;
            splitPosition =  Random.Range(node.bottomLeft.x + offset, node.bottomRight.x - offset);
        }
        splitPosition = Mathf.Round(splitPosition);
       
        return splitPosition;
    }

    private void partitionCell(Node node, int splitDirection, float splitPosition)
    {
        // Create new child nodes with corner information and add to tree
        Vector3 bottomLeft, bottomRight, topLeft, topRight;
        Node child1;
        Node child2;
        if(splitDirection == 1) {
            // Horizontal Bottom
            bottomLeft = node.bottomLeft;
            bottomRight = node.bottomRight;
            topLeft = new Vector3(node.topLeft.x, node.topLeft.y, splitPosition);
            topRight = new Vector3(node.topRight.x, node.topRight.y,  splitPosition);
            child1 = BSPTree.AddChildNode(node, bottomLeft, bottomRight, topLeft, topRight);

            // Horizontal Top
            bottomLeft = new Vector3(node.topLeft.x, node.topLeft.y, splitPosition);
            bottomRight = new Vector3(node.topRight.x, node.topRight.y, splitPosition);
            topLeft = node.topLeft;
            topRight = node.topRight;
            child2 = BSPTree.AddChildNode(node, bottomLeft, bottomRight, topLeft, topRight);

        } else {
            // Vertical Left
            bottomLeft = node.bottomLeft;
            bottomRight = new Vector3(splitPosition, node.bottomRight.y, node.bottomRight.z);
            topLeft = node.topLeft;
            topRight = new Vector3(splitPosition, node.topRight.y, node.topRight.z);
            child1 = BSPTree.AddChildNode(node, bottomLeft, bottomRight, topLeft, topRight);
            // Vertical Right
            bottomLeft = new Vector3(splitPosition, node.bottomRight.y, node.bottomRight.z);
            bottomRight = node.bottomRight;
            topLeft = new Vector3(splitPosition, node.topRight.y, node.topRight.z);
            topRight = node.topRight;
            child2 = BSPTree.AddChildNode(node, bottomLeft, bottomRight, topLeft, topRight);
        }

        // Need to know parents and siblings for connecting rooms with corridors.
        child1.sibling = child2;
        child2.sibling = child1;
    }

    private void PopulateRooms(){
        foreach (Node node in BSPTree.GetLeafNodes()) {
            node.UpdateRoomSpace();
            dungeonDrawer.PopulateRoom(node);
        }        
    }
}
