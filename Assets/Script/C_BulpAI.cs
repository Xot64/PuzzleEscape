using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class C_BulpAI : MonoBehaviour
{
    float g;
    float t;
    bool finish = false;
    int spinState = 0;
    bool CV = false;

    // Start is called before the first frame update
    void Start()
    {
        g = Mathf.Abs(Physics.gravity.y);
    }
    float finishTime;
    public float finishAngle = 3f;
    public float finishDelay = 2f;
    Vector3 finishDir;
    public GameObject start;
    C_Finish portal;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButton("step")) step(stepTime, correctStep);
        if ((!onAir) && (!onSpin) && (!finish))
        {
            switch (checkBoxes())
            {
                case 0:
                    step(stepTime, correctStep);
                    break;
                case 1:
                    step(jumpTime, correctJump, 1);
                    break;
                case 2:
                    spinState++;
                    if (spinState > 2) spinState = 1;
                    switch(spinState)
                    {
                        case 1:
                            direct += CV ? 90 : -90;
                            break;
                        case 2:
                            direct += CV ? -180 : 180;
                            CV = !CV;
                            break;
                    }
                    
                    break;
                case 3:
                    step(jumpTime, correctDown, -1);
                    break;
                case 4:
                    finish = true;
                    finishTime = Time.time;
                    break;
            }
        }
        spin();
        if (finish)
        {
            if (Time.time - finishTime <= finishDelay)
            {
                transform.Rotate(Vector3.up * finishAngle);
                transform.localScale = Vector3.one * ((finishDelay + 0.1f) - (Time.time - finishTime)) / finishDelay;
            }
            else if (Time.time - finishTime <= 2* finishDelay)
            {
                if  (portal != null)
                {
                    bool exit = portal.GetComponent<C_Finish>().exit;
                    if (!exit)
                    {
                        portal.GetComponent<C_Finish>().boom(finishDir != Vector3.up);
                    }
                    else
                    {
                        C_GameValues.level++;
                        if (C_GameValues.level > 4)
                        {
                            C_GameValues.level = 1;
                            C_GameValues.World++;
                        }
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                transform.position = start.transform.position + Vector3.up / 2;
                transform.Rotate(Vector3.up * finishAngle);
                transform.localScale = Vector3.one * ((Time.time - finishTime) - finishDelay) / finishDelay;
            }
            else if (Time.time - finishTime <= 2 * finishDelay + 0.5f)
            {
                transform.eulerAngles = Vector3.zero;
            }
            else
            {
                finish = false;
                onSpin = false;
                onAir = false;
                direct = 0;
            }
        }
    }


    bool onAir = false;
    public float correctStep = 1.15f;
    public float correctJump = 1.15f;
    public float correctDown = 1.15f;
    public float stepTime = 0.4f;
    float lastStep;
    public float stepDelay = 0.5f;
    public float jumpTime = 1f;
    void step(float time, float k, int level = 0)
    {        
        if ((Time.time >= lastStep + stepDelay) && (!onAir) && (!onSpin))
        {
            spinState = 0;
           // onAir = true;
            t = time;
            GetComponent<Rigidbody>().velocity = gameObject.transform.forward  / t * k + Vector3.up * (g * t / 2 + level / t);
            lastStep = Time.time;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((collision.collider.tag == "Wall") || (collision.collider.tag == "board") || (collision.collider.tag == "Finish"))
        {
            onAir = false;
            transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, transform.position.y, Mathf.Floor(transform.position.z) + 0.5f);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if ((collision.collider.tag == "Wall") || (collision.collider.tag == "board") || (collision.collider.tag == "Finish"))
        {
            onAir = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.tag == "Wall") || (collision.collider.tag == "board") || (collision.collider.tag == "Finish"))
        {
            onAir = false;
        }
        checkBoxes();
    }

    bool onSpin = false;
    float direct = 0f;
    public float rotTime = 0.3f;
    void spin()
    {
        float angle = (transform.eulerAngles.y);
        float cD = C_MF.toRotation(angle, direct);
        if (Mathf.Abs(cD) > 0.01)
        {
            onSpin = true;
            float dTA = Time.deltaTime * 90 / rotTime;
            float dA = Mathf.Min(dTA, Mathf.Abs(cD)) * Mathf.Sign(cD);
            transform.eulerAngles = C_MF.NormalAngles(transform.eulerAngles) + Vector3.up * dA;  
        }
        else
        {
            if (onSpin)
            {
                transform.eulerAngles = Vector3.up * direct;
                onSpin = false;
                checkBoxes();
            }
        }
    }

    int checkBoxes()
    {
        // 0 - Шаг вперед
        // 1 - Прыгнуть вверх
        // 2 - Прыгнуть невозможно - поворот
        // 3 - Прыгнуть вниз
        // 4 - Выход
        C_BoxCollider[] eyes = transform.GetChild(1).GetComponentsInChildren<C_BoxCollider>();
        if (eyes[0].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[0].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.up; 
            return 4;
        }
        if (eyes[1].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[1].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.right;
            return 4;
        }
        if (eyes[2].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[2].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.right;
            return 4;
        }
        if (eyes[4].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[4].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.forward;
            return 4;
        }
        if ((eyes[4].tag != "Unvis") && (eyes[4].tag != ""))
        {
           // Debug.Log(string.Format("Впереди ящик"));
            if ((eyes[3].tag != "Unvis") && (eyes[3].tag != ""))
            {
         //       Debug.Log(string.Format("Не могу запрыгнуть"));
                return 2;
            }
            else
            {
           //     Debug.Log(string.Format("Могу запрыгнуть"));
                return 1;
            }
        }
        else
        {
          //  Debug.Log(string.Format("Впереди ящика нет"));
            if ((eyes[5].tag != "Unvis") && (eyes[5].tag != ""))
            {
           //     Debug.Log(string.Format("Могу идти вперед"));
                return 0;
            }
            else
            {
           //     Debug.Log(string.Format("Впереди спуск"));
                if ((eyes[6].tag != "Unvis") && (eyes[6].tag != ""))
                {
            //        Debug.Log(string.Format("Могу спрыгнуть"));
                    return 3;
                }
                else
                {
            //        Debug.Log(string.Format("Не могу спрыгнуть"));
                    return 2;
                }
            }
        }    
    }
}
