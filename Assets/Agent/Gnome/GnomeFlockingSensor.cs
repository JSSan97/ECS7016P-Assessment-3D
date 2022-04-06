using UnityEngine;
using System.Collections.Generic;
using UnityMovementAI;

// Note that this is a modified version of NearSensor in UnityMovementAI
// Modified so that only gnomes are flocking together rather than all MovementAIRigidBodies
public class GnomeFlockingSensor : MonoBehaviour
{
    public HashSet<MovementAIRigidbody> _targets = new HashSet<MovementAIRigidbody>();
    private bool isPlayerNearby = false;

    public HashSet<MovementAIRigidbody> targets
    {
        get
        {
            /* Remove any MovementAIRigidbodies that have been destroyed */
            _targets.RemoveWhere(IsNull);
            return _targets;
        }
    }

    static bool IsNull(MovementAIRigidbody r)
    {
        return (r == null || r.Equals(null));
    }

    void TryToAdd(Component other)
    {
        MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
        if (rb != null)
        {
            _targets.Add(rb);
        }
    }

    void TryToRemove(Component other)
    {
        MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
        if (rb != null)
        {
            _targets.Remove(rb);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            isPlayerNearby = true;

        if(other.gameObject.tag == "Gnome" || other.gameObject.tag == "Player")
            TryToAdd(other);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
            isPlayerNearby = false;

        if(other.gameObject.tag == "Gnome" || other.gameObject.tag == "Player")
            TryToRemove(other);
    }

    public bool getIsPlayerNearby(){
        return this.isPlayerNearby;
    }
}
