using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopScript : MonoBehaviour {
    //References
    public GameObject pointIndicator;
    public GameObject CraftInstance;
    public Texture2D CraftTexture;

    private List<Transform> indicators = new List<Transform>();
    private LineRenderer LR;

    private List<Vector2> points = new List<Vector2>();

    private List<GameObject> blocks = new List<GameObject>();

    public void Start() {
        LR = GetComponent<LineRenderer>();
    }

    public void Update() {
        RecieveInput();
        Draw();
    }

    private void RecieveInput() {
        if (Input.GetMouseButtonDown(0)) {
            if (!GrabBlock(GetMousePos()))
                points.Add(GetMousePos());
        }

        if (Input.GetMouseButtonDown(1)) {
            int closestPoint = GetClosetPoint(GetMousePos());
            if (!RemoveBlock(GetMousePos()) && closestPoint != -1)
                points.RemoveAt(closestPoint);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            AssembleCraft();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            points.Clear();
            var blockNum = blocks.Count;
            for (int i = 0; i < blockNum; i++) {
                var block = blocks[0];
                blocks.RemoveAt(0);
                Destroy(block);
                Camera.main.orthographicSize = 15;
                Camera.main.transform.position = new Vector3(0,15,-10);
            }
        }

        bool GrabBlock(Vector2 pos) {
            var col = Physics2D.OverlapCircle(pos, 0);
            var block = col?.GetComponent<Block>();
            if (block)
                block.isHeld = true;
            return block!=null;
        }

        bool RemoveBlock(Vector2 pos) {
            var col = Physics2D.OverlapCircle(pos, 0);
            var block = col?.GetComponent<Block>();
            if(!block)
                return false;
            blocks.Remove(block.gameObject);
            Destroy(block.gameObject);
            return true;
        }

        int GetClosetPoint(Vector2 Pos) {
            int point = -1;
            float dist = 0;
            if (points.Count > 0) {
                point = 0;
                dist = GetDist(point);
            }
            for (int i = 1; i < points.Count; i++) {
                var d = GetDist(i);
                if (dist > d) {
                    dist = d;
                    point = i;
                }
            }
            return point;

            float GetDist(int pnt) {
                return (points[pnt] - Pos).magnitude;
            }
        }

        Vector2 GetMousePos() {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void Draw() {
        //Draw Dots
        int dif = points.Count - indicators.Count;
        for (int i = 0; i < dif; i++)
            indicators.Add(Instantiate(pointIndicator).transform);

        for (int i = 0; i < -dif; i++) {
            var ind = indicators[0];
            indicators.RemoveAt(0);
            Destroy(ind.gameObject);
        }
        for (int i = 0; i < indicators.Count; i++)
            indicators[i].position = points[i];

        //Draw Lines
        LR.positionCount = (points.Count > 1) ? points.Count+1 : points.Count;
        for (int i = 0; i < points.Count; i++)
            LR.SetPosition(i, points[i]);
        if (points.Count > 1)
            LR.SetPosition(points.Count, points[0]);         
    }

    private void AssembleCraft() {
        if (points.Count < 3)
            return;

        var sprite = Sprite.Create(CraftTexture, new Rect(0,0,CraftTexture.width,CraftTexture.height), new Vector2(0.5f, 0.5f));
        Vector4 minMaxCoords = GetMinMaxCoords();
        sprite.OverrideGeometry(ScalePoints().ToArray(), Triangulation.Triangulate(points).ToArray());

        var scaleFactor = GetScaleFactor();
        Vector2 craftPos = new Vector2(minMaxCoords.x+sprite.bounds.size.x*scaleFactor/2, minMaxCoords.y+sprite.bounds.size.y*scaleFactor/2);

        var craft = Instantiate(CraftInstance, craftPos, Quaternion.identity);
        craft.GetComponent<SpriteRenderer>().sprite = sprite;
        craft.GetComponent<SpriteRenderer>().color = Random.ColorHSV() / 2 + new Color(0.5f, 0.5f, 0.5f);
        craft.GetComponent<PolygonCollider2D>().SetPath(0, ScaleColPoints());
        craft.transform.localScale = new Vector2(scaleFactor, scaleFactor);

        points.Clear();
        blocks.Add(craft);

        Vector4 GetMinMaxCoords() {
            //Get min and max world coords
            float minX = sprite.rect.width;
            float maxX = 0;
            float minY = sprite.rect.height;
            float maxY = 0;
            foreach (var point in points) {
                if (point.x < minX)
                    minX = point.x;
                if (point.y < minY)
                    minY = point.y;
                if (point.x > maxX)
                    maxX = point.x;
                if (point.y > maxY)
                    maxY = point.y;
            }
            return new Vector4(minX, minY, maxX, maxY);
        }

        List<Vector2> ScalePoints() {
            //Scale points to sprite space
            List<Vector2> scaledPoints = new List<Vector2>();
            float scale = Mathf.Min(sprite.rect.width,sprite.rect.height)/Mathf.Max((minMaxCoords.z-minMaxCoords.x), (minMaxCoords.w-minMaxCoords.y))*0.9999f;
            for (int i = 0; i < points.Count; i++)
                scaledPoints.Add(new Vector2((points[i].x - minMaxCoords.x)*scale, (points[i].y - minMaxCoords.y)*scale));
            return scaledPoints;
        }
        float GetScaleFactor() {
            float mySize = Mathf.Max((minMaxCoords.z-minMaxCoords.x), (minMaxCoords.w-minMaxCoords.y));
            float myPreferredSize = Mathf.Max(sprite.rect.width, sprite.rect.height) / sprite.pixelsPerUnit;
            return mySize/myPreferredSize;
        }
        List<Vector2> ScaleColPoints() {
            List<Vector2> scaledPoints = new List<Vector2>();
            points.ForEach(x => scaledPoints.Add((x-craftPos)/scaleFactor));
            return scaledPoints;
        }
    }

    private void Build() {
        var craft = Instantiate(CraftInstance);
        craft.transform.position = Vector2.zero;

        var sprite = Sprite.Create(CraftTexture, new Rect(0,0,CraftTexture.width,CraftTexture.height), new Vector2(0.5f, 0.5f));
        float scaleFactor = 1;
        List<Vector2> colliderPoints = null;
        var scaledPoints = ScaleToSprite(points, sprite, ref scaleFactor, ref colliderPoints);
        sprite.OverrideGeometry(scaledPoints.ToArray(), Triangulation.Triangulate(points).ToArray());
        craft.GetComponent<SpriteRenderer>().sprite = sprite;
        craft.GetComponent<SpriteRenderer>().color = Random.ColorHSV() / 2 + new Color(0.5f, 0.5f, 0.5f);
        craft.GetComponent<PolygonCollider2D>().SetPath(0, colliderPoints);
        craft.transform.localScale = new Vector2(scaleFactor, scaleFactor);

        //points.Clear();

        List<Vector2> ScaleToSprite(List<Vector2> points, Sprite sprite, ref float scaleFactor, ref List<Vector2> colliderPoints) {
            float minX = sprite.rect.width;
            float maxX = 0;
            float minY = sprite.rect.height;
            float maxY = 0;
            foreach (var point in points) {
                if (point.x < minX)
                    minX = point.x;
                if (point.y < minY)
                    minY = point.y;
                if (point.x > maxX)
                    maxX = point.x;
                if (point.y > maxY)
                    maxY = point.y;
            }

            scaleFactor = GetScale();

            //Scale and return points to collider space
            colliderPoints = new List<Vector2>();
            foreach (var point in points) {
                colliderPoints.Add(point/scaleFactor);
            }

            //Scale and return points to sprite space
            List<Vector2> scaledPoints = new List<Vector2>();
            float scale = Mathf.Min(sprite.rect.width,sprite.rect.height)/Mathf.Max((maxX-minX), (maxY-minY))*0.9999f;
            for (int i = 0; i < points.Count; i++)
                scaledPoints.Add(new Vector2((points[i].x - minX)*scale, (points[i].y - minY)*scale));
            return scaledPoints;

            float GetScale() {
                float mySize = Mathf.Max((maxX-minX), (maxY-minY));
                float myPreferredSize = Mathf.Max(sprite.rect.width, sprite.rect.height) / sprite.pixelsPerUnit;
                return mySize/myPreferredSize;
            }
        }
    }
}