using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0); // �������� ��� ���������� ����
    public float CubeChangePlaceSpeed = 0.5f;      //�������� �������� ����� ������� ����
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes;
    public Text scoreRecord;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRB;
    private float camMoveToYposition, camMoveSpeed = 2f;
    private bool isLose, firstCube;
    public Color[] bgColors;
    private Color toCameraColor;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,1),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,1),
        new Vector3(1,0,-1),
    };
    private Transform mainCam;
    private Coroutine showCubePlace;
    private int countMaxHor;

    private void Start()
    {
        scoreRecord.text = "<size=90><color=#2D7DF4>������:</color></size>" + PlayerPrefs.GetInt("score") + "\n <size=80> �ר�:</size> 0";
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform; //������� ��������� ������
        camMoveToYposition = 8f + nowCube.y - 1f; //��������� ������� ���������� ������������� ���� �� Y
        allCubesRB = allCubes.GetComponent<Rigidbody>(); //����� ���������
        showCubePlace = StartCoroutine(ShowCubePlace()); //������ ��������
    }
    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) 
            && cubeToPlace != null
            && allCubes != null
            && !EventSystem.current.IsPointerOverGameObject()) //���� ����� UI, �� ������� �� ������� 
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began) //���� ���� �� ����� ������ �������
            {
                return; //������� �� �������
            }
#endif
            if (!firstCube) //���� ������� ���� �� �����
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage) //�� ���������� ��� ������� � �������
                {
                    Destroy(obj);                         //���������� ��� ������� UI
                }
            }

           GameObject newCube = Instantiate
                (cubeToCreate,                                     //������� ���
                cubeToPlace.position,                             //����� �������
                Quaternion.identity) as GameObject;              //����� ��������
            newCube.transform.SetParent(allCubes.transform);    //������ ��������
            nowCube.setVector(cubeToPlace.position);           //������ ����� ������� ����
            allCubesPositions.Add(nowCube.getVector());       //������ ������� �������
            allCubesRB.isKinematic = true;                   //�������� isKinematic
            allCubesRB.isKinematic = false;                 //�������� isKinematic
            SpawnPositions();                              //�������� ����� ������ ����
            MoveCameraChangeBg();                         //�������� ������� ��� �������� ����. xyz
        }

        if (!isLose &&  allCubesRB.velocity.magnitude > 0.1f) //���� ������� ���������
        {
            Destroy(cubeToPlace.gameObject);   //�� ���������� ������
            isLose = true;                    //��� ������������ ���� 1 ���
            StopCoroutine(showCubePlace);    //��������� �������� �� ����������
        }
        //������� ��������� ������ ����� � ����
        mainCam.localPosition = Vector3.MoveTowards (mainCam.localPosition,
           new Vector3 (mainCam.localPosition.x, camMoveToYposition, mainCam.localPosition.z), 
            camMoveSpeed * Time.deltaTime);
        if (Camera.main.backgroundColor != toCameraColor)
        {   // ������ ������ ���� ���� ������
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f );
        }
    }
    IEnumerator ShowCubePlace() //��������� ��������
    {
        while(true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(CubeChangePlaceSpeed); // ��������� ���������� �������
        }
    }
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();                          //������������ ������
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z))  //���� ����� ������
            && nowCube.x + 1 != cubeToPlace.position.x)                       // � �� ����� ����� �� �������
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)); //�� ��������� ���������� 
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z))
            && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z))
            && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z))
            && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1))
            && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1))
            && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        if (positions.Count > 1)
        {
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)]; //���������� �� ���� ��������� ������
        }
        else if(positions.Count == 0) //���� ����� ��������� ���
        {
            isLose = true;          //�� ��������
        }
        else
        {
            cubeToPlace.position = positions[0]; //����� �������� ������������ ��������� �������
        }  
    }
    private bool IsPositionEmpty(Vector3 targetPos) //������� ��� �������� ��������� ���� 
    { 
        if(targetPos.y == 0) // ���� ���������� ���� ��������� 
        {   
            return false; 
        }
        foreach (Vector3 pos in allCubesPositions) //���������� ��� �������� ������ 
        {
            if (pos.x == targetPos.x && pos.y ==targetPos.y && pos.z == targetPos.z)
            {
                return false;
            }
        }
        return true;
    }
    //������� ��� ��������� ������������� �������� �� xyz
    private void MoveCameraChangeBg() //����������� ������ ���, ����� �������� ����� ���
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;
        foreach (Vector3 pos in allCubesPositions)
        {
            if(Mathf.Abs(Convert.ToInt32 (pos.x)) > maxX)
            {
                maxX = Convert.ToInt32(pos.x);
            }
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
            {
                maxZ = Convert.ToInt32(pos.z);
            }
            if (Convert.ToInt32(pos.y) > maxY)
            {
                maxY = Convert.ToInt32(pos.y);
            }
        }
        maxY--;
        if(PlayerPrefs.GetInt("score") < maxY -1) //��������� ������ �������
            PlayerPrefs.SetInt("score", maxY);
            scoreRecord.text = "<size=90><color=#2D7DF4>������:</color></size>" + PlayerPrefs.GetInt("score") + "\n <size=80> �ר�:</size>" + maxY;

        camMoveToYposition = 8f + nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;
        if (maxHor % 3 == 0 && countMaxHor != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f); //���������� ������
            countMaxHor = maxHor;
        }
        if (maxY >= 40) //���� ������ �����
        //������ �����
            toCameraColor = bgColors[3];
        else if (maxY >= 30)
            toCameraColor = bgColors[2];
        else if (maxY >= 20)
            toCameraColor = bgColors[1];
        else if (maxY >= 10)
            toCameraColor = bgColors[0];
        //���������� �������� �����
        else if (maxY >= 40)
            CubeChangePlaceSpeed = 0.2f;
        else if (maxY >= 30) 
            CubeChangePlaceSpeed = 0.3f;
        else if (maxY >= 20)
            CubeChangePlaceSpeed = 0.4f;
        else if (maxY >= 10)
            CubeChangePlaceSpeed = 0.45f;
    }
}

struct CubePos //��������� ���������
{
    public int x, y, z; //���������� ��� ���������
    public CubePos(int x, int y, int z) //��������� �����������
    {
        this.x = x; //������������� ���������� ���������� �� �������
        this.y = y;
        this.z = z;
    }
    public Vector3 getVector() //����� ���������� ������ �� ���������
    {
        return new Vector3(x, y, z);
    } public void setVector(Vector3 pos) //��������� ������ 3
    { //������������ � �� float � int
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
    
}

//�����, �������:  D2ECFF,  BBFFE0, BAFFC0, FFFFFF//