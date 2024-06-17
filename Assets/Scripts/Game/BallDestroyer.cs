using UniRx;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class BallsDestroyer : MonoBehaviour
    {
        [SerializeField] GameBallTrigger trigger;

        private void Awake()
        {
            trigger.GameBallEntered.Subscribe(x =>
            {
                if (!x) return;

                Destroy(x.gameObject, .1f);
                RemoveScore(x);

            }).AddTo(gameObject);
        }

        public void RemoveScore(GameBallObject o)
        {
            var score = GameManager.Instance.scoreDictionary[o.Color];
            GameManager.Instance.PlayerScore.Value -= score;
        }
    }
}