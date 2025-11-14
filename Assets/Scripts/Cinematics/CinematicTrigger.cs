using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{   
    // 특정 영역에 들어가면 시네마틱을 자동으로 재생하는 클래스
    public class CinematicTrigger : MonoBehaviour
    {   
        // 이미 시네마틱을 실행했는지 여부
        bool alreadyTriggered = false;
        
        // 플레이어가 Trigger Collider 트리거에 들어왔을 때 호출
        private void OnTriggerEnter(Collider other)
        {   
            // 아직 실행되지 않았고 들어온 객체가 Player인 경우
            if (!alreadyTriggered && other.gameObject.tag == "Player")
            {   
                // 중복 재생 방지
                alreadyTriggered = true;

                // 현재 오브젝트에 붙은 PlayableDirector(타임라인) 실행
                GetComponent<PlayableDirector>().Play();
            }

        }

    }
}

