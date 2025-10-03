using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{   
    // 세이브 파일(딕셔너리)을 디스크에 읽고/쓰며, 씬 전환과 함께 모든 SaveableEntity들의 상태를 캡처/복원하는 핵심 시스템
    public class SavingSystem : MonoBehaviour
    {   
        // 마지막으로 플레이하던 씬을 로드하고, 이어서 상태를 복원하는 코루틴
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);

            // 기본값: 현재 씬, 저장값 있으면 저장된 씬 인덱스로
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            // 씬 로드
            yield return SceneManager.LoadSceneAsync(buildIndex);

            // 오브젝트 상태 복원
            RestoreState(state);
        }

        // 현재 씬의 모든 SaveableEntity 상태를 캡처하여 파일로 저장
        public void Save(string saveFile)
        {   
            // 기존 상태 로드(병합 목적)
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        // 파일에서 읽어 모든 엔티티 상태 복원
        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        // 세이브 파일 삭제
        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        // 디스크에서 세이브 딕셔너리 읽기
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }
        
        // 디스크에 세이브 딕셔너리 쓰기
        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        // 모든 SaveableEntity를 순회하며 상태 캡처
        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            // 마지막으로 플레이하던 씬 인덱스도 함께 저장
            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        // 세이브 딕셔너리를 바탕으로 모든 SaveableEntity 상태 복원
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }
        
        // 세이브 파일 경로(플랫폼별 지속 데이터 폴더 내부)
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}