using DG.Tweening;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UniRx;
using UnityEngine;
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

        public ReactiveProperty<bool> GameStuck { get; private set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<GameBallObject> BallArrival { get; private set; } = new ReactiveProperty<GameBallObject>();

        private void Awake()
        {
            Observable.Merge(leftTrigger.GameBallEntered, middleTrigger.GameBallEntered, rightTrigger.GameBallEntered).Subscribe(b =>
            {
                BallArrival.SetValueAndForceNotify(b);
                ScheduleFieldCheck();

            }).AddTo(gameObject);
        }

        // GAME FIELD STRUCTURE
        // |0.0|-|2.0|->
        // |   1.1   |
        // |0.2| |2.2|
        // V
        private GameBallObject[,] ballsInsideCollector = new GameBallObject[3, 3];
        private bool[,] ballRemoveMask = new bool[3, 3];

        private void FieldCheck()
        {
            var gameField = CalculateBallsInsideCollector(ballsInsideCollector);
            var removeMask = Clear(ballRemoveMask);
            CalculateRemoveMask(gameField, removeMask);
            AnimateBallRemovalAndCheckFull(gameField, removeMask, out var isFull, out var somethingRemoved);

            if (isFull)
                GameStuck.Value = isFull;

            if (somethingRemoved)
                ScheduleFieldCheck();
        }

        private bool[,] Clear(bool[,] arr)
        {
            var width = arr.GetLength(0);
            var height = arr.GetLength(1);
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    arr[i, j] = false;
                }
            }
            return arr;
        }

        private GameBallObject[,] CalculateBallsInsideCollector(GameBallObject[,] ballsInsideCollector)
        {
            FillGameBallColumn(ballsInsideCollector, 0, leftTrigger.GetBallPlacements2());
            FillGameBallColumn(ballsInsideCollector, 1, middleTrigger.GetBallPlacements2());
            FillGameBallColumn(ballsInsideCollector, 2, rightTrigger.GetBallPlacements2());
            return ballsInsideCollector;
        }

        private void FillGameBallColumn(GameBallObject[,] field, int x, List<GameBallObject> ballsInsideTrigger)
        {
            var i2 = ballsInsideTrigger.Count - 1;
            for (var i = field.GetLength(1) - 1; i >= 0; i--)
            {
                if (i2 < 0)
                    field[x, i] = null;

                while (i2 >= 0 && !ballsInsideTrigger[i2])
                    i2--;

                if (i2 < 0)
                    field[x, i] = null;
                else
                {
                    field[x, i] = ballsInsideTrigger[i2];
                    i2--;
                }
            }
        }

        private void CalculateRemoveMask(GameBallObject[,] ballsField, bool[,] ballRemoveMask)
        {
            var width = ballsField.GetLength(0);
            var height = ballsField.GetLength(1);

            // Column check
            for (var x = 0; x < width; x++)
            {
                if (!ballsField[x, 0])
                    continue;

                var col = ballsField[x, 0].Color;
                var allSameColor = true;
                for (var y = 1; y < height; y++)
                {
                    if (!ballsField[x, y] || ballsField[x, y].Color != col)
                        allSameColor = false;
                }

                if (!allSameColor)
                    continue;

                for (var y = 0; y < height; y++)
                    ballRemoveMask[x, y] = true;
            }

            // Rows check
            for (var y = 0; y < height; y++)
            {
                if (!ballsField[0, y])
                    continue;

                var col = ballsField[0, y].Color;
                var allSameColor = true;
                for (var x = 1; x < width; x++)
                {
                    if (!ballsField[x, y] || ballsField[x, y].Color != col)
                        allSameColor = false;
                }

                if (!allSameColor)
                    continue;

                for (var x = 0; x < width; x++)
                    ballRemoveMask[x, y] = true;
            }

            if (ballsField[0, 0] && ballsField[1, 1] && ballsField[2, 2]
                && ballsField[0, 0].Color == ballsField[1, 1].Color && ballsField[1, 1].Color == ballsField[2, 2].Color)
            {
                var col = ballsField[1, 1].Color;
                ballRemoveMask[0, 0] = true;
                ballRemoveMask[1, 1] = true;
                ballRemoveMask[2, 2] = true;
            }

            if (ballsField[0, 2] && ballsField[1, 1] && ballsField[2, 0]
                && ballsField[0, 2].Color == ballsField[1, 1].Color && ballsField[1, 1].Color == ballsField[2, 0].Color)
            {
                var col = ballsField[1, 1].Color;
                ballRemoveMask[0, 2] = true;
                ballRemoveMask[1, 1] = true;
                ballRemoveMask[2, 0] = true;
            }
        }

        private void AnimateBallRemoval(GameBallObject ball)
        {
            if (!ball)
                return;

            GameManager.Instance.LauchGameBallAnimation(ball);
            Destroy(ball.gameObject);

            var emitParams = new EmitParams();
            emitParams.position = ball.transform.position;

            ParticleSystem clone = null;
            switch (ball.Color)
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
                case BallColor.None:
                    throw new InvalidEnumArgumentException();
            }

            clone.transform.position = ball.transform.position;
            clone.Play();
        }

        private void AnimateBallRemovalAndCheckFull(GameBallObject[,]  gameField, bool[,] removeMask, out bool isFull, out bool somethingRemoved)
        {
            var width = gameField.GetLength(0);
            var height = gameField.GetLength(1);
            isFull = true;
            somethingRemoved = false;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (!gameField[x, y])
                    {
                        isFull = false;
                        continue;
                    }

                    if (!removeMask[x, y])
                        continue;

                    isFull = false;
                    somethingRemoved = true;
                    AnimateBallRemoval(gameField[x, y]);
                }
            }
        }

        const float FIELD_CHECK_DELAY = .25f;
        private void ScheduleFieldCheck()
        {
            nextFieldCheck = Time.time + FIELD_CHECK_DELAY;
        }

        float nextFieldCheck = -1;
        private void Update()
        {
            if (nextFieldCheck > 0 && Time.time > nextFieldCheck)
            {
                nextFieldCheck = -1;
                FieldCheck();
            }
        }
    }
}