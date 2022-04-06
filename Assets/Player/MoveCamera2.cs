using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera2 : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset; 

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 10.0f, player.transform.position.z - 5.0f);
    }
}
