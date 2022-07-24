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
        public float zNear;
        public float zFar;
        public float fov;
        [Range(0, 2f)]
        public float aspect = 1;

        public override void DrawGizmos()
        {

            base.DrawGizmos();

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + Vector3.right);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + Vector3.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + Vector3.forward);

            float t = Mathf.Tan(fov / 360f * Mathf.PI) * Mathf.Abs(zNear);
            float b = -t;
            float r = t * aspect;
            float l = -r;

            float t2 = Mathf.Tan(fov / 360f * Mathf.PI) * Mathf.Abs(zFar);
            float b2 = -t2;
            float r2 = t2 * aspect;
            float l2 = -r2;

            Vector3 n1 = position - new Vector3(l, b, zNear);
            Vector3 n2 = position - new Vector3(r, b, zNear);
            Vector3 n3 = position - new Vector3(r, t, zNear);
            Vector3 n4 = position - new Vector3(l, t, zNear);
            Vector3 f1 = position - new Vector3(l2, b2, zFar);
            Vector3 f2 = position - new Vector3(r2, b2, zFar);
            Vector3 f3 = position - new Vector3(r2, t2, zFar);
            Vector3 f4 = position - new Vector3(l2, t2, zFar);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(n1, n2);
            Gizmos.DrawLine(n2, n3);
            Gizmos.DrawLine(n3, n4);
            Gizmos.DrawLine(n4, n1);

            Gizmos.DrawLine(f1, f2);
            Gizmos.DrawLine(f2, f3);
            Gizmos.DrawLine(f3, f4);
            Gizmos.DrawLine(f4, f1);

            Gizmos.color = Color.gray;

            Gizmos.DrawLine(position, f2);
            Gizmos.DrawLine(position, f3);
            Gizmos.DrawLine(position, f4);
            Gizmos.DrawLine(position, f1);
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