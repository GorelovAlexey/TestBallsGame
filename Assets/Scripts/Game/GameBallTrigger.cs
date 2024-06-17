using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameBallTrigger : MonoBehaviour
    {
        public ReactiveProperty<GameBallObject> GameBallEntered = 
            new ReactiveProperty<GameBallObject>();

        [SerializeField] private List<GameBallObject> ballsList = new List<GameBallObject>();
        [SerializeField] private GameBallObject[] ballsPlacements = new GameBallObject[3];

        void OnTriggerEnter2D(Collider2D col)
        {
            var ball = col.gameObject.GetComponent<GameBallObject>();

            if (!ball)
                return;

            GameBallEntered.SetValueAndForceNotify(ball);

            Debug.Log(gameObject.name + " : " + col.gameObject.name);

            ballsList.Add(ball);
            ballsList.Sort(gameBallPositionComaprer);
        }
        void OnTriggerExit2D(Collider2D col)
        {
            var ball = col.gameObject.GetComponent<GameBallObject>();

            if (!ball)
                return;

            ballsList.Remove(ball);
            ballsList.Sort(gameBallPositionComaprer);
        }

        public List<GameBallObject> GetBallPlacements2() => ballsList;

        public GameBallObject[] GetBallPlacements()
        {
            var i2 = ballsPlacements.Length - 1;
            for (var i = ballsList.Count - 1; i>= 0; i--)
            {
                if (i2 < 0)
                    break;

                if (!ballsList[i])
                    continue;

                ballsPlacements[i2] = ballsList[i];
                i2--;
            }

            return ballsPlacements;
        }

        private GameBallPositionComaprer gameBallPositionComaprer = new GameBallPositionComaprer();
        private class GameBallPositionComaprer : IComparer<GameBallObject>
        {
            public int Compare(GameBallObject x, GameBallObject y)
            {
                if (!x)
                {
                    if (!y)
                        return 0;

                    return 1;
                }

                if (!y)
                    return -1;

                if (x.transform.position.y > y.transform.position.y)
                    return -1;
                if (x.transform.position.y < y.transform.position.y)
                    return 1;

                return 0;

            }
        }

        public GameBallObject GetBall(int y)
        {
            if (y < 0 || y > 2 || y >= ballsList.Count)
                return null;

            return ballsList[y];
        }
    }
}