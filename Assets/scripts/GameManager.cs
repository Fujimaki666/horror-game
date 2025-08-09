using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Phase Management")]
    public GamePhase currentPhase = GamePhase.Patrol;

    [Header("References")]
    public PlayerHealth player;
    public Charahealth enemy;
    public ChaseTarget chaseTarget;
    public Navigation navigation;
    public ScareTriggerUI scareUI;
    public GameObject playerSlider;
    public GameObject enemySlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // シーン遷移対応
    }

    void Start()
    {
        SetPhase(GamePhase.Patrol);
    }

    public void SetPhase(GamePhase phase)
    {
        if (currentPhase == phase) return;

        currentPhase = phase;
        Debug.Log($"[GameManager] フェーズ切り替え: {phase}");

        switch (phase)
        {
            case GamePhase.Patrol:
                navigation.enabled = true;
                chaseTarget.ClearTarget();
                enemySlider.SetActive(true);
                playerSlider.SetActive(false);
                break;

            case GamePhase.Scare:
                navigation.enabled = false;
                enemySlider.SetActive(true);
                playerSlider.SetActive(false);
                break;

            case GamePhase.Chase:
                navigation.enabled = false;
                enemySlider.SetActive(false);
                playerSlider.SetActive(true);
                break;

            case GamePhase.GameOver:
                Time.timeScale = 0;
                Debug.Log("ゲームオーバー！");
                break;

            case GamePhase.Clear:
                Time.timeScale = 0;
                Debug.Log("クリア！");
                break;
        }
    }

    public void OnPlayerDamaged(int hp)
    {
        if (hp <= 0)
        {
            SetPhase(GamePhase.GameOver);
        }
    }

    public void OnEnemyDamaged(int hp)
    {
        if (hp <= 0)
        {
            SetPhase(GamePhase.Clear);
        }
    }
}
