using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Unit {
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (!target) {
            LookForEnemies();
        }
	}

    /// <summary>
    /// Ugly hack
    /// </summary>
    private void LookForEnemies() {
        GetNewTarget(RTS.Order.ATTACK);
        if (target) {
            BeginAttack(target as Unit);
            CallForAid(target as Unit);
        }
    }
}
