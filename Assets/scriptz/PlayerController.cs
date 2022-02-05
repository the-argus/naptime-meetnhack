using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;


    public float ogSpeed;
    public float jumpForce;


    private float xInput;
    private float zInput;
    private Transform cameraTrans;
    


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        cameraTrans = GameObject.Find("Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {


        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            print("got jump");

            rb.AddForce(0, jumpForce, 0);
        }



    }

    bh vgcxc


    private void FixedUpdate()
    {
        float runSpeed = ogSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            runSpeed = runSpeed * 2;
        }

        rb.velocity = new Vector3(xInput * runSpeed, rb.velocity.y, zInput*ogSpeed);
    }

    private void Jump()
    {
        
    }




}
