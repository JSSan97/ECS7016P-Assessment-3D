# ECS7016P-Assessment-3D
 Procedural Content Generatin (PCG) and Interactive Agents Demo.

# Third Party Libraries
Disclaimer!! I do not own any of the third party libraries. These libraries are located in Assets/Third Party.

NPBehave - https://github.com/meniku/NPBehave

Unity Movement AI - https://github.com/sturdyspoon/unity-movement-ai

# Procedural Content Generation (PCG)

This project combines Binary Space Partition Trees to partition a dungeon into rooms and cellular automata to populate rooms with tiles. The code for dungeon generation can be found in Assets/Generator.

## Generator (Monobehaviour)

This handles the main sequence of generating a dungeon and spawning the agents and player. In order: setting up the base dungeon, building a BSP tree, partitioning the dungeon, building rooms and corridors and populating tiles and agents per room. This script must be attached to a game object of the scene and public variables assigned. The following public variables can be adjusted in the inspector once assigned to a game object:

- *baseDungeon (GameObject)*: An empty dungeon gameobject, an example found in Assets/Generator/Prefabs/Base Dungeon
- *mainCamera (GameObject)*: Camera used that will follow the player
- *gnome (GameObject)*: Gnome agent, found in Assets/Agent/Prefabs/
- *hunter (GameObject)*: Hunter agent, found in Assets/Agent/Prefabs/
- *player (GameObject)*: Player, found in Assets/Player/Prefabs
- *baseDungeonWidth (float)*: Width of the dungeon
- *baseDungeonHeight (float)*: Height of the dungeon
- *corridorWidth (float)*: Width of the generated corridors
- *wallHeight (float)*: Height of the generated walls
- *minSplits (int)*: The minimum number of b-tree splits. The PCG chooses the number of splits between minSplits and maxSplits.
- *maxSplits (int)*: The maximum number of b-tree splits. The PCG chooses the number of splits between minSplits and maxSplits.
- *roomMinHeightAcceptance (float)*: Any room with a height smaller than this value will no longer be split.
- *roomMinWidthAcceptance (float)*: Any room with a width smaller than this value will no longer be split.
- *gnomesPerRoom*: Spawn a number of gnomes per room. Note spawning too many agents can result in lag.
- *huntersPerRoom*: Spawn a number of hunters per room. Note spawning too many agents can result in lag.

## BinaryTree
Class that contains information about the tree and methods for retrieving, adding and searching for nodes,

## BSPNode
Class that contains information about a partition and a room within the partition. This includes partition space, room space; children, sibling, parents, depth of a node, vector room and partition corners and name of the room.

## CellularAutomata
Class that contains the logic behind populating rooms with tiles using Moores Neighbourhood cellular automata technique. 

## DungeonDrawer
Class that contains the logic behind building the room and corridor game objects. 

## Settings
Static class for settings. Currently has settings for colours of tiles.

## Tile
Class that when instantiated, creates a tile or 'cube' object based on the parameters set in the constructor. 


# Interactive Agents
This project combines the use of steering from the *Unity Movement AI* library and behaviour trees with the aid of *NPBehave* to decide on actions for the interactive agent. The code for the interactive agents can be found in Assets/Agent.

## Gnome
Gnomes quench their thirst by seeking water under a certain threshhold of thirst and heal themselves in grass when under a certain threshold of health. They also flee away from hunters, hiding in grass when there is grass in sight. Gnomes also flock towards the player (and other gnomes in that case) when the player is in close proximity. Note that gnomes priorities certain behaviours than others (see GnomeBT). The relevant code can be found in Assets/Agent/Hunter.

### Gnome (MonoBehaviour)
Contains information and logic about the gnome, including their health and thirst. Also handles the onTrigger events when touching water or grass tiles. Attached to the gnome agent game object.

### GnomeBT (MonoBehaviour)
Contains the logic behaviour of the gnome using behaviour trees. Attached to the gnome agent game object. In FixedUpdate, the chosen steering behaviour is performed.

### GnomeFlockingSensor (MonoBehaviour)
Contains information of nearby gnomes and nearby players. Makes use of onTrigger events to keep track of these targets for flocking behaviour. The script is attached to the flocking sensor, a child of the gnome agent game object. This game object is expected to contain a trigger collider and rigidbody so it is treated seperately to the parent.

### GnomePerception (MonoBehaviour)
Contains logic and information of keeping track of the nearest water and grass tiles using onTrigger events. The script is attached to the Perception game object, a child of the gnome agent game object. This game object is expected to contain a trigger collider and rigidbody so it is treated seperately to the parent.

### GnomeThreatField (MonoBehaviour)
Contains logic and information of keeping track of the nearest hunter using onTrigger events. The script is attached to the Threat Field game object, a child of the gnome agent game object. This game object is expected to contain a trigger collider and rigidbody so it is treated seperately to the parent.

## Hunter
Hunters seek gnomes to attack. Hunters can not perceive a gnome when the gnome is in grass. They flee from the player when in close proximity. Note that hunters priorities certain behaviours than others (see HunterBT). The relevant code can be found in Assets/Agent/Gnome.

### HunterBT (MonoBehaviour)
Contains the logic behaviour of the hunter using behaviour trees. Attached to the hunter agent game object. In FixedUpdate, the chosen steering behaviour is performed.
 
### HunterPerception (MonoBehaviour)
Contains logic and information of keeping track of the nearest gnome using onTrigger events. The script is attached to the Perception game object, a child of the hunter agent game object. This game object is expected to contain a trigger collider and rigidbody so it is treated seperately to the parent.

### HunterThreatField (MonoBehaviour)
Contains logic and information of keeping track of the player using onTrigger events. The script is attached to the Threat Field game object, a child of the hunter agent game object. This game object is expected to contain a trigger collider and rigidbody so it is treated seperately to the parent.

## Behaviours
Behaviours can be found in Assets/Agent/Behaviours/. Many of the scripts can be considered wrappers for using the steering behaviours from the *Unity Movement AI* library extending from the abstract class 'CustomBehaviour'. 

### CustomBehaviour (Abstract)
This is instantiated in HunterBT and AgentBT. The actual behaviour is switched/assigned in the decision tree actions e.g. instantiating BehaviourWander, BehaviourFlock, BehaviourSeek etc and performed in FixedUpdate of the HunterBT or AgentBT.