using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour
{
    public GameObject restartButton;
    private bool _collisionSet;                                                //Переменная для проверки корректной работы скрипта
    private void OnCollisionEnter(Collision collision)                        //Функция срабатывает при соприкосновение с объектом.
    { 
        if(collision.gameObject.tag == "Cube" && !_collisionSet)            //Если объект имеет тэг Cube
        {
            for (int i = collision.transform.childCount - 1; i >= 0; i--) //Перебираем в цикле все дочерние объекты
            {
                Transform child = collision.transform.GetChild(i);      //Выбираем дочерний элемент по индексу
                child.gameObject.AddComponent<Rigidbody>();            //Добавляем физику для дочерних элементов
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f); //Добавляем взрывную силу
                child.SetParent (null);                              //Убираем родителя для каждого объекта
            }
            restartButton.SetActive(true);                                             //Делаем кнопку рестарт активной
            Camera.main.transform.position -= new Vector3(0, 0, 3f) * Time.deltaTime; //Отдаляем камеру
            Destroy(collision.gameObject);                                           //Удаляем All cubes
            _collisionSet = true;
        }
    }
}
