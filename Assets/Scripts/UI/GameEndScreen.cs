using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameEndScreen : SuperSimpleWindowBase
    {
        [SerializeField] private Button btnExit;
        [SerializeField] private Button btnMenu;
        [SerializeField] private Button btnRestart;
        [SerializeField] private TMP_Text scoreText;

        public void InitScore(int score)
        {
            scoreText.text = $"Your score {score}";
        }

        public override void OnShow()
        {
            InitScore(GameManager.Instance.PlayerScore.Value);
        }

        public override void OnAwake()
        {
            base.OnAwake();

            btnExit.onClick.AddListener(() => Application.Quit());
            btnRestart.onClick.AddListener(() => {
                Hide();
                GameManager.Instance.RestartGame();
            });

            btnMenu.onClick.AddListener(() => {
                Hide();
                GameManager.Instance.SetStateMenu();
            });
        }
    }
}