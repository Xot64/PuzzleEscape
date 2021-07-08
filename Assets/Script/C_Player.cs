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
    Vector3[] boxSizes;
    Transform[] boxes;
    // Update is called once per frame
    void Update()
    {
        text[0].text = string.Format("{0}-{1}", C_GameValues.World, C_GameValues.level);
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, 1 << 6))
            {
                takingBlock = hit.collider.transform.parent.gameObject;
                boxes = takingBlock.GetComponentsInChildren<Transform>();
                boxSizes = new Vector3[boxes.Length];
                for (int i = 0; i < boxes.Length; i++)
                {
                    boxSizes[i] = boxes[i].localScale;
                }
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
                Vector3 dir;
                Vector3 dir2;

                if (hit.collider.tag == "Wall")
                {
                    dir2 = C_MF.NormalAngles(hit.transform.eulerAngles) / 90f - direct / 2f;
                }
                else
                {
                    dir = (hit.collider.transform.position - hit.point);
                    Vector3 bS = new Vector3 (C_MF.Round(hit.transform.localScale.x,0), C_MF.Round(hit.transform.localScale.y, 0), C_MF.Round(hit.transform.localScale.z, 0));
                    Vector3 bB = new Vector3(Mathf.Sign(dir.x), Mathf.Sign(dir.y), Mathf.Sign(dir.z)) / 2;


                    dir2 = Vector3.zero;
                    string[] m = new string[3];
                    for (int c = 0; c < 3; c++)
                    {
                        if (Mathf.Abs(dir[c]/bS[c]) == Mathf.Max(Mathf.Abs(dir.x/ bS.x), Mathf.Abs(dir.y / bS.y), Mathf.Abs(dir.z / bS.z)))
                        {
                            dir2[c] += direct[c] / 2;
                            m[c] = "()";
                        }
                        else
                        {
                            dir2[c] += -direct[c] / 2;
                            m[c] = "  ";
                        }
                    }
                    dir2 += C_MF.mulVec3((bS - Vector3.one), -bB);
                }
                
                dir2 += hit.collider.transform.position;

                takingBlock.transform.position = dir2 + Vector3.up * 0.01f;
                boxSize(0.99f);
                reColor(onTrig() ? 2 : 1);

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
            takingBlock.transform.localScale = Vector3.one;
            boxSize(1f);
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
            if (col == 0)
                takingBlock.GetComponentsInChildren<BoxCollider>()[i].tag = "board";
            else
                takingBlock.GetComponentsInChildren<BoxCollider>()[i].tag = "Unvis";
        }
    }

    bool onTrig()
    {
        C_BoxCollider[] triggers = takingBlock.GetComponentsInChildren<C_BoxCollider>();
        foreach (C_BoxCollider BC in triggers)
        {
            if (BC.trig != null) if (BC.tag != "Unvis") return true;
        }
        return false;
    }

    void boxSize(float s)
    {

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].localScale = s * boxSizes[i];
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
