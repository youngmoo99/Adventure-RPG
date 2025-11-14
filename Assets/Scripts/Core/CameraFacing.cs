using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    // 항상 카메라를 바라보게 만드는 빌보드 효과 
    public class CameraFacing : MonoBehaviour
    {
        void LateUpdate()
        {
            // 카메라의 정면 벡터와 동일하게 정면을 맞춘다.
            transform.forward = Camera.main.transform.forward;
        }
    }
}

