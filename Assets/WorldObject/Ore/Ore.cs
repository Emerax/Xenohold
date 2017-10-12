using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Ore : WorldObject {

    public OreType type;
    public Unit carrier { get; set; }

	// Use this for initialization
	protected override void Start () {
        base.Start();
        CalculateBounds();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        if (carrier) {
            CalculateBounds();
        }
	}

    public void OnPickUp(Unit pickerUper) {
        this.carrier = pickerUper;
    }

    public void OnDrop() {
        carrier = null;
    }
}
