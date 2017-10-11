using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class WorldObject : MonoBehaviour {

    public string objectName;
    public Texture2D buildImage;
    public int cost, sellValue, hitPoints, maxHitPoints;

    protected Player player;
    protected string[] actions = { };
    protected bool currentlySelected = false;
    protected Bounds selectionBounds;
    protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    protected virtual void Awake() {
        selectionBounds = ResourceManager.InvalidBounds;
        CalculateBounds();
    }

	// Use this for initialization
	protected virtual void Start () {
        player = transform.root.GetComponentInChildren<Player>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}

    protected virtual void OnGUI() {
        if (currentlySelected) {
            DrawSelection();
        }
    }

    protected virtual void DrawSelectionBox(Rect selectBox) {
        GUI.Box(selectBox, "");
    }

    protected virtual void RightClickGround(Vector3 hitPoint) {

    }

    public void SetSelection(bool selected, Rect playingArea) {
        currentlySelected = selected;
        if (selected) this.playingArea = playingArea;
    }

    public string[] GetActions() {
        return actions;
    }

    public virtual void PerformAction(string action) {

    }

    /**
     * Sets the mouse cursor depending on the state of this worldobject, and what the cursor hovers over.
     */
    public virtual void SetHoverState(GameObject hoverObject) {
        //only handle for human player, if and when they have something selected
        if(player && player.human && currentlySelected) {
            if(hoverObject.name != "Ground") {
                player.ui.SetCursorState(CursorState.Select);
            }else {
                player.ui.SetCursorState(CursorState.Idle);
            }
        }
    }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        if(currentlySelected && hitObject) {
            if(hitObject.name != "Ground") {
                WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
                if (worldObject) ChangeSelection(worldObject, controller);
            } else {
                //Deselect if left clicking the ground
                controller.SelectedObject.SetSelection(false, playingArea);
                controller.SelectedObject = null;
            }
        }
    }

    /**
     * Define behaviour for the selected worldobject on a right-click. Default checks if the ground or another object was clicked
     * and calls the corresponding RightClickGround or RightClickObject methods.
     */
    public virtual void RightClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        //Only the owner should be able to order the object
        if(player && player.human && currentlySelected && hitObject && controller == player) {
            if(hitObject.name != "Ground") {
            } else {
                RightClickGround(hitPoint);
            }
        }
    }

    private void ChangeSelection(WorldObject worldObject, Player controller) {
        WorldObject newSelection = worldObject;
        if(worldObject is Ore) {
            print("CLICKED ORE");
            Ore ore = (Ore)worldObject;
            if (!ore.Uncarried()) {
                print("IT WAS CARRIED");
                newSelection = ore.GetCarrier();
            }
        }
        SetSelection(false, playingArea);
        if (controller.SelectedObject) controller.SelectedObject.SetSelection(false, playingArea);
        controller.SelectedObject = newSelection;
        newSelection.SetSelection(true, controller.ui.GetPlayingArea());
    }

    private void DrawSelection() {
        GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
        GUI.BeginGroup(playingArea);
        DrawSelectionBox(selectBox);
        GUI.EndGroup();
    }

    public void CalculateBounds() {
        selectionBounds = new Bounds(transform.position, Vector3.zero);
        foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
            selectionBounds.Encapsulate(r.bounds);
        }
    }
}
