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
        if (cooldown > 0 || in_trigger) {
            return;
        }
        Portal entered = other.gameObject.GetComponent<Portal>();

        //teleport to the exit of the other portal
        //var diff = entered.transform.localToWorldMatrix * transform.localToWorldMatrix;
        //Vector3 new_pos = diff.GetColumn(3);
        //new_pos += entered.transform.position - entered.Sibling.transform.position;
        Quaternion diff = Quaternion.Euler(entered.Sibling.transform.localEulerAngles - entered.transform.localEulerAngles);
        Quaternion new_rot = Quaternion.Euler(transform.localEulerAngles + diff.eulerAngles);
        transform.SetPositionAndRotation(entered.Sibling.transform.position, new_rot);

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = diff * rb.velocity;
        }

        cooldown = cooldown_max;
        in_trigger = true;
    }

    void OnTriggerExit(Collider other) {
        in_trigger = false;
    }
}
