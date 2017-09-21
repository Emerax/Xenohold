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

    public void SetSelection(bool selected, Rect playingArea) {
        currentlySelected = selected;
        if (selected) this.playingArea = playingArea;
    }

    public string[] GetActions() {
        return actions;
    }

    public virtual void PerformAction(string action) {

    }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        if(currentlySelected && hitObject) {
            if(hitObject.name != "Ground") {
                WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
                if (worldObject) ChangeSelection(worldObject, controller);
            } else {
                //Deselect if left clicking the ground
                controller.SelectedObject.SetSelection(false, playingArea);
                controller.SelectedObject = null;
            }
        }
    }

    private void ChangeSelection(WorldObject worldObject, Player controller) {
        SetSelection(false, playingArea);
        if (controller.SelectedObject) controller.SelectedObject.SetSelection(false, playingArea);
        controller.SelectedObject = worldObject;
        worldObject.SetSelection(true, controller.ui.GetPlayingArea());
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
