using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Unit : WorldObject {
    public float moveSpeed, rotateSpeed;

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    protected override void OnGUI() {
        base.OnGUI();
    }

    public override void SetHoverState(GameObject hoverObject) {
        base.SetHoverState(hoverObject);
        if(player && player.human && currentlySelected) {
            if(hoverObject.name == "Ground") {
                player.ui.SetCursorState(CursorState.Move);
            }
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        base.MouseClick(hitObject, hitPoint, controller);
        //Only handle for human players, if player is owner and this unit is currently selected
        if(player && player.human && currentlySelected) {
            if(hitObject.name == "Ground" && hitPoint != ResourceManager.InvalidPosition) {
                float x = hitPoint.x;
                //Unit y is actually in the middle of the unit, this has to be added to the zero-level or unit will sink into the ground...
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3(x, y, z);
                StartMove(destination);
            }
        }
    }

    private void StartMove(Vector3 destination) {
        //TODO: Call CalculateBounds here or in submethods whenever position or rotation of unit has changed
        print("Moving to" + destination);
    }
}
