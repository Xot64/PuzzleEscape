using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Area : MonoBehaviour
{
    public GameObject wall, floor, bulpa;
    public GameObject[] portals = new GameObject[4];
    public Transform[] area = new Transform[3];
    public Vector3Int size = new Vector3Int(8,8,8);
    public int doors = 1;
    Vector3Int[] doorCoord;
    // Start is called before the first frame update
    void Start()
    {
        doors = C_GameValues.World;
        size = Vector3Int.one * (4 + C_GameValues.level);
        doorCoord = new Vector3Int[doors];
        for (int d = 0; d < doors; d++)
        {
            bool isNew = false;
            while (!isNew)
            {
                int[] limits = new int[6];
                int side = Random.Range(0, 3); // 0 - пол, 1 - правая стена, 2 - левая стена
                switch (side)
                {
                    case 0:
                        limits = new int[] { 2, size.x - 3, 0, 0, 2, size.z - 3 };
                        break;
                    case 1:
                        limits = new int[] { 0, 0, 2, size.y - 1, 1, size.z - 1 };
                        break;
                    case 2:
                        limits = new int[] { 2, size.x - 1, 2, size.y - 1, 0, 0 };
                        break;
                }

                isNew = true;
                doorCoord[d] = new Vector3Int(Random.Range(limits[0], limits[1] + 1), Random.Range(limits[2], limits[3] + 1), Random.Range(limits[4], limits[5] + 1));
                for (int i = 0; i < d; i++)
                {
                    if (doorCoord[i] == doorCoord[d]) isNew = false;
                }
            }
        }

        genWall(floor, area[0], 0, 2);
        genWall(wall, area[1], 0, 1);
        genWall(wall, area[2], 2, 1);
        GameObject board1 = Instantiate(wall, new Vector3(size.x / 2f , size.y / 2f, -size.z - 0.5f), Quaternion.identity);
        
        board1.transform.localScale = new Vector3(size.x + 2, size.y + 2, 1);
        board1.GetComponent<BoxCollider>().isTrigger = true;
        Destroy(board1.GetComponent<MeshRenderer>());
        Destroy(board1.GetComponent<MeshFilter>());
        board1.layer = 2;

        GameObject board2 = Instantiate(wall, new Vector3(size.x + 0.5f, size.y / 2f, -size.z / 2f), Quaternion.identity);
        board2.transform.localScale = new Vector3(1, size.y + 2, size.z + 2);
        board2.GetComponent<BoxCollider>().isTrigger = true;
        Destroy(board2.GetComponent<MeshRenderer>());
        Destroy(board2.GetComponent<MeshFilter>());
        board2.layer = 2;

        GameObject board3 = Instantiate(wall, new Vector3(size.x / 2f, size.y + 0.5f, -size.z / 2f), Quaternion.identity);
        board3.transform.localScale = new Vector3(size.x + 2, 1, size.z + 2);
        board3.GetComponent<BoxCollider>().isTrigger = true;
        Destroy(board3.GetComponent<MeshRenderer>());
        Destroy(board3.GetComponent<MeshFilter>());
        board3.layer = 2;

        GameObject bulpaSpawner = Instantiate(portals[0], new Vector3(size.x - 0.5f, -0.5f, -size.z - 0.5f), Quaternion.identity);
        
        GameObject jumper = Instantiate(bulpa, bulpaSpawner.transform.position + Vector3.up, Quaternion.identity);
        jumper.name = "Bulpa";
        jumper.GetComponent<C_BulpAI>().start = bulpaSpawner;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void genWall (GameObject gO, Transform par, int c1, int c2)
    {
        Vector3 offset = Vector3.one / 2f;
        GameObject portal = portals[2];
        Vector3 angle = new Vector3 (((c1 == 0) || (c2 == 0)) ? 0f : 90f, ((c1 == 1) || (c2 == 1)) ? 0f : 90f, ((c1 == 2) || (c2 == 2)) ? 0f : -90f);
        if (((c1 == 0) && (c2 == 2)) || ((c1 == 2) && (c2 == 0))) //Пол
        {
            offset += Vector3.down + Vector3.back;
            portal = portals[1];
        }
        if (((c1 == 0) && (c2 == 1)) || ((c1 == 1) && (c2 == 0))) //Правая стена
        {
            offset += Vector3.zero;
            portal = portals[2];
        }
        if (((c1 == 1) && (c2 == 2)) || ((c1 == 2) && (c2 == 1))) //Левая стена
        {
            offset += Vector3.back + Vector3.left;
            portal = portals[3];
        }
            Vector3Int c0 = Vector3Int.zero;
        GameObject generate;
        for (c0[c1] = 0; c0[c1] < size[c1]; c0[c1]++)
        {
            for (c0[c2] = 0; c0[c2] < size[c2]; c0[c2]++)
            {
                generate = gO;
                for (int d = 0; d < doors; d++)
                {
                    if (doorCoord[d] == c0)
                    {
                        portal.GetComponent<C_Finish>().exit = (d == 0);
                        generate = portal;
                        continue;
                    }
                }

                GameObject block = Instantiate(generate, C_MF.mulVec3(c0, (Vector3Int.one + 2 * Vector3Int.back)) + offset, Quaternion.Euler(angle), par);
                block.name = string.Format("{0}_{1}_{2}_{3}", generate.name, c0.x, c0.y, c0.z); 
            }
        }
    }
}
