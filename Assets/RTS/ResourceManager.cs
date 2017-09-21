using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS {
    public static class ResourceManager {
        private static Vector3 invalidPosition = new Vector3(-9999, -9999, -9999);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }
    }
}
