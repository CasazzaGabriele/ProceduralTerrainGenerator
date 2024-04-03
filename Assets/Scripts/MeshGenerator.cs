using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    // plane size variables
    [SerializeField] Vector2 size;

    [Range(0.1f, 10f)]
    [SerializeField] float frequency;

    [Range(0.1f, 100f)]
    [SerializeField] float amplitudeElevator;

    //gradient to color the mesh 
    [SerializeField] Gradient gradient;


    // Terrein Height
    float maxTerreinHeight;
    float minTerreinHeight;

    // Offset value:
    float offset = 1 ;

    // axis n suddivisation
    int xSuddivisation;
    int zSuddivisation;

    // variables for nois y generation value

    float[] amplitudes = {1f, 2f, 4f, 8f}; 

    float noisx;
    float noisz;

    float seedX;
    float seedZ;

    float perlinNois;

    // List Of Vertex
    List<Vector3> vertices;

 

    // List of index containg the triangles 
    List<int> triangles;

    // mesh variables
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRender;

    // list of colors
    Color[] colors;
    Material material;


    private void Awake()
    {
        seedX = Random.Range(0, 999f);
        seedZ = Random.Range(0, 999f);

        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

    }

    // Start is called before the first frame update
    void Start()
    {
        Mesh_genertaor(size);
        AssignMesh();
        
    }

    //-------------------------------------------------------------------------------------------------------------------------------
    private void Mesh_genertaor(Vector2 size)
    {
        // creating a list with the vertex of the mesh 
        vertices = new List<Vector3>();

        // n suddivistion for each axis
        int xSuddivisation = System.Convert.ToInt32(size.x / offset);
        int zSuddivisation = System.Convert.ToInt32(size.y / offset);

      
        for (int z = 0; z <= zSuddivisation; z++)
        {
            for (int x = 0; x <= xSuddivisation; x++)
            {
                Vector3 position = SpawnPosition(x, z);
                vertices.Add(position);

                // determinating terreing height for the clamping vale of the grandient color change
                if(position.y < minTerreinHeight)
                {
                    minTerreinHeight = position.y;
                }
                else if( position.y > maxTerreinHeight)
                {
                    maxTerreinHeight = position.y;
                }

            }

        }


        // list of index of the vertex of the mesh 
        triangles = new List<int>();
        for (int z =0 ; z< (zSuddivisation); z++)
        {
            for (int x = 0; x < xSuddivisation; x++)
            {

                int i = (z * xSuddivisation) + x + z;

                // First triangle
                triangles.Add(i);
                triangles.Add(i + xSuddivisation + 1);
                triangles.Add(i + xSuddivisation + 2);

                // Second triangle 
                triangles.Add(i);
                triangles.Add(i + xSuddivisation + 2);
                triangles.Add(i + 1);

            }
            
        }

        // inzialise colors list
        colors = new Color[vertices.ToArray().Length];
        for (int z = 0; z <= (zSuddivisation); z++)
        {
            for (int x = 0; x <= xSuddivisation; x++)
            {

                int i = (z * xSuddivisation) + x + z;

                float height = Mathf.InverseLerp(minTerreinHeight, maxTerreinHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
            }

        }
         
    }
    //-------------------------------------------------------------------------------------------------------------------------------
    float GeneratorNoisY(int x, int z)
    {
        noisx = x  * seedX; 
        noisz = z  * seedZ; 

        float div = 0;
        perlinNois = 0f;

        for ( int i = 0; i < amplitudes.Length; i++)
        {
            perlinNois += (offset/amplitudes[i] * Mathf.PerlinNoise(( amplitudes[i]) * noisx, ( amplitudes[i]) * noisz));
            div += offset/amplitudes[i];

        }
        float perlinValue = perlinNois / div;


        return Mathf.Pow(perlinValue, amplitudeElevator);
        
    }

    //-------------------------------------------------------------------------------------------------------------------------------
    private void AssignMesh()
    {
         mesh.Clear();
         mesh.vertices = vertices.ToArray();
         mesh.triangles = triangles.ToArray();
         mesh.colors = colors;

    }

    //-------------------------------------------------------------------------------------------------------------------------------
    private Vector3 SpawnPosition(int x , int z )
    {
        float spawnX =  x * offset;
        float spawnY =  GeneratorNoisY(x, z) ;
        float spawnZ =  z * offset;


        return new Vector3(spawnX, spawnY, spawnZ);
    }

   


   
}
