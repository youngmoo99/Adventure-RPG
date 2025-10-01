using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    // 세이브/로드의 진입점 래퍼
    // - 게임 시작 시 마지막 씬 로드
    // - 페이드로 자연스럽게 화면 전환
    // - 단축기(S/L/Delete)로 세이브/로드/삭제
    public class SavingWrapper : MonoBehaviour
    {   
        // 기본 세이브 파일 이름
        const string defaultSaveFile = "save"; 
        // 시작 페이드인 시간
        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {   
            // 게임 시작 즉시 마지막 씬을 로드하는 코루틴 시작
            StartCoroutine(LoadLastScene());
        }

        // 마지막 씬 로드 -> 즉시 검은 화면 -> 페이드인
        private IEnumerator LoadLastScene()
        {   
            // 세이브된 씬과 그 씬의 상태를 로드
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);

            // 화면을 먼저 검게 만든 후
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            // 천천히 밝게
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {   
            // 저장 S
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            // 로드 L
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            // 삭제 Delete
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}