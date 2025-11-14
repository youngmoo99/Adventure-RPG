using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    // 적 위에 표시되는 체력 바 UI
    // 체력이 꽉 찼거나 0이면 비활성화 하여 UI 노이즈 줄임
    // 체력 변화에 따라 바의 X스케일을 조정
    public class HealthBar : MonoBehaviour
    {   
        // 연결된 Health 컴포넌트
        [SerializeField] Health healthComponent = null;
        // 빨간색(전경) 체력바 부분
        [SerializeField] RectTransform foreground = null;
        // 전체 체력바 캔버스
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {   
            // 체력이 0이거나 100%이면 표시하지 않음
            if (Mathf.Approximately(healthComponent.GetFraction(), 0) || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            // 체력 변동 시 캔버스 활성화
            rootCanvas.enabled = true;

            // 체력 비율(0~1)에 따라 x축 스케일 변경
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }

    }
}
