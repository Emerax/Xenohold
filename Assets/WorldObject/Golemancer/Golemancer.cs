using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Golemancer : Unit {

    protected override void Update() {
        base.Update();
        if(currentOrder == Order.PUT_DOWN) {
            if (carrying && target) {
                if (DistanceToTarget(target) <= pickUpDistance) {
                    Infuse(target as Statue);
                    ClearOrder();
                }
            } else {
                //Stop trying to infuse if no longer holding ore
                ClearOrder();
            }
        }
    }

    protected override void PickUp(Ore ore) {
        if (carrying) {
            Drop();
        }

        Vector3 attachPos = transform.Find("StaffTop").position;
        ore.transform.position = attachPos;
        ore.transform.parent = transform;

        ore.OnPickUp(this);
        carrying = true;
    }

    public override void SetHoverState(WorldObject worldObject) {
        base.SetHoverState(worldObject);
        if(worldObject is Statue && carrying) {
            player.ui.SetCursorState(RTS.CursorState.PutDown);
        }
    }

    public override void RightClickObject(WorldObject worldObject) {
        base.RightClickObject(worldObject);
        if(worldObject is Statue) {
            BeginInfuse(worldObject as Statue);
        }
    }

    protected void Infuse(Statue statue) {
        Ore ore = transform.GetComponentInChildren<Ore>();
        //Use ore color to determine what unit is created
        player.AddUnit(statue.transform.position, statue.transform.rotation);
        //Immediately after creating the new unit, drop ore, destroy it, and lastly destroy the statue.
        Drop();
        Destroy(ore.gameObject);
        Destroy(statue.gameObject);
    }

    private void BeginInfuse(Statue statue) {
        currentOrder = Order.PUT_DOWN;
        target = statue;
        agent.SetDestination(target.transform.position);
    }

}
