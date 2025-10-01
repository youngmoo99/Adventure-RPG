using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{   
    // 카메라가 특정 타겟을 따라가도록 하는 클래스
    public class FollowCamera : MonoBehaviour
    {   
        //따라갈 대상(플레이어)
        [SerializeField] Transform target;

        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}
