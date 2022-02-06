using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Portal[] portals;
    void Awake() {
        portals = FindObjectsOfType<Portal>();
    }

    void OnPreCull() {
        foreach (Portal p in portals) {
            p.Render();
        }
    }

    // debug movement
    float xin;
    float zin;
    int rotating;

    void Update() {
        xin = Input.GetAxisRaw("Horizontal");
        zin = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Q)) {
            rotating = -1;
        } else if (Input.GetKey(KeyCode.E)) {
            rotating = 1;
        } else {
            rotating = 0;
        }

        transform.Rotate(new Vector3(0, rotating, 0));
        transform.Translate(0.1f*xin, 0, 0.1f*zin);
    }
}
