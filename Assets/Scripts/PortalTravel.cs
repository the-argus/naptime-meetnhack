using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTravel : MonoBehaviour
{
    private int cooldown;
    private int cooldown_max;
    private bool in_trigger;

    void Start() {
        cooldown_max = 15;
        cooldown = 0;
    }
    void Update() {
        if (cooldown > 0) {
            cooldown --;
        }
    }
    void OnTriggerEnter(Collider other) {
        Debug.Log("entered");
        if (cooldown > 0 || in_trigger) {
            return;
        }
        Portal entered = other.gameObject.GetComponent<Portal>();

        //teleport to the exit of the other portal
        var diff = entered.transform.localToWorldMatrix * transform.localToWorldMatrix;
        Vector3 new_pos = diff.GetColumn(3);
        new_pos += entered.Sibling.transform.position - entered.transform.position;

        transform.SetPositionAndRotation(new_pos, transform.rotation);

        cooldown = cooldown_max;
        in_trigger = true;
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("exited");
        in_trigger = false;
    }
}
