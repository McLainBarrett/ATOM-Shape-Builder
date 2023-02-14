using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        var size = new Vector2(cam.orthographicSize*cam.aspect, cam.orthographicSize);
        if (Input.GetMouseButton(2))
            transform.position -= new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*cam.orthographicSize/15;
        var offset = transform.position.y-size.y;
        if (offset < 0)
            transform.position -= new Vector3(0,offset);


        cam.orthographicSize -= Input.mouseScrollDelta.y;
        if (cam.orthographicSize < 3 )
            cam.orthographicSize = 3;
        if (100 < cam.orthographicSize)
            cam.orthographicSize = 100;
    }
}