using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyEdit : MonoBehaviour {
    //References
    private LineRenderer LR;
    public GameObject Point;
    public GameObject Line;

    List<Transform> Points = new List<Transform>();
    List<Transform> Lines = new List<Transform>();

    List<Vector2> Vertexes = new List<Vector2>();

    Transform myLine;

    public void Start() {
        LR = GetComponent<LineRenderer>();
        myLine = Instantiate(Line, transform).transform;
    }

    private void Update() {
        GetInput();
        Draw();
    }

    private void GetInput() {
        if (Input.GetMouseButtonDown(0))
            Vertexes.Add(GetMousePos());
        if (Input.GetMouseButtonDown(1) && Vertexes.Count > 0)
            Vertexes.RemoveAt(0);


        if (Vertexes.Count > 1)
            DrawLine(Vertexes[0], Vertexes[1], 0.1f, myLine);

        Vector2 GetMousePos() {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void Draw() {
        //Draw Points
        int dif = Vertexes.Count - Points.Count;
        for (int i = 0; i < dif; i++)
            Points.Add(Instantiate(Point, transform).transform);
        for (int i = 0; i < -dif; i++) {
            var ind = Points[0];
            Points.RemoveAt(0);
            Destroy(ind.gameObject);
        }

        for (int i = 0; i < Points.Count; i++)
            Points[i].position = Vertexes[i];

        



        /*
        //Draw Points
        int dif = Vertexes.Count - Points.Count;
        for (int i = 0; i < dif; i++)
            Points.Add(Instantiate(Point).transform);

        for (int i = 0; i < -dif; i++) {
            var ind = Points[0];
            Points.RemoveAt(0);
            Destroy(ind.gameObject);
        }
        for (int i = 0; i < Points.Count; i++)
            Points[i].position = Vertexes[i];

        //Draw Lines
        LR.positionCount = (Vertexes.Count > 1) ? Vertexes.Count+1 : Vertexes.Count;
        for (int i = 0; i < Vertexes.Count; i++)
            LR.SetPosition(i, Vertexes[i]);
        if (Vertexes.Count > 1)
            LR.SetPosition(Vertexes.Count, Vertexes[0]); 
        */       

    }

    private void DrawLine(Vector2 a, Vector2 b, float width, Transform line) {
        line.position = (a+b)/2;
        line.localScale = new Vector2(Vector2.Distance(a,b)*15/Camera.main.orthographicSize/2, width);
        line.rotation = Quaternion.Euler(0,0,-Vector2.SignedAngle((b-a), Vector2.left));
    }

}