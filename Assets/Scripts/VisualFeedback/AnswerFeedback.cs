using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnswerFeedback : MonoBehaviour
{
    [Header("Animation duration")]
    [SerializeField] private float _fadeDuration = 0.6f; 
    [SerializeField] private float _displayDuration = 3f;
    
    [Header("Animation position")]
    [SerializeField] private Transform _uiParentTransform; 
    [SerializeField] private Vector3 _moveOffset = new Vector3(0, 30, 0);
    
    public void ShowAnswerUI(GameObject uiPrefab)
    {
        GameObject uiInstance = Instantiate(uiPrefab, _uiParentTransform);
        CanvasGroup uiCanvasGroup = uiInstance.GetComponent<CanvasGroup>();
        RectTransform rectTransform = uiInstance.GetComponent<RectTransform>();
        
        uiInstance.transform.localPosition = Vector3.zero;
        uiCanvasGroup.alpha = 0;
        Vector2 targetPosition = rectTransform.anchoredPosition + (Vector2)_moveOffset;
        
        Sequence uiSequence = DOTween.Sequence();
        
        uiSequence.Append(uiCanvasGroup.DOFade(1, _fadeDuration).SetEase(Ease.InOutQuad));
        uiSequence.Join(rectTransform.DOAnchorPos(targetPosition, _fadeDuration).SetEase(Ease.OutQuad));
        
        uiSequence.AppendInterval(_displayDuration);
        
        uiSequence.Append(uiCanvasGroup.DOFade(0, _fadeDuration).SetEase(Ease.InOutQuad));

        uiSequence.OnComplete(() => Destroy(uiInstance));
    }
}
