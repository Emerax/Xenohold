using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour {

    public string username;
    public bool human;
    public UI ui;
    public WorldObject SelectedObject { get; set; }

    public GameObject baseUnitPrefab;

	// Use this for initialization
	void Start () {
        ui = GetComponentInChildren<UI>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectObject(WorldObject worldObject) {
        if (SelectedObject) {
            SelectedObject.SetSelection(false, ui.GetPlayingArea());
        }
        if(worldObject is Ore && (worldObject as Ore).carrier) {
            SelectedObject = (worldObject as Ore).carrier;
        } else {
            SelectedObject = worldObject;
        }
        SelectedObject.SetSelection(true, ui.GetPlayingArea());
    }

    public void Deselect() {
        if (SelectedObject) {
            SelectedObject.SetSelection(false, ui.GetPlayingArea());
            SelectedObject = null;
        }
    }

    /**
     * Add a new unit to this player, spawning at spawnpoint
     */
    public void AddUnit(Vector3 spawnPoint, Quaternion rotation) {
        Units units = GetComponentInChildren<Units>();
        GameObject newUnit = (GameObject)Instantiate(baseUnitPrefab, spawnPoint, rotation);
        newUnit.transform.parent = units.transform; //add to this players units.
    }
}
