using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    Terrain terrain;
    TerrainData terrainData;

    public GameObject targetPositionObject;
    Vector3 targetPosition;

    public Simulation simulation;
    public bool succeed = false;
    public bool finish = false;

    Vector3 newAngle = new Vector3(0, 0, 0);

    //물리 계산 변수
    float tan;
    Vector3 velocity = new Vector3(0f, 0f, 0f);
    float friction = 0.3f;
    Vector3 gravity = new Vector3(0, 9.8f, 0);
    float greenFriction = 0f;
    public float T = 0.5f; //0.25f;
    Vector3 FrictionA = new Vector3(0, 0, 0);
    Vector3[] normVecs = new Vector3[3];
    Vector3 side1, side2, perp;
    float biasOfX;
    float biasOfZ;

    public bool isColliedGGreen = false;
    int cnt = 0;  //.//
    
    public void setVelocity(Vector3 v)
    {
        velocity = v;
        velocity = velocity - perp * (Vector3.Dot(perp, velocity));
    }
    public void initMoveBall(Terrain t)
    {
        this.terrain = t;
        terrainData = terrain.terrainData;
        biasOfX = terrain.transform.position.x;
        biasOfZ = terrain.transform.position.z;
        Debug.Log("set Terrain "+terrain.name+"X of pos "+terrain.transform.position.x);
    }
    public void setBallPos(Vector3 biasPos)
    {
        transform.position = biasPos;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void startMovingBall()
    {
        targetPosition = targetPositionObject.transform.position;
        //while(simulation.state == Progress.StateLevel.Roll && cnt<11)
        //{
            if (Mathf.Abs(targetPosition.x - transform.position.x) < 1.3f && Mathf.Abs(targetPosition.z - transform.position.z) < 1.3f)
            {
                Debug.Log("정답!");
                succeed = true;
                velocity = new Vector3(0f, 0f, 0f);
                finish = true;//들어갔다

                return;
            }

            if ((Mathf.Round(velocity.z * 1000) * 0.001f) <= 1)
            {
                finish = true; // 끝났다
                return;
            }
            if (transform.position.x - biasOfX < 75 || transform.position.x - biasOfX > 375 || transform.position.z - biasOfZ < 75 || transform.position.z - biasOfZ > 375)
            {
                finish = true;
                return;
            }
            //norm
            //targetPosition = targetPositionObject.transform.position;
            normVecs[0] = new Vector3(transform.position.x, getHeight(transform.position.x - biasOfX, transform.position.z - biasOfZ), transform.position.z);
            normVecs[1] = new Vector3(transform.position.x + 1, getHeight(transform.position.x + 1 - biasOfX, transform.position.z - biasOfZ), transform.position.z);
            normVecs[2] = new Vector3(transform.position.x, getHeight(transform.position.x - biasOfX, transform.position.z + 1 - biasOfZ), transform.position.z + 1);

            side1 = normVecs[1] - normVecs[0];
            side2 = normVecs[2] - normVecs[0];

            perp = Vector3.Cross(side2, side1);
            perp = perp.normalized;

            Debug.Log("법선 : "+perp.ToString("F3"));

            // 중력
            float height = getHeight(transform.position.x - biasOfX, transform.position.z - biasOfZ);
            Vector3 gravityA;
            if (transform.position.y <= height + 1f)
            {
                gravityA = new Vector3(gravity.y * perp.x, gravity.y * (perp.y - 1.5f), gravity.y * perp.z);
            }
            else
            {
                gravityA = new Vector3(0, -gravity.y, 0);
            }

            //마찰력
            greenFriction = Mathf.Lerp(0.8f, 0.3f, velocity.magnitude / T); // 운동, 정지 마찰력 //  0.3f * (Vector3.Magnitude(velocity) / T) + 0.4f * ((T - Vector3.Magnitude(velocity) / T));
            FrictionA = -perp.y * gravity.y * velocity.normalized * greenFriction;

            velocity += (gravityA + FrictionA) * Time.deltaTime;
            //transform.position += velocity * Time.deltaTime;
            transform.Translate(velocity * Time.deltaTime); // local 좌표로 이동
            height = getHeight(transform.position.x-biasOfX, transform.position.z - biasOfZ);
            if (height + 1f > transform.position.y)
                transform.Translate(0, height + 1f - transform.position.y, 0);
            //cnt++;
        //if (cnt == 11) finish = true;
        //}
    }
    /*
    void FixedUpdate()
    {
        if (simulation.state == Progress.StateLevel.Start)
        {
            //norm
            targetPosition = targetPositionObject.transform.position;

        }
        if (simulation.state == Progress.StateLevel.Roll)
        {
            if (Mathf.Abs(targetPosition.x - transform.position.x) < 1.3f && Mathf.Abs(targetPosition.z - transform.position.z) < 1.3f)
            {
                Debug.Log("정답!");
                succeed = true;
                velocity = new Vector3(0f, 0f, 0f);
                finish = true;//들어갔다

                return;
            }

            if ((Mathf.Round(velocity.z * 1000) * 0.001f) <= 1)
            {
                finish = true; // 끝났다
                return;
            }
            if (transform.position.x < 75 || transform.position.x > 375 || transform.position.z < 75 || transform.position.z > 375)
            {
                finish = true;
                return;
            }
            //norm
            targetPosition = targetPositionObject.transform.position;

            perp = Vector3.Cross(side2, side1);
            perp = perp.normalized;

            // 중력
            float height = getHeight(transform.position.x, transform.position.z);
            Vector3 gravityA;
            if (transform.position.y <= height + 1f)
            {
                gravityA = new Vector3(gravity.y * perp.x, gravity.y * (perp.y - 1.5f), gravity.y * perp.z);
            }
            else
            {
                gravityA = new Vector3(0, -gravity.y, 0);
            }

            //마찰력
            greenFriction = Mathf.Lerp(0.8f, 0.3f, velocity.magnitude / T); // 운동, 정지 마찰력 //  0.3f * (Vector3.Magnitude(velocity) / T) + 0.4f * ((T - Vector3.Magnitude(velocity) / T));
            FrictionA = -perp.y * gravity.y * velocity.normalized * greenFriction;

            velocity += (gravityA + FrictionA) * Time.deltaTime;
            //transform.position += velocity * Time.deltaTime;
            transform.Translate(velocity * Time.deltaTime); // local 좌표로 이동
            height = getHeight(transform.position.x, transform.position.z);
            if (height + 1f > transform.position.y)
                transform.Translate(0, height + 1f - transform.position.y, 0);
        }
    }
    */
    float getHeight(float x, float z)
    {
        float ld, rd, lu, ru;
        ld = terrain.SampleHeight(new Vector3(Mathf.FloorToInt(x), 0f, Mathf.FloorToInt(z))); // td.GetHeight(Mathf.FloorToInt(x), Mathf.FloorToInt(z));
        rd = terrain.SampleHeight(new Vector3(Mathf.CeilToInt(x), 0f, Mathf.FloorToInt(z)));  //td.GetHeight(Mathf.CeilToInt(x), Mathf.FloorToInt(z));
        lu = terrain.SampleHeight(new Vector3(Mathf.FloorToInt(x), 0f, Mathf.CeilToInt(z))); //td.GetHeight(Mathf.FloorToInt(x), Mathf.CeilToInt(z)); 
        ru = terrain.SampleHeight(new Vector3(Mathf.CeilToInt(x), 0f, Mathf.CeilToInt(z))); //td.GetHeight(Mathf.CeilToInt(x), Mathf.CeilToInt(z));
        float l, r;
        l = Mathf.Lerp(ld, lu, z - Mathf.Floor(z));
        r = Mathf.Lerp(rd, ru, z - Mathf.Floor(z));
        return Mathf.Lerp(l, r, x - Mathf.Floor(x));
    }
    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("onCollis entered");
    //    if (collision.gameObject.name == terrain.name)
    //    {
    //        GetComponent<Rigidbody>().isKinematic = true;
    //        isColliedGGreen = true;
    //        Debug.Log(terrain.name + "/충돌 "+isColliedGGreen);
    //    }
    //}
    public void turnOffKinematic()
    {
        GetComponent<Rigidbody>().isKinematic = false;

    }

    public void ChangeAngle(float y)
    {
        transform.Rotate(new Vector3(0, y, 0));
    }
}
