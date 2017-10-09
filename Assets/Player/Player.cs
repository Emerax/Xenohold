using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour {

    public string username;
    public bool human;
    public UI ui;
    public WorldObject SelectedObject { get; set; }

    public GameObject[] unitList; //Units creatable for this player

	// Use this for initialization
	void Start () {
        ui = GetComponentInChildren<UI>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Add a new unit to this player, spawning at spawnpoint
     */
    public void AddUnit(string unitName, Vector3 spawnPoint, Quaternion rotation) {
        Units units = GetComponentInChildren<Units>();
        GameObject newUnit = (GameObject)Instantiate(GetUnit(unitName), spawnPoint, rotation);
        newUnit.transform.parent = units.transform; //add to this players units.
    }

    private GameObject GetUnit(string unitName) {
        for (int i = 0; i < unitList.Length; i++) {
            Unit unit = unitList[i].GetComponent<Unit>();
            if(unit && unit.name == unitName) {
                return unitList[i];
            }
        }
        return null;
    }
}
