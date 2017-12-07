using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Statue : WorldObject {

    public NavMeshObstacle obstacle;

    protected override void Start() {
        base.Start();
        obstacle = GetComponent<NavMeshObstacle>();
    }

}
