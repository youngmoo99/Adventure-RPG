using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{   
    // 파티클 이펙트가 모두 끝나면 지정한 대상(혹은 자기 자신)을 파괴하는 컴포넌트
    public class DestroyAfterEffect : MonoBehaviour
    {   
        // 파괴 대상 지정
        [SerializeField] GameObject targetToDestroy = null;
        void Update()
        {   
            //파티클 살아있는지 확인
            if (!GetComponent<ParticleSystem>().IsAlive())
            {   
                // 대상을 지정했으면 그 오브젝트를 제거
                if (targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                // 아니면 자기 자신을 제거
                else
                {
                    Destroy(gameObject);
                }
            }
        }

    }
}
