using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour {

    public string username;
    public bool human;
    public UI ui;
    public List<Unit> ownedUnits;
    public List<Unit> selectedUnits;

    public GameObject baseUnitPrefab;

    void Awake() {
        ui = GetComponentInChildren<UI>();
        //Add all editor-placed units to the list of owned ones.
        foreach(Unit unit in GetComponentsInChildren<Unit>()) { ownedUnits.Add(unit); }
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectUnits(List<Unit> units) {
        if (selectedUnits.Count > 0) {
            foreach (Unit unit in selectedUnits) {
                unit.SetSelection(false, ui.GetPlayingArea());
            }
            selectedUnits.Clear();
        } else {
            selectedUnits = units;
            foreach (Unit unit in selectedUnits) {
                unit.SetSelection(true, ui.GetPlayingArea());
            }
        }
    }

    public void Deselect() {
        if (selectedUnits.Count > 0) {
            foreach (Unit unit in selectedUnits) {
                unit.SetSelection(false, ui.GetPlayingArea());
            }
            selectedUnits.Clear();
        }
    }

    /**
     * Add a new unit to this player, spawning at spawnpoint
     */
    public void AddUnit(Vector3 spawnPoint, Quaternion rotation) {
        Units units = GetComponentInChildren<Units>();
        GameObject newUnit = (GameObject)Instantiate(baseUnitPrefab, spawnPoint, rotation);
        newUnit.transform.parent = units.transform; //add to this players units.
        ownedUnits.Add(newUnit.GetComponentInChildren<Unit>());
    }
}
