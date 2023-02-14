using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedTriangulation : MonoBehaviour {

    private class Trapezoid {
        Vector2 High;
        Vector2 Low;
        Trapezoid U1, U2, D1, D2, U3;
        bool U3SIDE;
        Edge LSEG, RSEG;
        int SINK;
        bool STATE;
    }

    private class Edge {
        public Vector2 a;
        public Vector2 b;
        public Vector2 hVert {
            get {return (a.y > b.y) ? a : b;}
        }
    }

    private class PTNode {
        public string NodeType = "region";//trap, edge, vert
        public Vector2 Vert;
        public Edge Edge;
        public Trapezoid Trap;
        public PTNode Left, Right, Parent;
        

        public void Initilize(Vector2 pos) {
            Left = new PTNode();
            Right = new PTNode();
            NodeType = "vert";
            Vert = pos;
        }
    }

    

    public void Trapezoidate() {
        List<Edge> edges = new List<Edge>();
        PTNode tree;

        //1.
        var edge = edges[0];
        //2.
        var highVert = edge.hVert;
        //3.
        tree = new PTNode();
        tree.Initilize(highVert);





        /*
        Traverse the tree to find correct region in which higher vertex will be added
            If vertex is not already added, change the leaf node (trapezoid) into a vertex node. 
            This node will have two trapezoids as its left and right children. 
            Left child represents region below the vertex whereas right child represents region above the vertex. 
            Right child trapezoid is old region to be split by the vertex. Left child trapezoid is new region introduced after addition of vertex. 
            Update the trapezoid structure.
            If vertex is already added (as part of previously added edge), just traverse the tree to find the region in which the vertex reside. 
            This is required for addition of an edge.

        4. Add lower vertex to the trapezoidation and follow the same procedure as for higher vertex to traverse the tree and find correct region.

        5. Finally, add the edge starting from higher vertex to lower vertex. 
        This edge will split all the regions which are below higher vertex and above lower vertex. 
        Update the trapezoid structure, for regions split by the edge.
        */
    }

    private void Traverse(PTNode tree, Edge edge) {

    }
}

/*
1. Select an edge from randomly generated sequence of edges

2. Find higher vertex of that edge

3. Traverse the tree to find correct region in which higher vertex will be added
    If vertex is not already added, change the leaf node (trapezoid) into a vertex node. 
    This node will have two trapezoids as its left and right children. 
    Left child represents region below the vertex whereas right child represents region above the vertex. 
    Right child trapezoid is old region to be split by the vertex. Left child trapezoid is new region introduced after addition of vertex. 
    Update the trapezoid structure.
    If vertex is already added (as part of previously added edge), just traverse the tree to find the region in which the vertex reside. 
    This is required for addition of an edge.

4. Add lower vertex to the trapezoidation and follow the same procedure as for higher vertex to traverse the tree and find correct region.

5. Finally, add the edge starting from higher vertex to lower vertex. 
    This edge will split all the regions which are below higher vertex and above lower vertex. 
    Update the trapezoid structure, for regions split by the edge.
*/