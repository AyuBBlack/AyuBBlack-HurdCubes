using UnityEngine;
using UnityEngine.SceneManagement;                                              //Подключаем библиотеку менеджера сцен

public class CanvasButtons : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Загрузка активной сцены

    }
    public void LoadInstagram()
    {
        Application.OpenURL("https://www.instagram.com/ayub_gadiev/");
    } 
}
