using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 15f; // Скорость вращения камеры.
    private Transform _rotator;
    private void Start() => _rotator = GetComponent<Transform>();            // Получаем компонент Трансформ
    private void Update() => _rotator.Rotate(0, speed * Time.deltaTime, 0); // Задаем скорость врашение в компоненте 
}
