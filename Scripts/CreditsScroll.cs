using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private float scrollDuration;

    private void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1;
        StartCoroutine(AutoScroll(1, 0, scrollDuration));
    }

    IEnumerator AutoScroll(float startPos, float endPos, float duration)
    {
        yield return new WaitForSeconds(0.5f);

        float to = 0.0f;
        while (to<1f)
        {
            to += Time.deltaTime / duration;
            scrollRect.verticalNormalizedPosition=Mathf.Lerp(startPos, endPos, to);
            yield return null;
        }
    }
}
