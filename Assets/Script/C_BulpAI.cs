using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class C_BulpAI : MonoBehaviour
{
    float g;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        g = Mathf.Abs(Physics.gravity.y);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("step")) step(stepTime, correctStep);
        if ((!onAir) && (!onSpin))
        {
            if (Input.GetButtonDown("left")) direct -= 90f;
            if (Input.GetButtonDown("right")) direct += 90f;
            if (Input.GetButtonDown("jump")) step(jumpTime, correctJump, 1);
            if (Input.GetButtonDown("downJump")) step(jumpTime, correctDown, -1);
        }
        spin();
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
           // onAir = true;
            t = time;
            GetComponent<Rigidbody>().velocity = gameObject.transform.forward  / t * k + Vector3.up * (g * t / 2 + level / t);
            lastStep = Time.time;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "board")
        {
            onAir = false;
            transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, transform.position.y, Mathf.Floor(transform.position.z) + 0.5f);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "board")
        {
            onAir = true;
            transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, transform.position.y, Mathf.Floor(transform.position.z) + 0.5f);
        }
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
            transform.eulerAngles = Vector3.up * direct;
            onSpin = false;
        }
    }
}
