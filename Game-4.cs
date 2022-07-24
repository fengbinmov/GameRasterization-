using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MOVCAT0
{

    #region StaticHelper
    public static class GHelper
    {
        public static Vector4 V4(this Vector3 v, float w = 1)
        {
            return new Vector4(v.x, v.y, v.z, w);
        }
        public static Vector4 V4(this Vector2 v, float z = 1, float w = 1)
        {
            return new Vector4(v.x, v.y, z, w);
        }
        public static Vector3 V3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }
    }
    #endregion

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

                //Matrix4x4 matrix = Matrix4x4.identity;
                Matrix4x4 matrix = Game.Opo_ ? Camera.opo : Matrix4x4.identity;

                Vector4 d = matrix * a.V4();
                Vector4 e = matrix * b.V4();
                Vector4 f = matrix * c.V4();

                a = d / d.w;
                b = e / e.w;
                c = f / f.w;

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

        [HideInInspector] public Matrix4x4 rotationMat;

        [HideInInspector] public static Matrix4x4 opo;

        public void OnValidate()
        {
            MakeRotaionMat();

            float n = -zNear;
            float f = -zFar;

            float t = Mathf.Tan(fov / 360f * Mathf.PI) * Mathf.Abs(n);
            float b = -t;
            float r = t * aspect;
            float l = -r;

            opo = Matrix4x4.identity;
            opo.SetRow(0, new Vector4(n, 0, 0, 0));
            opo.SetRow(1, new Vector4(0, n, 0, 0));
            opo.SetRow(2, new Vector4(0, 0, n + f, -n * f));
            opo.SetRow(3, new Vector4(0, 0, 1, 0));
        }
        public Vector3 Rotation(Vector3 p)
        {

            return rotationMat * p.V4();
        }
        public void MakeRotaionMat()
        {
            float x = rotation.x / 180f * Mathf.PI;
            float y = rotation.y / 180f * Mathf.PI;
            float z = rotation.z / 180f * Mathf.PI;

            Matrix4x4 rotX = new Matrix4x4();
            rotX.SetRow(0, new Vector4(1, 0, 0, 0));
            rotX.SetRow(1, new Vector4(0, Mathf.Cos(x), -Mathf.Sin(x), 0));
            rotX.SetRow(2, new Vector4(0, Mathf.Sin(x), Mathf.Cos(x), 0));
            rotX.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 rotY = new Matrix4x4();
            rotY.SetRow(0, new Vector4(Mathf.Cos(y), 0, Mathf.Sin(y), 0));
            rotY.SetRow(1, new Vector4(0, 1, 0, 0));
            rotY.SetRow(2, new Vector4(-Mathf.Sin(y), 0, Mathf.Cos(y), 0));
            rotY.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 rotZ = new Matrix4x4();
            rotZ.SetRow(0, new Vector4(Mathf.Cos(z), -Mathf.Sin(z), 0, 0));
            rotZ.SetRow(1, new Vector4(Mathf.Sin(z), Mathf.Cos(z), 0, 0));
            rotZ.SetRow(2, new Vector4(0, 0, 1, 0));
            rotZ.SetRow(3, new Vector4(0, 0, 0, 1));

            rotationMat = rotY * rotX * rotZ;
        }
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

            Vector3 n1 = position - Rotation(new Vector3(l, b, zNear));
            Vector3 n2 = position - Rotation(new Vector3(r, b, zNear));
            Vector3 n3 = position - Rotation(new Vector3(r, t, zNear));
            Vector3 n4 = position - Rotation(new Vector3(l, t, zNear));
            Vector3 f1 = position - Rotation(new Vector3(l2, b2, zFar));
            Vector3 f2 = position - Rotation(new Vector3(r2, b2, zFar));
            Vector3 f3 = position - Rotation(new Vector3(r2, t2, zFar));
            Vector3 f4 = position - Rotation(new Vector3(l2, t2, zFar));

            if (Game.Opo_) {
                f1 = new Vector3(n1.x, n1.y, f1.z);
                f2 = new Vector3(n2.x, n2.y, f2.z);
                f3 = new Vector3(n3.x, n3.y, f3.z);
                f4 = new Vector3(n4.x, n4.y, f4.z);
            }
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

            Gizmos.DrawLine(position, n2);
            Gizmos.DrawLine(position, n3);
            Gizmos.DrawLine(position, n4);
            Gizmos.DrawLine(position, n1);

            Gizmos.DrawLine(n2, f2);
            Gizmos.DrawLine(n3, f3);
            Gizmos.DrawLine(n4, f4);
            Gizmos.DrawLine(n1, f1);
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


        private void OnValidate()
        {

            camera.OnValidate();
        }
        private void OnDrawGizmos()
        {
            Opo_ = Opo;
            camera.DrawGizmos();
            primitive.DrawGizmos();
        }

        public float z = -4;
        public float zSalce = 2;
        public bool Opo = false;
        public static bool Opo_ = false;
        [ContextMenu("sdfdf")]
        void sdfdf() {

            List<Vector3> vertexs = new List<Vector3>();
            List<int> triangles = new List<int>();

            vertexs.Add(new Vector3(0, 0, z));

            for (int i = 0; i < 52; i++)
            {
                float v = (float)i / 52f * Mathf.PI * 2;

                Vector3 p = new Vector3(Mathf.Cos(v) * zSalce, Mathf.Sin(v) * zSalce, z);

                vertexs.Add(p);
            }
            for (int b = 1; b < vertexs.Count; b++)
            {
                int c = (b + 1) == vertexs.Count ? 1 : (b + 1);

                triangles.Add(0);
                triangles.Add(b);
                triangles.Add(c);
            }

            primitive.mMeshes[0] = new MMesh() { 
                vectors = vertexs.ToArray(),
                triangles = triangles.ToArray()
            };
        }
    }

}