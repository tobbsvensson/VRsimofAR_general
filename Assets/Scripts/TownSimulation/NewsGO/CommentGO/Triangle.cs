using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TownSimulation.NewsGO.CommentGO
{
    /// <summary>
    /// This creates a triangle for the "comics" bubble form.    
    /// </summary>
    /// <remarks>Attach to : Resources/Prefabs/Comment/Triangle/</remarks>
    [RequireComponent(typeof(MeshFilter))]
    public class Triangle : MonoBehaviour
    {

        Mesh mesh;

        Vector3[] vertices;
        int[] triangles;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        // Use this for initialization
        void Start()
        {
            MakeMeshData();
            CreateMesh();
        }

        private void MakeMeshData()
        {
            vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0) };
            triangles = new int[] { 0, 1, 2 };
        }
        private void CreateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
    }
}