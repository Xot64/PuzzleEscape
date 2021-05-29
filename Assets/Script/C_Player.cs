using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_Player : MonoBehaviour
{
    public GameObject[] Figures;
    public GameObject cubes;
    public Material[] materials = new Material[3];
    GameObject takingBlock;
    int col = 0;
    public Transform spawnPoint;
    public Text[] text = new Text[4];
    // Start is called before the first frame update
    void Start()
    {

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
                Vector3 dir2;
                string[] m = new string[3];
                if (Mathf.Abs(dir.x) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                {
                    dir2.x = 0.5f;
                    m[0] = "()";
                }
                else
                {
                    dir2.x = -0.5f;
                    m[0] = "  ";
                }

                if (Mathf.Abs(dir.y) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                {
                    dir2.y = 0.5f;
                    m[1] = "()";
                }
                else
                {
                    dir2.y = -0.5f;
                    m[1] = "  ";
                }

                if (Mathf.Abs(dir.z) == Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)))
                {
                    dir2.z = -0.5f;
                    m[2] = "()";
                }
                else
                {
                    dir2.z = 0.5f;
                    m[2] = "  ";
                }
                text[3].text = string.Format("X: {0:f2} \nY: {1:f2} \nZ: {2:f2} \nMAX: {3:f2}", Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z), Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)));
                text[0].text = string.Format("{3}X{4}: ({0:f2} + ({1:f2}) = {2:f2}", hit.collider.transform.position.x, dir2.x, hit.collider.transform.position.x + dir2.x, m[0][0], m[0][1]);
                text[1].text = string.Format("{3}Y{4}: ({0:f2} + ({1:f2}) = {2:f2}", hit.collider.transform.position.y, dir2.y, hit.collider.transform.position.y + dir2.y, m[1][0], m[1][1]);
                text[2].text = string.Format("{3}Z{4}: ({0:f2} + ({1:f2}) = {2:f2}", hit.collider.transform.position.z, dir2.z, hit.collider.transform.position.z + dir2.z, m[2][0], m[2][1]);

                dir2 += hit.collider.transform.position;
                takingBlock.transform.position = dir2;
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
            if (col != 1)
            {
                takingBlock.transform.position = spawnPoint.position;

            }
            takingBlock.layer = 7;
            Transform[] figure = takingBlock.GetComponentsInChildren<Transform>();
            reColor(0);
            setKinematic(false);
            foreach (Transform gO in figure)
            {
                gO.gameObject.layer = 7;
                gO.parent = cubes.transform;
                Destroy(takingBlock);
            }
            
            
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
}
