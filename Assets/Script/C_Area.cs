using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Area : MonoBehaviour
{
    public GameObject wall, floor, bulpa;
    public GameObject[] portals = new GameObject[3];
    public Transform[] area = new Transform[3];
    public Vector3Int size = new Vector3Int(8,8,8);
    public int doors = 1;
    Vector3Int[] doorCoord;
    // Start is called before the first frame update
    void Start()
    {
        doorCoord = new Vector3Int[doors];
        for (int d = 0; d < doors; d++)
        {
            int[] limits = new int[6];
            int side = Random.Range(0, 3);
            switch (side)
            {
                case 0:
                    limits = new int[] { 2, size.x - 3, 0, 0, 2, size.z - 3 };
                    break;
                case 1:
                    limits = new int[] { 0, 0, 2, size.x - 3, 2, size.z - 3 };
                    break;
                case 2:
                    limits = new int[] { 2, size.x - 3, 0, 0, 2, size.z - 3 };
                    break;
            }
            doorCoord[d] = new Vector3Int(Random.Range(limits[0], limits[1] + 1), Random.Range(limits[2], limits[3] + 1), Random.Range(limits[4], limits[5] + 1));
        }

        genWall(floor, area[0], 0, 2);
        genWall(floor, area[1], 0, 1);
        genWall(floor, area[2], 2, 1);
        GameObject bulpaSpawner = Instantiate(portals[0], new Vector3(size.x - 0.5f, -0.5f, -size.z - 0.5f), Quaternion.identity);
        
        GameObject jumper = Instantiate(bulpa, bulpaSpawner.transform.position + Vector3.up, Quaternion.identity);
        jumper.name = "Bulpa";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void genWall (GameObject gO, Transform par, int c1, int c2)
    {
        Vector3 offset = Vector3.one / 2f;
        GameObject portal = portals[2];

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
            portal = portals[2];
            portal.transform.eulerAngles += 90 * Vector3.up;
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

                        generate = portal;
                        continue;
                    }
                }
                GameObject block = Instantiate(generate, C_MF.mulVec3(c0, (Vector3Int.one + 2 * Vector3Int.back)) + offset, Quaternion.identity, par);
                block.name = string.Format("{0}_{1}_{2}_{3}", generate.name, c0.x, c0.y, c0.z); 
            }
        }
    }
}
