using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class gridMesh : MonoBehaviour
{
    public GameObject prefabQuad;
    Grid grid;
    public int width;
    public int height;
    bool gridloaded = false;

    public GameObject gridParent;
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;
    Mesh mesh;
    public float cellsize;
    Camera cam;
    float[] m_points;
    int m_hitCount = 0;
    public Material mat;
    float[,] totalWeight;
    float WRed;
    Vector3 previousPoint = new Vector3(0f, 0f, 0f);

    public bool Gridloaded { get => gridloaded; set => gridloaded = value; }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        totalWeight = new float[width, height];
        WRed = 0;
        vertices = new Vector3[4 * (width * height)];
        uvs = new Vector2[4 * (width * height)];
        triangles = new int[6 * (width * height)];
        mesh = new Mesh();
        gridParent.GetComponent<MeshFilter>().mesh = mesh;
        m_points = new float[3 * 32];

        Vector3[] tmpVertices = new Vector3[4];
        Vector2[] tmpuv = new Vector2[4];
        int[] tmpTriangles = new int[6];

        // First check if it is not already present



        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject subM = Instantiate(prefabQuad);

                subM.name = "tile" + i + "," + j;
                Mesh tmpMesh = new Mesh();
                int index = i * height + j;
                vertices[index * 4 + 0] = new Vector3((cellsize * (i)), (cellsize * (j)));
                vertices[index * 4 + 1] = new Vector3((cellsize * (i)), (cellsize * (j + 1)));
                vertices[index * 4 + 2] = new Vector3((cellsize * (i + 1)), (cellsize * (j + 1)));
                vertices[index * 4 + 3] = new Vector3((cellsize * (i + 1)), (cellsize * (j)));

                uvs[index * 4 + 0] = new Vector2(0, 0);
                uvs[index * 4 + 1] = new Vector2(0, 1);
                uvs[index * 4 + 2] = new Vector2(1, 1);
                uvs[index * 4 + 3] = new Vector2(1, 0);

                triangles[index * 6 + 0] = index * 4 + 0;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;
                triangles[index * 6 + 3] = index * 4 + 0;
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;

                tmpVertices[0] = vertices[index * 4 + 0];
                tmpVertices[1] = vertices[index * 4 + 1];
                tmpVertices[2] = vertices[index * 4 + 2];
                tmpVertices[3] = vertices[index * 4 + 3];

                tmpuv[0] = uvs[index * 4 + 0];
                tmpuv[1] = uvs[index * 4 + 1];
                tmpuv[2] = uvs[index * 4 + 2];
                tmpuv[3] = uvs[index * 4 + 3];

                tmpTriangles[0] = 0;
                tmpTriangles[1] = 1;
                tmpTriangles[2] = 2;
                tmpTriangles[3] = 0;
                tmpTriangles[4] = 2;
                tmpTriangles[5] = 3;



                subM.transform.position = new Vector3(subM.transform.position.x + cellsize * i, subM.transform.position.y + cellsize * j, subM.transform.position.z);

                subM.transform.GetComponent<MeshRenderer>().material = mat;
                subM.layer = 13;
                subM.transform.parent = gridParent.transform;


            }

        }
        if (gridParent.transform.name == "HighResHeat1")
        {
            gridParent.transform.position = new Vector3(-7.138929f, -2.907556f, 1.531785f);
            gridParent.transform.rotation = Quaternion.Euler(-0.398f, -4.811f, -0.409f);
        }
        else if (gridParent.transform.name == "HighResHeat2")
        {
            gridParent.transform.position = new Vector3(6.11f, -2.90856f, 5.138142f);
            gridParent.transform.rotation = Quaternion.Euler(-1.214f, 85.096f, 0.104f);
        }
        else if (gridParent.transform.name == "LowResHeat3")
        {
            gridParent.transform.position = new Vector3(6.585453f, -2.98f, -2.95f);
            gridParent.transform.rotation = Quaternion.Euler(-2.7f, -184.518f, 0.254f);
        }



        gridloaded = true;

    }

}