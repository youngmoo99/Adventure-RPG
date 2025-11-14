using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    // 손에 장착된 무기 오브젝트가 가지는 훅(이벤트) 컨테이너
    // 애미네이션 이벤트 Hit() 시점 등에서 호출되어 사운드/이펙트 트리거 가능
    public class Weapon : MonoBehaviour
    {   
        // 피격 타이밍에 실행할 이벤트
        [SerializeField] UnityEvent onHit;
        
        // Fighter.Hit() 등에서 호출됨
        public void OnHit()
        {
            onHit.Invoke();
        }

    }

}