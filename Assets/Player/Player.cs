using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public string username;
    public bool human;
    public UI ui;
    public WorldObject SelectedObject { get; set; }

	// Use this for initialization
	void Start () {
        ui = GetComponentInChildren<UI>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
