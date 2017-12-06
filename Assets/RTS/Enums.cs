using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS {
    public enum CursorState {Idle, Select, Move, Attack, PanLeft, PanRight, PanUp, PanDown, PickUp, PutDown}
    public enum OreType {Green, Red, Purple, Blue}
    public enum Order {MOVE, ATTACK, PICK_UP, NONE}
}