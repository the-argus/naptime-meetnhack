using System;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    // okay this thing stores references to like everything in the game
    // not fun but give me a break this is a jam okay
    public Portal Sibling;

    // sebastian understands this one but I don't really
    private MeshRenderer screen;
    
    // the linked cameras
    private Camera PlayerCam;
    private Camera SiblingCam;

    // render texture which contains the view of the sibling cam
    private RenderTexture viewTexture;

    // sebastian constants
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    void Awake() {
        // always gets the selected camera :)
        PlayerCam = Camera.main;

        screen = gameObject.GetComponent<MeshRenderer>();

        int target_viewer = 2;
        int target_sibling = 1;
        if (gameObject.transform.parent.gameObject.name == "Portal 2") {
            target_viewer = 3;
            target_sibling = 0;
        }

        SiblingCam = transform.root.GetChild(target_viewer).gameObject.GetComponent<Camera>();

        Sibling = transform.root.GetChild(target_sibling).GetChild(1).gameObject.GetComponent<Portal>();
    }


    // copied straight from sebby's video
    void CreateViewTexture() {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if (viewTexture != null) {
                // release hardware if the viewtexture dimensions dont match the screen
                viewTexture.Release();
            }

            // create new viewtexture, this one does match the screen
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            
            // assign the sibling camera to render to our viewtexture
            SiblingCam.targetTexture = viewTexture;
            
            // send the viewtexture to the portal material's shader code
            Sibling.screen.material.SetTexture("_MainTex", viewTexture);
            //Debug.Log("Created view texture");
        }
        //Debug.Log($"View texture is: {viewTexture is null}");
    }

    // http://wiki.unity3d.com/index.php/IsVisibleFrom
    public static bool VisibleFromCamera (Renderer renderer, Camera camera) {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (frustumPlanes, renderer.bounds);
    }

    public void Render() {
        //Debug.Log($"Main Camera: {PlayerCam}\nPortalViewer: {SiblingCam}");

        // stop render if this object is not visible
        if (!VisibleFromCamera(Sibling.screen, PlayerCam)) {
            //Debug.Log($"Skipped render of {this}");
            return;
        }

        // sebastian said to disable the mesh renderer? idk why
        screen.enabled = false;

        CreateViewTexture();

        // matrix of the relative position between this portal and the main camera
        // using var because i dont want to figure out the type
        var alignmentMatrix = transform.localToWorldMatrix * Sibling.transform.worldToLocalMatrix * PlayerCam.transform.localToWorldMatrix;

        // move the other camera to the desired alignment
        SiblingCam.transform.SetPositionAndRotation(alignmentMatrix.GetColumn(3), alignmentMatrix.rotation);

        // okay now the camera is in the right position and rendering to the right texture, now render
        SetNearClipPlane();
        SiblingCam.Render();


        screen.enabled = true;
    }

    // Sebastian lague's code literally copy pasted
    void SetNearClipPlane () {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = transform;
        int dot = System.Math.Sign (Vector3.Dot (clipPlane.forward, transform.position - SiblingCam.transform.position));

        Vector3 camSpacePos = SiblingCam.worldToCameraMatrix.MultiplyPoint (clipPlane.position);
        Vector3 camSpaceNormal = SiblingCam.worldToCameraMatrix.MultiplyVector (clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs (camSpaceDst) > nearClipLimit) {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            SiblingCam.projectionMatrix = PlayerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } else {
            SiblingCam.projectionMatrix = PlayerCam.projectionMatrix;
        }
    }
}