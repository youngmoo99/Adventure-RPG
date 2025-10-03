using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Saving
{
    // 씬안의 개별 오브젝트에 대해 "고유 식별자"를 부여하고 그 오브젝트에 붙은 ISaveable 컴포넌트들의 상태를 묶어서 저장/복원하는 허브 역할
    // 에디터 모드에서 동작(고유 ID 부여)
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {   
        // 오브젝트를 전역에서 식별하는 키(세이브 파일의 딕셔너리 키로 사용)
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        // 외부에서 ID 접근(세이브 시스템에서 사용)
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        // 이 엔티티에 붙은 모든 ISaveable의 상태를 딕셔너리로 묶어서 반환
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {   
                // 타입 이름을 키로 하여 충돌 없이 저장
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        // 딕셔너리로부터 각 ISaveable 타입의 상태를 꺼내어 복원
        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

#if UNITY_EDITOR
        // 에디터에서 항상 고유 id를 유지/검증
        private void Update() {
            // 플레이 중엔 건드리지 않음
            if (Application.IsPlaying(gameObject)) return;
            // 프리팹 스테이지 등은 제외
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            // SerializedObject를 통해 Undo/리네임 등 에디터 작업과 호환
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            // 비어있거나 중복이면 새 GUID 발급
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            // 전역 룩업에 반영
            globalLookup[property.stringValue] = this;
        }
#endif

        // 후보 ID가 전역에서 유일한지 검사
        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            // 맵에 있는데 참조가 끊겼다면 정리
            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            // 맵에 있는 엔티티의 ID가 갱신되었다면 정리
            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }
            // 위 조건에 걸리지 않으면 중복
            return false;
        }
    }
}