using UnityEngine;
using RPG.Core;
using RPG.Resources;


namespace RPG.Combat
{
    //전투 대상임을 표시하는 마커 컴포넌트
    // RequireComponent: 반드시 Health가 함께 붙어 있어야함
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {

    }
}

