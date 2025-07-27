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

    private bool gameOver = false;

    private void Start()
    {
        
        objectToMove.anchoredPosition = startPosition;

        
        globalTimer.exportButton.onClick.AddListener(StartAnimation);

        if (questManager != null)
        {
            questManager.restartButton.onClick.AddListener(OnGameOver);
        }
    }

    public void OnGameOver()
    {
        gameOver = true;
    }

    public void StartAnimation()
    {
        if (isAnimating) return;

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

        
        while (elapsedTime < halfDuration)
        {
            if (gameOver)
            {
                // ѕри Game Over останавливаемс€ на текущей позиции
                objectImage.transform.rotation = Quaternion.Euler(0, 180, 0);
                isAnimating = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            objectToMove.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        
        objectImage.transform.rotation = Quaternion.Euler(0, 0, 0);

       
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            objectToMove.anchoredPosition = Vector2.Lerp(endPosition, startPosition, t);
            yield return null;
        }

        
        objectImage.transform.rotation = Quaternion.Euler(0, 180, 0);

        isAnimating = false;
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
