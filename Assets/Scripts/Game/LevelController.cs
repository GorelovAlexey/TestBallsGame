using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GameBallSpawner spawner;
        [SerializeField] private ChainController chain;
        [SerializeField] private BallsCollector collector;

        private GameBallObject currentBall;
        public void SetupBall()
        {
            currentBall = spawner.SpawnBall();
            chain.AttachBall(currentBall);
        }

        public void Awake()
        {
            collector.BallArrival.Subscribe(x =>
            {
                if (currentBall && currentBall == x)
                {
                    currentBall = null; // getting new ball;
                }

            }).AddTo(gameObject);

            collector.GameStuck.Subscribe(x =>
            {
                if (x == true)
                    GameManager.Instance.FinishGame();

            }).AddTo(gameObject);

        }

        public void ReleaseBall()
        {
            chain.ReleaseBall();
        }

        const float TIME_TO_LET_GO = 0.5f;
        private float minSpeed = .01f;
        private float timeWhenLetGo = 0;
        public void Update()
        {
            BallUpdate();
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ReleaseBall();
            }
        }

        private void BallUpdate()
        {
            if (currentBall)
            {
                if (currentBall.gameObject.activeSelf == false)
                    return;
                
                if (currentBall.Rigidbody2D.velocity.magnitude > minSpeed)
                    timeWhenLetGo = Time.time + TIME_TO_LET_GO;

                if (Time.time > timeWhenLetGo)
                    currentBall = null;
            }
            else
            {
                SetupBall();
            }
        }

        public void Restart()
        {
            spawner.ClearBalls();
            chain.Restart();
            SetupBall();

            GameManager.Instance.SetOnScrenClick(x => ReleaseBall());
        }
    }
}