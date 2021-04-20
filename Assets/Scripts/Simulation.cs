using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Simulation : Progress
{
    public MoveBall mb;
    bool stateIsNone = true;
    bool foundAns; // 정답찾기
    bool isNewStart = true;

    int numberOfSimulation;//시뮬레이션 횟수

    System.Random random;

    public GameObject ball;
    public GameObject startPos;
    public GameObject hole;
    Terrain terrain;
    float biasOfX;
    float biasOfZ;

    public Vector3 userVeloc;
    Vector3 findAnsVeloc;
    public Vector3 stoppedPos;

    //계산하기
    float angle;
    private int count = 0;
    private int numberOfTryToFoundAnswer = 0;
    private int standScore = 10;

    float answerOfAngle;
    List<float> rightAngleList;
    List<float> distanceList = new List<float>();
    List<float> velocList = new List<float>();
    List<float> angleList = new List<float>();
    public float valueOfZ;

    //public GameObject test;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void simulationStart()
    {
        if (stateIsNone) state = 0;  //none 상태
        terrain = gameObject.GetComponent<Terrain>();
        mb.initMoveBall(terrain);
        biasOfX = terrain.transform.position.x;
        biasOfZ = terrain.transform.position.z;
        Debug.Log("simulationStart " + this.name +"/ state: "+state);

        //변수 선언
        //정답 찾기
        stateIsNone = false;
        foundAns = false;
        numberOfSimulation = 0;
        mb.succeed = false;
        mb.finish = false;

        Debug.Log("entering.. " + Random.Range(10,18));

        hole.transform.position = new Vector3(Random.Range(100, 200)+ biasOfX, 3.1f, Random.Range(100, 200)); //Random.range(100, 200), 3.1f, random.Next(100, 200)
        float startPosX = Random.Range(100, 200)+ biasOfX; float startPosZ = Random.Range(100, 200);
        float startPosY = this.GetComponent<Terrain>().terrainData.GetHeight((int)startPosX - (int)biasOfX, (int)startPosZ);
        Debug.Log("시작 높이 " + startPosY);
        startPos.transform.position = new Vector3(startPosX, startPosY, startPosZ);
        findAnsVeloc = new Vector3(0, 0, 20);

        if(angleList.Count>0) angleList.Clear();
        if(distanceList.Count>0) distanceList.Clear();
        if(velocList.Count>0) velocList.Clear();

        while (!stateIsNone)
        {
            run(state);
        } // 자체 loop

        Debug.Log("simulationEnd " + this.name + "/ state: " + state);
    }

    public override void noneState()
    {
        Debug.Log("none");
        if (!stateIsNone) nextState();
    }
    public override void startState()
    {
        Debug.Log("start");
        if (!foundAns)
        {
            //Debug.Log("정답 각도 찾기 "+countRound);
            Debug.Log("찾기 시작");
        }
        ball.transform.position = startPos.transform.position; //원래 뉴 스타트 안에 있었음
        mb.setBallPos(startPos.transform.position);
        nextState();
    }
    public override void readyState()
    {
        Debug.Log("ready");
        if (!foundAns)
        {
            //Debug.Log("못찾음");
            mb.setVelocity(findAnsVeloc);
        }
        else
        {
            mb.setVelocity(userVeloc);
        }
        nextState();
    }
    bool isFirstRoll = true;
    public override void rollState()
    {
        Debug.Log("roll");

        mb.startMovingBall();
        if (mb.finish)
        {
            nextState();
        }
    }
    public override void endState()
    {
        Debug.Log("end");
        stoppedPos = ball.transform.position; // 종료 위치
        // 각도 계산 
        angle = Mathf.Atan2((stoppedPos.z - startPos.transform.position.z), (stoppedPos.x - startPos.transform.position.x)) * Mathf.Rad2Deg
            - Mathf.Atan2((hole.transform.position.z - startPos.transform.position.z), (hole.transform.position.x - startPos.transform.position.x)) * Mathf.Rad2Deg;
        if (!foundAns)
        { //정답 찾기
            if (mb.succeed)
            {
                //홀에 들어갔을 경우
                Debug.Log("들어감");
                foundAns = true;
                answerOfAngle = mb.transform.rotation.z;
                //rightAngleList.Add(mb.transform.rotation.z); // 정답 각도 저장 //1회 -0까지만 있음
            }
            else
            {
                Debug.Log("못 들어감");
                numberOfTryToFoundAnswer++;
                if (numberOfTryToFoundAnswer == 8)
                { //10번 못들어감
                    float startPosX = Random.Range(100, 200); float startPosZ = Random.Range(100, 200);
                    float startPosY = this.GetComponent<Terrain>().terrainData.GetHeight((int)startPosX - (int)biasOfX, (int)startPosZ);
                    Debug.Log("시작 높이 " + startPosY);
                    startPos.transform.position = new Vector3(startPosX, startPosY, startPosZ);

                    numberOfTryToFoundAnswer = 0;
                    angle = 0;
                }
                mb.ChangeAngle(angle); //각도 변경
            }
        }
        else
        { //정답 찾은 후 10번 시뮬레이션
            Debug.Log("count" + count);

            //거리 비율 저장
            distanceList.Add(Mathf.Sqrt(Mathf.Pow(hole.transform.position.x - stoppedPos.x, 2) + Mathf.Pow(hole.transform.position.z - stoppedPos.z, 2)));
            
            float temp = UnityEngine.Random.Range(-5f, 5f);
            userVeloc = new Vector3(0, 0, findAnsVeloc.z + temp);
            temp = UnityEngine.Random.Range(-25f, 25f);
            angle = answerOfAngle + temp;

            velocList.Add(userVeloc.z);
            angleList.Add(angle);

            numberOfSimulation++; //몇번 시뮬레이션 했나

            if (numberOfSimulation == 10)
            { //시뮬레이션 종료
                float avgDistance = 0;
                float deviationOfDistance = 0;
                float sumOfDistance = 0;
                
                avgDistance = distanceList.Average(); //거리 평균 구하기

                for (int i = 0; i < numberOfSimulation; i++)
                {
                    sumOfDistance += Mathf.Pow(distanceList[i] - avgDistance, 2);
                }
                deviationOfDistance = Mathf.Sqrt(sumOfDistance / (numberOfSimulation));

                valueOfZ = Mathf.Abs((standScore - avgDistance) / deviationOfDistance);
                Debug.Log("거리 평균: " + avgDistance + "표준편차:" + deviationOfDistance + " 정규화 " + valueOfZ); //probList[countRound], 10

                //초기화
                //rightAngleList.Clear();
                distanceList.Clear();
                angleList.Clear();
                
                //정답 부터 다시 찾기
                stateIsNone = true; //더이상 시뮬레이션할 필요없다
            }
        }
        //새로 시뮬레이션 try
        mb.turnOffKinematic();
        mb.isColliedGGreen = false;
        mb.finish = false; // 초기화
        nextState();
    }
}
