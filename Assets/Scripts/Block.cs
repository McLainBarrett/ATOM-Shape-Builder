using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    [HideInInspector]
    public bool isHeld;
    Rigidbody2D RB;
    public float d1 = 10;
    public float d2 = 10;
    Vector2? offset = null;

    private void Start() {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (Input.GetMouseButtonUp(0))
            isHeld = false;

        if (!offset.HasValue && isHeld)
            offset = (Vector2)transform.position-GetMousePos();
        else if (offset.HasValue && !isHeld)
            offset = null;
        
        if (isHeld) {
            var mp = GetMousePos();
            var dir = (mp-(Vector2)transform.position+offset.Value)*d1-RB.velocity*d2;
            RB.AddForce(dir*Time.deltaTime*200);
        }
    }

    Vector2 GetMousePos() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}