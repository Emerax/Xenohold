using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour {

    public bool menu = true;

    public string username;
    public bool human;
    public UI ui;
    public List<Unit> ownedUnits;
    public List<Unit> selectedUnits;

    public GameObject baseUnitPrefab;
    public GameObject redGolemPrefab, greenGolemPrefab, blueGolemPrefab;

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
        DeselectAll();
        selectedUnits = units;
        foreach (Unit unit in selectedUnits) {
            unit.SetSelection(true);
        }
    }

    public void DeselectAll() {
        if (selectedUnits.Count > 0) {
            foreach (Unit unit in selectedUnits) {
                unit.SetSelection(false);
            }
            selectedUnits.Clear();
        }
    }

    public void Deselect(Unit unit) {
        selectedUnits.Remove(unit);
    }

    /**
     * Add a new unit to this player, spawning at spawnpoint
     */
    public void AddUnit(GameObject prefab, Vector3 spawnPoint, Quaternion rotation) {
        Units units = GetComponentInChildren<Units>();
        GameObject newUnit = (GameObject)Instantiate(prefab, spawnPoint, rotation);
        newUnit.transform.parent = units.transform; //add to this players units.
        ownedUnits.Add(newUnit.GetComponentInChildren<Unit>());
    }
}
