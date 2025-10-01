using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{   
    //시네마틱(타임라인) 재생 중에 플레이어 조작을 비활성화하는 클래스
    public class CinematicControlRemover : MonoBehaviour
    {   
        //플레이어 오브젝트 참조
        GameObject player;
        //관찰자(Observer) 패턴

        void Awake()
        {   
            //"Player" 태그가 붙은 오브젝트를 찾아 저장
            player = GameObject.FindWithTag("Player");
        }

        //컴포넌트가 활성화될때 실행
        void OnEnable()
        {   
            //PlayableDirector(타임라인)이 재생될 때 DisableControl 실행
            GetComponent<PlayableDirector>().played += DisableControl;
            //타임라인이 정지될 때 EnableControl 실행
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        //컴포넌트가 비활성화될 때 이벤트 연결 해제
        void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        // 타임라인이 시작될때 호출
        void DisableControl(PlayableDirector pd)
        {   
            //현재 실행중인 행동 취소
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            //플레이어 조작 비활성화
            player.GetComponent<PlayerController>().enabled = false;
            print("DisableControl");
        }

        //타임라인이 끝났을때 호출
        void EnableControl(PlayableDirector pd)
        {   
            //플레이어 조작 활성화
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}