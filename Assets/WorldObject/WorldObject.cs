using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class WorldObject : MonoBehaviour {

    public string objectName;

    protected Player player;
    protected string[] actions = { };
    protected bool currentlySelected = false;

	// Use this for initialization
	protected virtual void Start () {
        player = transform.root.GetComponentInChildren<Player>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}

    public string[] GetActions() {
        return actions;
    }

    public Player GetOwner() {
        return player;
    }

    public virtual void PerformAction(string action) {

    }

    /**
     * Sets the cursor state depending on what this unit can do with the object currently being hovered over.
     */
    public virtual void SetHoverState(WorldObject worldObject) {
        player.ui.SetDefaultHoverState(worldObject);
        //Override for more specific behaviour
    }

    public virtual void SetGroundHoverState() {
        player.ui.SetCursorState(CursorState.Idle);
        //Override for more specific behaviour
    }

    /**
     * Define behaviour for the selected worldobject on a right-click. Default checks if the ground or another object was clicked
     * and calls the corresponding RightClickGround or RightClickObject methods.
     */
    public virtual void RightClickObject(WorldObject worldObject) {
        //Deafult behaviour is to do nothing, only Units should try to interact with the world.
    }

    public virtual void RightClickGround(Vector3 hitPoint) {
        //Deafult behaviour is to do nothing, only Units should move.
    }
}
