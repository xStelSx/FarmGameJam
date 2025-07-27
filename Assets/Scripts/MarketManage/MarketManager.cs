using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f; // Увеличим скорость для более заметного эффекта
    [SerializeField] private float topYPosition = 0f; // Пример: 0 - позиция на экране
    [SerializeField] private float bottomYPosition = -800f; // Пример: уходит за пределы экрана

    [Header("UI Reference")]
    [SerializeField] private RectTransform movingElement;
    [SerializeField] private CanvasGroup canvasGroup; // Для плавного появления/исчезания

    private bool isMoving = false;
    private bool isOpened = false;
    private Vector2 targetPosition;

    private void Awake()
    {
        if (movingElement == null)
            movingElement = GetComponent<RectTransform>();

        // Добавляем CanvasGroup если нет
        if (canvasGroup == null)
        {
            canvasGroup = movingElement.gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = movingElement.gameObject.AddComponent<CanvasGroup>();
        }

        // Начальная позиция и прозрачность
        movingElement.anchoredPosition = new Vector2(movingElement.anchoredPosition.x, bottomYPosition);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        // Обработка нажатия ПКМ для скрытия UI
        if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            if (!EventSystem.current.IsPointerOverGameObject() || IsPointerOverMovingElement())
            {
                if (isOpened) CloseMarket();
            }
        }

        // Плавное перемещение и изменение прозрачности
        if (isMoving)
        {
            movingElement.anchoredPosition = Vector2.Lerp(
                movingElement.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Плавное изменение прозрачности
            float targetAlpha = isOpened ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, moveSpeed * Time.deltaTime);

            // Проверка достижения цели
            if (Vector2.Distance(movingElement.anchoredPosition, targetPosition) < 1f)
            {
                movingElement.anchoredPosition = targetPosition;
                canvasGroup.alpha = targetAlpha;
                canvasGroup.interactable = isOpened;
                canvasGroup.blocksRaycasts = isOpened;
                isMoving = false;
            }
        }
    }

    public void OpenMarket()
    {
        if (isOpened) return;

        isOpened = true;
        isMoving = true;
        targetPosition = new Vector2(movingElement.anchoredPosition.x, topYPosition);

        // Включаем взаимодействие сразу при начале открытия
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseMarket()
    {
        if (!isOpened) return;

        isOpened = false;
        isMoving = true;
        targetPosition = new Vector2(movingElement.anchoredPosition.x, bottomYPosition);

        // Отключаем взаимодействие сразу при начале закрытия
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private bool IsPointerOverMovingElement()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Exists(r => r.gameObject == movingElement.gameObject);
        }
        return false;
    }
}