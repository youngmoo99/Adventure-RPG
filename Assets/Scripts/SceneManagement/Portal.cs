using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    // 포탈에 들어가면 다른 씬으로 전환하는 컴포넌트 
    // 같은 'destination' 값을 가진 포탈끼리 대응
    public class Portal : MonoBehaviour
    {
        // 씬 간 포탈 매칭 식별자
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }
        // 로드할 씬의 인덱스
        [SerializeField] int sceneToLoad = -1;
        // 도착 씬에서 플레이어를 배치할 위치/회전
        [SerializeField] Transform spawnPoint;
        // 이 포탈의 목적지 키
        [SerializeField] DestinationIdentifier destination;
        // 페이드 아웃 시간
        [SerializeField] float fadeOutTime = 1f;
        // 페이드 인 시간
        [SerializeField] float fadeInTime = 2f;
        // 로드 직후 대기 시간(세이브/배치 안정화)
        [SerializeField] float fadeWaitTime = 0.5f;

        // 플레이어가 포탈 트리거에 들어오면 전환 시작
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        // 씬 전환 전체 시퀀스
        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            // 포탈 자신은 전환 동안 파괴되지 않도록 유지(페이드/저장 절차 보호)
            DontDestroyOnLoad(gameObject);

            // 화면 어둡게
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            // 현재 씬 저장
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            // 목표 씬 로드(비동기)
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            // 로드 직후 저장 불러오기(씬 내 오브젝트 상태 복원)
            wrapper.Load();

            // 같은 destination을 가진 '반대편 포탈'을 찾아 플레이어 위치 갱신
            Portal otherPortal = GetOtherPortal();
            // NavMeshAgent 안전 텔레포트
            UpdatePlayer(otherPortal);
            
            // 위치 반영 후 한 번 더 저장
            wrapper.Save();

            // 약간 대기후 화면 밝게
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            // 임시 포탈 오브젝트 정리
            Destroy(gameObject);
        }
        
        // 플레이어를 otherPortal의 SpawnPoint로 순간이동
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        // 같은 destination을 가진 다른 포탈을 찾음
        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}