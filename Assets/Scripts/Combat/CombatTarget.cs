using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Control;


namespace RPG.Combat
{
    // 전투 대상임을 표시하는 마커 컴포넌트
    // 반드시 Health가 함께 붙어 있어야함(공격/피격 전제)
    // IRaycastable 구현으로 마우스 인터렉션(커서/클릭) 처리
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {   
        // 전투 대상 위에 마우스를 올렸을 때 사용할 커서 종류 반환
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        // 레이캐스트가 이 대상에 맞았을 때의 처리 
        // true를 반환하면 다른 상호작용(예 : 이동) 로직은 막힘
        public bool HandleRaycast(PlayerController callingController)
        {
            // 공격 가능 여부 검사(사거리/ 이동 가능 여부/ 대상 생존 등)
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            // 좌클릭 시 공격 명령
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            
            // 전투 상호작용 대상이므로 true 반환(이동 로직 막기)
            return true;
        }
    }
}

