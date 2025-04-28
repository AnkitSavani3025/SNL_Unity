using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP {
    public class SNL_3PLUS_MGP_Snake : MonoBehaviour {
        public List<Transform> wayPoints = new List<Transform>();
        public float time;
        public Image blink;
    }
}