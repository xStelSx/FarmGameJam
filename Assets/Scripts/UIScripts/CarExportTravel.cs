using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarExportTravel : MonoBehaviour
{
    [SerializeField] private RectTransform objectToMove;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private GlobalTimer globalTimer;
    [SerializeField] private Image objectImage;
    [SerializeField] private QuestManager questManager;

    private bool isAnimating = false;
    private Coroutine animationCoroutine;
    private bool shouldStop = false;
    private Vector2 stopPosition;

    private void Start()
    {
        objectToMove.anchoredPosition = startPosition;
        globalTimer.exportButton.onClick.AddListener(StartAnimation);

        // ������������� �� ������� �������
        if (questManager != null)
        {
            questManager.restartButton.onClick.AddListener(OnGameOver);
        }
    }

    public void OnGameOver()
    {
        shouldStop = true;
    }

    public void StartAnimation()
    {
        if (isAnimating) return;

        shouldStop = false; // ���������� ���� ��������� ��� ����� ������� ��������

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateObject());
    }

    private IEnumerator AnimateObject()
    {
        isAnimating = true;

        float halfDuration = globalTimer.exportCooldown / 2f;
        float elapsedTime = 0f;

        // �������� ������
        while (elapsedTime < halfDuration)
        {
            if (shouldStop)
            {
                // ���������� ������� ������� ��� ���������
                stopPosition = objectToMove.anchoredPosition;
                // ������������� ������
                objectImage.transform.rotation = Quaternion.Euler(0, 180, 0);
                isAnimating = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            objectToMove.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // �������� (������ ���� �� ����� ���������������)
        if (!shouldStop)
        {
            objectImage.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // �������� ����� (������ ���� �� ����� ���������������)
        if (!shouldStop)
        {
            elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                if (shouldStop)
                {
                    stopPosition = objectToMove.anchoredPosition;
                    objectImage.transform.rotation = Quaternion.Euler(0, 0, 0);
                    isAnimating = false;
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                float t = elapsedTime / halfDuration;
                objectToMove.anchoredPosition = Vector2.Lerp(endPosition, startPosition, t);
                yield return null;
            }

            objectImage.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        isAnimating = false;
    }

    // ����� ��� ��������� ������ �� ������� �������
    public void StopCarAtCurrentPosition()
    {
        shouldStop = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (objectToMove != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(objectToMove.parent.TransformPoint(startPosition), 10f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(objectToMove.parent.TransformPoint(endPosition), 10f);
        }
    }
}