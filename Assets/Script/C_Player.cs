using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_Player : MonoBehaviour
{
    //public GameObject[] Figures;
    Vector3 direct = new Vector3(1f, 1f, -1f);
    public GameObject cubes;
    public GameObject box;
    public int maxBoxes = 8;
    public Material[] materials = new Material[3];
    GameObject takingBlock;
    int col = 0;
    public Transform spawnPoint;
    public Text[] text = new Text[4];
    Vector3 rnd = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10000; i++)
        {
            int s = Random.Range(0, 2);
            rnd[s]++;
        }
        generateFigure();

    }

    // Update is called once per frame
    void Update()
    { 
            if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, 1 << 6))
            {
                takingBlock = hit.collider.transform.parent.gameObject;
                setKinematic(true);
                Debug.Log(string.Format("Take Block: {0}", takingBlock.name));
            }
            else
            {
                Debug.Log("NOTHING");
            }
        }
        if (Input.GetButton("Fire1"))
        {
            Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, 1 << 7))
            {
                Vector3 dir = (hit.collider.transform.position - hit.point);
                Vector3 dir2 = Vector3.zero;
                string[] m = new string[3];
                for (int c = 0; c < 3; c++)
                {
                    if (Mathf.Abs(dir[c]) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                    {
                        dir2[c] = direct[c] / 2;
                        m[c] = "()";
                    }
                    else
                    {
                        dir2[c] = -direct[c] / 2;
                        m[c] = "  ";
                    }
                }
                /*if (Mathf.Abs(dir.x) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                {
                    dir2.x = offset.x;
                    m[0] = "()";
                }
                else
                {
                    dir2.x = -offset.x;
                    m[0] = "  ";
                }

                if (Mathf.Abs(dir.y) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                {
                    dir2.y = offset.y;
                    m[1] = "()";
                }
                else
                {
                    dir2.y = -offset.y;
                    m[1] = "  ";
                }

                if (Mathf.Abs(dir.z) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                {
                    dir2.z = offset.z;
                    m[2] = "()";
                }
                else
                {
                    dir2.z = -offset.z;
                    m[2] = "  ";
                }
               /* text[3].text = string.Format("X: {0:f2} \nY: {1:f2} \nZ: {2:f2} \nMAX: {3:f2}", Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z), Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)));
                text[0].text = string.Format("{3}X{4}: ({0:f2} + ({1:f2}) = {2:f2}", hit.collider.transform.position.x, dir2.x, hit.collider.transform.position.x + dir2.x, m[0][0], m[0][1]);
                text[1].text = string.Format("{3}Y{4}: ({0:f2} + ({1:f2}) = {2:f2}", hit.collider.transform.position.y, dir2.y, hit.collider.transform.position.y + dir2.y, m[1][0], m[1][1]);
                text[2].text = string.Format("{3}Z{4}: ({0:f2} + ({1:f2}) = {2:f2}", hit.collider.transform.position.z, dir2.z, hit.collider.transform.position.z + dir2.z, m[2][0], m[2][1]);
               */
                dir2 += hit.collider.transform.position;
                takingBlock.transform.position = dir2 + 0 * direct / 2;
                reColor(1);

            }
            else
            {
                if (takingBlock != null)
                {
                    takingBlock.transform.position = spawnPoint.position;
                    reColor(0);
                }
            }
        }
        if ((Input.GetButtonUp("Fire1")) && (takingBlock != null))
        {
            switch (col)
            {
                case 0:
                    takingBlock.transform.position = spawnPoint.position + direct / 2;
                    break;
                case 1:
                    takingBlock.layer = 7;
                    Transform[] figure = takingBlock.GetComponentsInChildren<Transform>();
                    setKinematic(false);
                    foreach (Transform gO in figure)
                    {
                        if (gO != takingBlock.transform)
                        {
                            reColor(0);
                            gO.GetComponent<BoxCollider>().isTrigger = false;
                            gO.gameObject.layer = 7;
                            gO.parent = cubes.transform;
                        }
                        Destroy(takingBlock);
                    }
                    generateFigure();
                    break;
                case 2:
                    takingBlock.transform.position = spawnPoint.position;
                    break;

            }
            reColor(0);
            takingBlock = null;
        }
    }

    void reColor(int c)
    {
        col = c;
        for (int i = 0; i < takingBlock.GetComponentsInChildren<Renderer>().Length; i++)
        {
            takingBlock.GetComponentsInChildren<Renderer>()[i].material = materials[c];
        }
    }

    void setKinematic(bool b)
    {
        for (int i = 0; i < takingBlock.GetComponentsInChildren<Rigidbody>().Length; i++)
        {
            takingBlock.GetComponentsInChildren<Rigidbody>()[i].isKinematic = b;
        }
    }
    void generateFigure()
    {
        int boxes = Random.Range(1, maxBoxes + 1);
        bool[,,] notEmpty = new bool[boxes + 1, boxes + 1, boxes + 1];
        Vector3Int coords;

        GameObject fig = Instantiate(spawnPoint.gameObject, spawnPoint.parent.transform);
        fig.name = "new Figure";
        coords = Vector3Int.zero;
        for (int i = 0; i < boxes;)
        {
            int type;
            if (boxes - i >= 4) type = C_MF.rnd(4, 2, 1);
            else if (boxes - i >= 2) type = C_MF.rnd(4, 2);
            else type = 0;
            GameObject newBox = Instantiate(box, fig.transform);
            coords = Vector3Int.zero;
            Vector3 scale = Vector3.one;
            if (type == 1)
            {
                scale[Random.Range(0, 3)] = 2f;
            }
            if (type == 2)
            {
                int s1 = Random.Range(0, 3);
                int s2 = Random.Range(0, 2);
                if (s2 == s1) s2++;
                scale[s1] = 2f;
                scale[s2] = 2f;

            }
            newBox.transform.localPosition = C_MF.mulVec3(direct / 2, scale);
            newBox.layer = 2;
            newBox.transform.localScale = C_MF.mulVec3(newBox.transform.localScale, scale);
            bool can = true;
            for (int x = coords.x; x < coords.x + scale.x; x++)
            {
                for (int y = coords.y; y < coords.y + scale.y; y++)
                {
                    for (int z = coords.y; z < coords.z + scale.z; z++)
                    {
                        if (notEmpty[x, y, z] == true) can = false;
                    }
                }
            }
            while (!can)
            {
                Vector3 move = Vector3.zero;
                move[Random.Range(0, 3)] = 1;
                move = C_MF.mulVec3(move, direct);
                coords += new Vector3Int(Mathf.FloorToInt(move.x), Mathf.FloorToInt(move.y), Mathf.FloorToInt(-move.z));
                newBox.transform.localPosition += move;
                can = true;
                for (int x = coords.x; x < coords.x + scale.x; x++)
                {
                    for (int y = coords.y; y < coords.y + scale.y; y++)
                    {
                        for (int z = coords.z; z < coords.z + scale.z; z++)
                        {
                            if (notEmpty[x, y, z] == true) can = false;
                        }
                    }
                }
            }
            newBox.layer = 6;
            for (int x = coords.x; x < coords.x + scale.x; x++)
            {
                for (int y = coords.y; y < coords.y + scale.y; y++)
                {
                    for (int z = coords.z; z < coords.z + scale.z; z++)
                    {
                        notEmpty[x, y, z] = true;
                    }
                }
            }
            i += (int) Mathf.Pow(2, type);

        }
    }
}
