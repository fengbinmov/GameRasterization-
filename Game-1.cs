using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MOVCAT0
{
    #region BaseClass
    public class Graphics
    {
        public virtual void DrawGizmos()
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
            Gizmos.matrix = matrix;
            Gizmos.color = Color.white;
        }
    }
    public class Screen
    {

        public Vector2Int size = new Vector2Int(256, 256);              //ÆÁÄ»·Ö±æÂÊ   
        protected Color[,] frameBuffer;
        protected float[,] zBuffer;
        protected Vector3[,] normalBuffer;
        public Texture2D texture2D;
        public RawImage rawImage;
    }
    #endregion

    #region Variable
    public class Triangle
    {
        public Vector3[] vectors = new Vector3[3];
        public Color[] colors = new Color[3];
        public Vector3[] normals = new Vector3[3];
    }
    [Serializable]
    public class MMesh : Graphics
    {
        public Vector3[] vectors;
        public int[] triangles;

        public override void DrawGizmos()
        {
            base.DrawGizmos();

            for (int i = 0; i < triangles.Length - 2; i += 3)
            {
                Vector3 a = vectors[triangles[i]];
                Vector3 b = vectors[triangles[i + 1]];
                Vector3 c = vectors[triangles[i + 2]];

                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, a);
            }
        }

        public int TriangleCount { get { return (int)(triangles.Length / 3); } }

    }
    #endregion

    [Serializable]
    public class Primitive : Graphics
    {
        public List<MMesh> mMeshes;
        public override void DrawGizmos()
        {
            base.DrawGizmos();

            foreach (var mesh in mMeshes)
            {
                mesh.DrawGizmos();
            }
        }
    }

    [Serializable]
    public class Camera : Graphics
    {
        public Vector3 position;
        public Vector3 rotation;
        public override void DrawGizmos()
        {

            base.DrawGizmos();

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + Vector3.right);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + Vector3.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + Vector3.forward);
        }
    }

    [Serializable]
    public class Rasterization : Screen
    {
    }
    public class Game : MonoBehaviour
    {
        public Primitive primitive = new Primitive();
        public Camera camera = new Camera();


        private void OnDrawGizmos()
        {
            camera.DrawGizmos();
            primitive.DrawGizmos();
        }
    }

}