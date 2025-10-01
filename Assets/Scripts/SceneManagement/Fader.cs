using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    // 화면 페이드 인/아웃을 담당하는 컴포넌트
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

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
        public IEnumerator FadeOut(float time)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        // 서서히 밝게 : time초 동안
        public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}