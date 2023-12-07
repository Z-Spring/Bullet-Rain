using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Transform gameOverCanvas;
        [SerializeField] private Button switchViewButton;
        [SerializeField] private Button gameOverButton;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private Camera mainCamera;

        private void Start()
        {
            gameOverCanvas.gameObject.SetActive(false);
            GameManager.Instance.OnGameOver.AddListener(GameManager_OnGameOver);    
            gameOverButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
            switchViewButton.onClick.AddListener(() =>
            {
                gameOverCanvas.gameObject.SetActive(false);
                mainCamera.enabled = true;
            });
        }
        

        private void GameManager_OnGameOver()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameOverCanvas.gameObject.SetActive(true);
            
            gameOverText.text = "Game Over";
            GameManager.Instance.OnGameOver.RemoveListener(GameManager_OnGameOver);
        }
        
    }
}