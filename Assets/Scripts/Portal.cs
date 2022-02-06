using System;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    // okay this thing stores references to like everything in the game
    // not fun but give me a break this is a jam okay
    public Portal Sibling;

    // sebastian understands this one but I don't really
    public MeshRenderer screen;
    
    // the linked cameras
    private Camera PlayerCam;
    private Camera SiblingCam;

    // render texture which contains the view of the sibling cam
    private RenderTexture viewTexture;

    void Awake() {
        // always gets the selected camera :)
        PlayerCam = Camera.main;
        try {
            // hardcoded location of portal camera
            SiblingCam = (Camera)transform.root.Find("PortalViewer").gameObject.GetComponent("Camera");
        }
        catch (InvalidCastException) {
            Debug.LogError($"PortalViewer camera not found by {this}");
        }
    }


    // copied straight from sebby's video
    void CreateViewTexture() {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if (viewTexture != null) {
                // release hardware if the viewtexture dimensions dont match the screen
                viewTexture.Release ();
            }

            // create new viewtexture, this one does match the screen
            viewTexture = new RenderTexture (Screen.width, Screen.height, 0);
            
            // assign the sibling camera to render to our viewtexture
            SiblingCam.targetTexture = viewTexture;
            
            // send the viewtexture to the portal material's shader code
            Sibling.screen.material.SetTexture ("_MainTex", viewTexture);
        }
    }

    public void Render() {

        // stop render if this object is not visible
        if (!Sibling.GetComponent<Renderer>().isVisible) {
            return;
        }

        // sebastian said to disable the mesh rendered? idk why
        screen.enabled = false;

        CreateViewTexture();

        // matrix of the relative position between this portal and the main camera
        // using var because i dont want to figure out the type
        var alignmentMatrix = (PlayerCam.transform.localToWorldMatrix * transform.worldToLocalMatrix);
        // shift that matrix to be relative to the other portal instead
        alignmentMatrix *= Sibling.transform.localToWorldMatrix;

        // move the other camera to the desired alignment
        SiblingCam.transform.SetPositionAndRotation(alignmentMatrix.GetColumn(3), alignmentMatrix.rotation);

        // okay now the camera is in the right position and rendering to the right texture, now render
        SiblingCam.Render();

        screen.enabled = true;
    }

}