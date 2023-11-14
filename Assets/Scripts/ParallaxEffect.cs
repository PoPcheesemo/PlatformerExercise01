using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera Cam;
    public Transform followTarget;

    //Starting Position for the parallax game object
    Vector2 startingPosition;

    //Start Z value of the parallax gameobject
    float startingZ;

    // Distance that the camera has moved from the starting position of the parallax object
    Vector2 camMoveSinceStart => (Vector2)Cam.transform.position - startingPosition;

    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    float clippingPlane => (Cam.transform.position.z + (zDistanceFromTarget > 0 ? Cam.farClipPlane :  Cam.nearClipPlane));    
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // When the target moves, move the parallax object the same distance * a multiplier
        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;

        // The X/Y position changes based on target travel speed * the parallax factor, but Z statrys consistent
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
