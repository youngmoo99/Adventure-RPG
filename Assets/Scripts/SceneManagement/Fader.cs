using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    // 화면 페이드 인/아웃을 담당하는 컴포넌트
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // 즉시 화면을 검게만듬. (씬 로드 직후 초기화 용)
        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        // 서서히 어둡게 : time초 동안
        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        // 서서히 밝게 : time초 동안
        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}