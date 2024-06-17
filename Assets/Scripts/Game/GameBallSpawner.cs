using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game
{
    [Serializable]
    public class BallsSpriteDictionary : SerializableDictionaryBase<BallColor, Sprite> { }

    public class GameBallSpawner : MonoBehaviour
    {
        [SerializeField] private BallsSpriteDictionary ballColors;
        [SerializeField] private GameBallObject ballPrefab;

        [SerializeField] private Transform ballsSpawn;

        public Sprite GetBallSprite(BallColor c) => ballColors[c];

        public GameBallObject SpawnBall()
        {
            var ball = Instantiate(ballPrefab, ballsSpawn);
            var color =  (BallColor)Random.Range(1, 4);
            ball.Setup(color, ballColors[color]);
            return ball;
        }

        public void SpawnBalls(int count)
        {
            for (var i = 0; i < count; i++)
            {
                SpawnBall();
            }
        }

        public void ClearBalls()
        {
            var balls = ballsSpawn.GetComponentsInChildren<GameBallObject>();
            foreach (var b in balls) 
                Destroy(b.gameObject);
        }

    }
}