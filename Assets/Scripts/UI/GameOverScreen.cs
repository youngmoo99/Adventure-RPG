using RPG.Attributes;
using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI
{
    // 플레이어 사망 시 게임오버 스크린을 표시하고 재시작 기능을 제공하는 컴포넌트
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] GameObject gameOverPanel = null; // 게임오버 패널 UI

        Health playerHealth;
        SavingWrapper savingWrapper;
        bool hasShownGameOver = false;

        void Awake()
        {
            // SavingWrapper 찾기
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        void Start()
        {
            // 플레이어 Health 컴포넌트 찾기 (Start에서 찾으면 씬 로드 후에도 작동)
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }

            // 시작 시 게임오버 패널 숨기기
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        void Update()
        {
            // 플레이어가 죽었고 아직 게임오버 스크린을 표시하지 않았으면 표시
            if (playerHealth != null && playerHealth.IsDead() && !hasShownGameOver)
            {
                ShowGameOverScreen();
                hasShownGameOver = true;
            }
            
            // 플레이어가 다시 살아났으면 플래그 리셋 (재시작 후)
            if (playerHealth != null && !playerHealth.IsDead() && hasShownGameOver)
            {
                hasShownGameOver = false;
            }
        }

        // 게임오버 스크린 표시
        void ShowGameOverScreen()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        // RESTART 버튼 클릭 시 호출
        public void RestartGame()
        {
            Debug.Log("RestartGame called");
            
            // SavingWrapper를 다시 찾기 (씬 전환 후에도 작동하도록)
            if (savingWrapper == null)
            {
                savingWrapper = FindObjectOfType<SavingWrapper>();
            }
            
            if (savingWrapper != null)
            {
                Debug.Log("SavingWrapper found, restarting game");
                
                // 게임오버 패널 숨기기
                if (gameOverPanel != null)
                {
                    gameOverPanel.SetActive(false);
                }
                hasShownGameOver = false;
                
                // 저장된 씬 다시 불러오기
                savingWrapper.RestartGame();
            }
            else
            {
                Debug.LogError("SavingWrapper not found!");
            }
        }
    }
}

