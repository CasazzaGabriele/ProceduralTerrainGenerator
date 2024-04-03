using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateProceduralMesh : MonoBehaviour
{

    #region Public_Variables

    // size Plane Variables
    [SerializeField] Vector2 planeSize;

    [Range(0.1f, 100f)]
    [SerializeField] float amplitude;

    [Range(0.1f, 10f)]
    [SerializeField] float distribuation;

    //gradient to color the mesh 
    [SerializeField] Gradient gradient;

    #endregion

    //------------------------------------------------------------------------------------------------

    #region Private_Variables


    // List Of Vertex
    List<Vector3> vertices;

    // List of index containg the triangles 
    List<int> triangles;

    float[] frequencise = { 1f, 2f, 4f, 8f, 16f, 32f};

    // mesh variables
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRender;

    // Terrein Height
    float maxTerreinHeight;
    float minTerreinHeight;

    // RandomValue For procedural
    float xSeed;
    float zSeed;

    float xNoise;
    float zNoise;

    float perlinNoise;
     // list of colors
    Color[] colors;

    #endregion

    //------------------------------------------------------------------------------------------------

    #region Generate_Function

    // retuns a nos vale that will be the height bvalue of the position of the vertex
    float GenerateYNoiseValue(float x , float z)
    {
        xNoise = x;
        zNoise = z ;

        float div = 0;
        perlinNoise = 0f;

        for (int i = 0; i < frequencise.Length; i++)
        {
            perlinNoise +=  1/ frequencise[i] * Mathf.PerlinNoise(frequencise[i] * xNoise , frequencise[i] * zNoise);
            div +=  1/ frequencise[i];

        }
        float perlinValue = perlinNoise/div * amplitude;

        return perlinValue;
         
    }

    // Generate the positionion of the vertex  in the 3D space thanke th size of the grid 
    Vector3 GenerateVerticesPosition(int x, int z,Vector2 size)
    {
        float spawnX = x * 1;
        float spawnY = GenerateYNoiseValue(x/size.x, z/size.y);
        float spawnZ = z * 1;

        print(spawnY);

        return new Vector3(spawnX, spawnY, spawnZ);
    }


    private void GenerateMesh(Vector2 size)
    {
        // creating a list with the vertex of the mesh 
        vertices = new List<Vector3>();

        for (int z = 0; z <= size.y; z++)
        {
            for (int x = 0; x <= size.x; x++)
            {
                Vector3 position = GenerateVerticesPosition(x, z, size);
                vertices.Add(position);

                // determinating terreing height for the clamping vale of the grandient color change
                if (position.y < minTerreinHeight)
                {
                    minTerreinHeight = position.y;
                }
                else if (position.y > maxTerreinHeight)
                {
                    maxTerreinHeight = position.y;
                }

            }

        }
        // list of index of the vertex of the mesh 
        triangles = new List<int>(vertices.ToArray().Length);
        for (int z = 0; z < (size.y-1); z++)
        {
            for (int x = 0; x < size.x-1; x++)
            {

                int i = (z * ((int)size.x)) + x + z;

                // First triangle
                triangles.Add(i);
                triangles.Add(i + (int)size.x + 1);
                triangles.Add(i + (int)size.x + 2);

                // Second triangle
                triangles.Add(i);
                triangles.Add(i + (int)size.x + 2);
                triangles.Add(i + 1);

            }

        }

        // inzialise colors list
        colors = new Color[vertices.ToArray().Length];
        for (int z = 0; z < (size.y - 1); z++)
        {
            for (int x = 0; x < size.x - 1; x++)
            {

                int i = (z * ((int)size.x)) + x + z;

                float height = Mathf.InverseLerp(minTerreinHeight, maxTerreinHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
            }

        }

    }


    //----------------------------------------------------------------------------------------------------------------

    private void AssignMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors;

    }

    //----------------------------------------------------------------------------------------------------------------------

    #endregion
    private void Awake()
    {



        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

    }

    private void Start()
    {
        GenerateMesh(planeSize);
        AssignMesh();

    }

    private void Update()
    {
 
    }
}
