using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{   
    //파티클 이펙트가 끝난 후 오브젝트 제거하는 클래스
    public class DestroyAfterEffect : MonoBehaviour
    {   
        //파괴 대상 지정
        [SerializeField] GameObject targetToDestroy = null;
        void Update()
        {   
            //파티클 살아있는지 확인
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                if (targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

    }
}
