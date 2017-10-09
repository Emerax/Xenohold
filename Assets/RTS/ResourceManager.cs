using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS {
    public static class ResourceManager {
        private static Vector3 invalidPosition = new Vector3(-9999, -9999, -9999);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }

        private static GUISkin selectBoxSkin;
        public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }

        public static GameObject[] unitList;

        public static void StoreSelectBoxItems(GUISkin skin) {
            selectBoxSkin = skin;
        }

        private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
        public static Bounds InvalidBounds { get { return invalidBounds; } }
    }
}
