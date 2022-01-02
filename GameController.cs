using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0); // Значения для начального куба
    public float CubeChangePlaceSpeed = 0.5f;      //Значения скорости смены позиции куба
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
        scoreRecord.text = "<size=90><color=#2D7DF4>РЕКОРД:</color></size>" + PlayerPrefs.GetInt("score") + "\n <size=80> СЧЁТ:</size> 0";
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform; //Берется трансформ камеры
        camMoveToYposition = 8f + nowCube.y - 1f; //Добавляем позицию последнего поставленного куба по Y
        allCubesRB = allCubes.GetComponent<Rigidbody>(); //Берем компонент
        showCubePlace = StartCoroutine(ShowCubePlace()); //Запуск куратино
    }
    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) 
            && cubeToPlace != null
            && allCubes != null
            && !EventSystem.current.IsPointerOverGameObject()) //Если задет UI, то выходим из функции 
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began) //Если фаза не равна началу касания
            {
                return; //выходим из функции
            }
#endif
            if (!firstCube) //Если касание было не экран
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage) //То перебираем все объекты в массиве
                {
                    Destroy(obj);                         //Уничтожаем все объекты UI
                }
            }

           GameObject newCube = Instantiate
                (cubeToCreate,                                     //Создаем куб
                cubeToPlace.position,                             //Берем позицию
                Quaternion.identity) as GameObject;              //Берем вращение
            newCube.transform.SetParent(allCubes.transform);    //Задаем родителя
            nowCube.setVector(cubeToPlace.position);           //Задаем новую позицию куба
            allCubesPositions.Add(nowCube.getVector());       //Меняем занятую позицию
            allCubesRB.isKinematic = true;                   //Включаем isKinematic
            allCubesRB.isKinematic = false;                 //Включаем isKinematic
            SpawnPositions();                              //Вызываем метод спавна куба
            MoveCameraChangeBg();                         //Вызываем функцию для проверки макс. xyz
        }

        if (!isLose &&  allCubesRB.velocity.magnitude > 0.1f) //Если имеются колебания
        {
            Destroy(cubeToPlace.gameObject);   //То уничтожаем объект
            isLose = true;                    //Для срабатывания кода 1 раз
            StopCoroutine(showCubePlace);    //Остановка куратино по переменной
        }
        //Плавное перещение камеры вверх и вниз
        mainCam.localPosition = Vector3.MoveTowards (mainCam.localPosition,
           new Vector3 (mainCam.localPosition.x, camMoveToYposition, mainCam.localPosition.z), 
            camMoveSpeed * Time.deltaTime);
        if (Camera.main.backgroundColor != toCameraColor)
        {   // Плавно меняем цвет фона камеры
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f );
        }
    }
    IEnumerator ShowCubePlace() //Создается куратино
    {
        while(true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(CubeChangePlaceSpeed); // установка промежутка времени
        }
    }
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();                          //Динамический массив
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z))  //Если место пустое
            && nowCube.x + 1 != cubeToPlace.position.x)                       // и не равно своей же позиции
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)); //То добавляем координаты 
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
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)]; //Обращаемся ко всем доступным местам
        }
        else if(positions.Count == 0) //Если негде поставить куб
        {
            isLose = true;          //То проигрыш
        }
        else
        {
            cubeToPlace.position = positions[0]; //Иначе выбираем единственную возможную позицию
        }  
    }
    private bool IsPositionEmpty(Vector3 targetPos) //Функция для проверки заняности мест 
    { 
        if(targetPos.y == 0) // если координата ниже платформы 
        {   
            return false; 
        }
        foreach (Vector3 pos in allCubesPositions) //Перебираем все элементы внутри 
        {
            if (pos.x == targetPos.x && pos.y ==targetPos.y && pos.z == targetPos.z)
            {
                return false;
            }
        }
        return true;
    }
    //Функция для получения максимального элемента по xyz
    private void MoveCameraChangeBg() //Срабатывает каждый раз, когда ставится новый куб
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
        if(PlayerPrefs.GetInt("score") < maxY -1) //Установка нового рекорда
            PlayerPrefs.SetInt("score", maxY);
            scoreRecord.text = "<size=90><color=#2D7DF4>РЕКОРД:</color></size>" + PlayerPrefs.GetInt("score") + "\n <size=80> СЧЁТ:</size>" + maxY;

        camMoveToYposition = 8f + nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;
        if (maxHor % 3 == 0 && countMaxHor != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f); //Отодвигаем камеру
            countMaxHor = maxHor;
        }
        if (maxY >= 40) //Если высота кубов
        //Меняем цвета
            toCameraColor = bgColors[3];
        else if (maxY >= 30)
            toCameraColor = bgColors[2];
        else if (maxY >= 20)
            toCameraColor = bgColors[1];
        else if (maxY >= 10)
            toCameraColor = bgColors[0];
        //увлечиваем скорость смены
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

struct CubePos //Создается структура
{
    public int x, y, z; //Переменные для координат
    public CubePos(int x, int y, int z) //Создается конструктор
    {
        this.x = x; //Присваивается переменным координаты из функции
        this.y = y;
        this.z = z;
    }
    public Vector3 getVector() //Метод возвращает вектор из координат
    {
        return new Vector3(x, y, z);
    } public void setVector(Vector3 pos) //Принимает вектор 3
    { //Конвертируем в из float в int
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
    
}

//Цвета, оснвной:  D2ECFF,  BBFFE0, BAFFC0, FFFFFF//