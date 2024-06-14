using System;
using System.Drawing;
using UniRx;
using UnityEngine;
using UnityEngine.tvOS;
using static UnityEngine.ParticleSystem;

namespace Assets.Scripts.Game
{
    public class BallsCollector : MonoBehaviour
    {
        [SerializeField] private GameBallTrigger leftTrigger;
        [SerializeField] private GameBallTrigger middleTrigger;
        [SerializeField] private GameBallTrigger rightTrigger;

        [Space]
        [SerializeField] private ParticleSystem redParticles;
        [SerializeField] private ParticleSystem greenParticles;
        [SerializeField] private ParticleSystem blueParticles;

        // |0.0|-|2.0|->
        // |   1.1   |
        // |0.2| |2.2|
        // V
        private GameBallObject[,] gameField = new GameBallObject[3, 3];

        public ReactiveProperty<bool> GameStuck { get; private set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<GameBallObject> BallArrival { get; private set; } = new ReactiveProperty<GameBallObject>();

        public string top;
        public string mid;
        public string bot;

        private void Awake()
        {
            leftTrigger.GameBallEntered.Subscribe(b =>
            {
                BallArrival.SetValueAndForceNotify(b);
                AddBall(b, 0);
                FieldCheck();

            }).AddTo(gameObject);

            middleTrigger.GameBallEntered.Subscribe(b =>
            {
                BallArrival.SetValueAndForceNotify(b);
                AddBall(b, 1);
                FieldCheck();

            }).AddTo(gameObject);


            rightTrigger.GameBallEntered.Subscribe(b =>
            {
                BallArrival.SetValueAndForceNotify(b);
                AddBall(b, 2);
                FieldCheck();

            }).AddTo(gameObject);
        }

        private void DebugShowBalls()
        {
            var strings = new string[3] { "", "", "" };
            for(var y = 0; y < gameField.GetLength(1); y++)
            {
                for (var x = 0; x < gameField.GetLength(0); x++)
                {
                    if (gameField[x, y] == null)
                    {
                        strings[y] += "[NULL]";
                        continue;
                    }

                    strings[y] += $"{gameField[x, y].Color}";
                }
            }
            top = strings[0];
            mid = strings[1];
            bot = strings[2];
        }

        private void AddBall(GameBallObject ball, int rowX)
        {
            if (!ball)
                return;

            for (var y = gameField.GetLength(1) - 1; y >= 0; y--)
            {
                if (gameField[rowX, y])
                    continue;

                gameField[rowX, y] = ball;
                return;
            }


            DebugShowBalls();
            // DESTROY BALL
        }

        private void FieldCheck()
        {
            while (CollectAndRemoveBalls())
            {
                FallDown();
            }
            DebugShowBalls();

            if (IsFull())
                GameStuck.SetValueAndForceNotify(true);
        }

        public bool IsFull()
        {
            for (var x = 0; x < gameField.GetLength(0); x++)
            {
                for (var y = 0; y < gameField.GetLength(1); y++)
                {
                    if (gameField[x, y] == null)
                        return false;
                }
            }
            return true;
        }

        private void FallDown()
        {
            for (var x = 0; x < gameField.GetLength(0); x++)
            {
                for (var y = gameField.GetLength(1) - 1; y > 0; y--)
                {
                    if (gameField[x, y] != null)
                        continue;

                    if (gameField[x, y - 1] == null)
                        continue;

                    gameField[x, y] = gameField[x, y - 1];
                }
            }
        }

        private void AnimateBallRemoval(GameBallObject[] balls)
        {
            if (balls == null)
                return;

            foreach (var b in balls)
            {
                GameManager.Instance.LauchGameBallAnimation(b);
                Destroy(b.gameObject);

                var emitParams = new EmitParams();
                emitParams.position = b.transform.position;

                ParticleSystem clone = null;
                switch (b.Color)
                {
                    case BallColor.Blue:
                        clone = Instantiate(blueParticles, redParticles.transform.parent);
                        break;
                    case BallColor.Red:
                        clone = Instantiate(redParticles, redParticles.transform.parent);
                        break;
                    case BallColor.Green:
                        clone = Instantiate(greenParticles, redParticles.transform.parent);
                        break;
                }

                clone.transform.position = b.transform.position;
                clone.Play();
            }
        }

        float nextTime;
        private void Update()
        {
            
        }

        private bool CollectAndRemoveBalls()
        {
            var removedSomething = false;
            for (var x = 0; x < gameField.GetLength(0); x++)
            {
                var removed = CheckAndRemove(x, 0, 3, (x1, y1) => (x1, y1 + 1));
                if (removed != null) removedSomething = true;
                AnimateBallRemoval(removed);
            }

            for (var y = 0; y < gameField.GetLength(1); y++)
            {
                var removed = CheckAndRemove(0, y, 3, (x1, y1) => (x1 + 1, y1));
                if (removed != null) removedSomething = true;
                AnimateBallRemoval(removed);
            }

            var diagonal1 = CheckAndRemove(0, 0, 3, (x1, y1) => (x1 + 1, y1 + 1));
            if (diagonal1 != null) removedSomething = true;
            AnimateBallRemoval(diagonal1);

            var diagonal2 = CheckAndRemove(2, 0, 3, (x1, y1) => (x1 - 1, y1 + 1));
            if (diagonal2 != null) removedSomething = true;
            AnimateBallRemoval(diagonal2);

            return removedSomething;

            GameBallObject[] CheckAndRemove(int x, int y, int steps, Func<int, int, (int, int)> func)
            {
                if (!CheckSameColor(x, y, steps, func))
                    return null;

                return RemoveFromField(x, y, steps, func);
            }

            GameBallObject[] RemoveFromField(int x, int y, int steps, Func<int, int, (int, int)> func)
            {
                var arr = new GameBallObject[steps];
                for (var i = 0; i < steps; i++)
                {
                    arr[i] = gameField[x, y];
                    gameField[x, y] = null;

                    (x, y) = func(x, y);
                }
                return arr;
            }

            bool CheckSameColor(int x, int y, int steps, Func<int, int, (int, int)> func)
            {
                if (!gameField[x, y])
                    return false;

                var color = gameField[x, y].Color;
                steps--;
               
                while (steps > 0)
                {
                    (x, y) = func(x, y);

                    if (!gameField[x, y] || gameField[x, y].Color != color)
                        return false;

                    steps--;
                }

                return true;
            }
        }
    }
}