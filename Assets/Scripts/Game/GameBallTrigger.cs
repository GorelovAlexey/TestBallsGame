using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameBallTrigger : MonoBehaviour
    {
        public ReactiveProperty<GameBallObject> GameBallEntered = 
            new ReactiveProperty<GameBallObject>();

        void OnTriggerEnter2D(Collider2D col)
        {
            var ball = col.gameObject.GetComponent<GameBallObject>();

            if (ball)
                GameBallEntered.SetValueAndForceNotify(ball);

            Debug.Log(gameObject.name + " : " + col.gameObject.name);
        }
    }
}