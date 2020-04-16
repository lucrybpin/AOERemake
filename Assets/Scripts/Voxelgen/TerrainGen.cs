using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public List<GameObject> treeCollection = new List<GameObject>();
    public List<GameObject> rockCollection = new List<GameObject>();
    private int heightScale = 4;
    private float detailScale = 4.2f; 
    private Transform terrainHolder;

    private World world;

    private void noiseGenereation(Vector3[] verts, int v)
    {
        float noiseSmoothing = 3f;
        float noiseIndex = Noise.GetNoise((verts[v].x + this.transform.position.x) / noiseSmoothing,
            (verts[v].y + this.transform.position.y * noiseSmoothing),
            (verts[v].z + this.transform.position.z) / detailScale) * heightScale;

        verts[v].y = (noiseIndex / noiseSmoothing) * 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<World>() as World;
        terrainHolder = FindObjectOfType<TerrainPool>().transform;
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        Vector3[] verts = mesh.vertices;

        for (int v = 0; v < verts.Length; v++)
        {
            noiseGenereation(verts, v);

            // The pure random function will not work on limitless or cheated terrain as the pos is not saved.. 
            // Hashtable or Disctionary MAybe? Or another perlin noise?
            if (verts[v].y > .4f && Random.Range(0, 100) <= world.treeDensity)
            {
                GenerateTree(verts, v);
            }

            if (verts[v].y > 0.2f && verts[v].y < 0.3f && Random.Range(0, 100) < world.rockDensity)
            {
                GenerateRock(verts, v);
            }
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        this.gameObject.AddComponent<MeshCollider>();

        ObjectPlacementGrid(mesh);
    }

    private void OnDestroy()
    {
        foreach(GameObject tree in treeCollection) {
            tree.SetActive(false);
        }
        foreach(GameObject rock in rockCollection){
            rock.SetActive(false);
        }
    }

    private void GenerateTree(Vector3 [ ] verts, int v) {
        GameObject getTerrainTrees = TerrainPool.getTrees();

        if (getTerrainTrees != null && world.treeDensity != 0) {
            Vector3 treePos = new Vector3(verts [ v ].x + this.transform.position.x,
                                        verts [ v ].y + this.transform.position.y,
                                        verts [ v ].z + this.transform.position.z);

            getTerrainTrees.transform.position = treePos;
            getTerrainTrees.SetActive(true);
            getTerrainTrees.transform.SetParent(terrainHolder);

            treeCollection.Add(getTerrainTrees);
        }
    }

    private void GenerateRock(Vector3 [ ] verts, int v) {
        GameObject getTerrainRocks = TerrainPool.getRocks();

        if (getTerrainRocks != null && world.rockDensity != 0) {
            Vector3 rockPos = new Vector3(verts [ v ].x + this.transform.position.x,
                                        verts [ v ].y + this.transform.position.y,
                                        verts [ v ].z + this.transform.position.z);

            getTerrainRocks.transform.position = rockPos;
            getTerrainRocks.SetActive(true);
            getTerrainRocks.transform.SetParent(terrainHolder);

            rockCollection.Add(getTerrainRocks);
        }
    }

    private void ObjectPlacementGrid(Mesh mesh) {

        //in progress...

        //Strategy is to find the center of the square of each chunk and store it in a array or list
        
        //1. Get the Grid
        //2. Relate Grid to Chunk
        //3. Onclick -> find Grid index and chunk related
        //4. Grid index should point to a Building

        //Future Problems:
        //How to handle changes in the environment?
        //How to handle different levels in Y axis?

        int i = 0;
        Vector3 firstPoint = new Vector3();
        Vector3 lastPoint = new Vector3();

        //For every square (4 vertices)
        for (int it = 0; it < mesh.vertices.Length; it+=4) {
            firstPoint = mesh.vertices [ it ];//1
            lastPoint = mesh.vertices [ it + 2 ];//3
            GameObject centerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            centerCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            centerCube.transform.position = ( ( transform.TransformPoint(firstPoint) + transform.TransformPoint(lastPoint) ) / 2 );
        }
    }
}
