using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{

    public RectTransform uiElement;
    public float speed = 1f;
    public float upperY = -290f;
    public float lowerY = -490f;
    private bool movingUp = true; // Переменная для отслеживания направления движения

    public void OpenMarket()
    {
        StartCoroutine(MoveCoroutine(upperY));
    }

    private IEnumerator MoveCoroutine(float targetY)
    {
        while (true)
        {

            while (Mathf.Abs(uiElement.anchoredPosition.y - targetY) > 0.01f)
            {
                float newY = Mathf.MoveTowards(uiElement.anchoredPosition.y, targetY, speed * Time.deltaTime);
                uiElement.anchoredPosition = new Vector2(uiElement.anchoredPosition.x, newY);
                yield return null;
            }

            movingUp = !movingUp;

            yield break;
        }
    }
    
    private void Update()
    {
        // Проверяем нажатие правой кнопки мыши
        if (Input.GetMouseButtonDown(1)) // 1 - правая кнопка мыши
        {
            // Запускаем корутину для перемещения вниз
            StartCoroutine(MoveCoroutine(lowerY));
        }
    }

}
