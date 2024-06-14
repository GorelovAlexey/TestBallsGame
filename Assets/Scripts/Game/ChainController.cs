using System;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ChainController : MonoBehaviour
    {
        [SerializeField] Transform leftBallPosition;
        [SerializeField] Transform midBallPosition;
        [SerializeField] Transform rightBallPosition;

        [SerializeField] Rigidbody2D fakeBallRigidbody;
        [SerializeField] GameBallObject fakeBall;

        [SerializeField] HingeJoint2D firstLink;

        private GameBallObject spawnedBall;

        public void AttachBall(GameBallObject b)
        {
            fakeBall.Setup(b);
            fakeBall.SetTransparent(false);

            spawnedBall = b;
            spawnedBall.gameObject.SetActive(false);
        }
        
        public void ReleaseBall()
        {
            if (!spawnedBall)
                return;

            fakeBall.SetTransparent(true);

            spawnedBall.gameObject.SetActive(true);
            spawnedBall.transform.position = fakeBall.transform.position;
            spawnedBall.transform.rotation = fakeBall.transform.rotation;

            var spawnedRigidbody = spawnedBall.GetComponent<Rigidbody2D>();
            spawnedRigidbody.velocity = fakeBallRigidbody.velocity;
            spawnedRigidbody.angularVelocity = fakeBallRigidbody.angularVelocity;

            spawnedBall = null;
        }

        public void Restart()
        {
            spawnedBall = null;
            fakeBall.SetTransparent(true);
        }

        private void SetupSwing()
        {
            const float MAX_VELOCITY = 5;
            Vector2 lastVelocity = Vector2.zero;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                var ballVelocity = fakeBall.Rigidbody2D.velocity;
                var velocityChange = ballVelocity.magnitude - lastVelocity.magnitude;
                lastVelocity = ballVelocity;

                var movingLeft = ballVelocity.x < 0;
                var movingRight = ballVelocity.x > 0;

                // we are speeding up
                if (velocityChange > 0)
                {
                    /*
                    if (movingLeft)
                        fakeBall.Rigidbody2D.AddRelativeForce(fakeBall.transform.right * -1);
                    else
                        fakeBall.Rigidbody2D.AddRelativeForce(fakeBall.transform.right);*/
                    var vectorTop = (firstLink.transform.position - fakeBall.transform.position);
                    var vectorLeft = (Vector2)(Quaternion.Euler(0, 0, 90) * vectorTop);
                    var vectorRight = -vectorLeft;

                    //Debug.DrawLine(fakeBall.transform.position, fakeBall.transform.position + vectorTop, Color.green, Time.fixedDeltaTime);
                    //Debug.DrawLine(fakeBall.transform.position, fakeBall.transform.position + (Vector3)vectorRight, Color.red, Time.fixedDeltaTime);
                    //Debug.DrawLine(fakeBall.transform.position, fakeBall.transform.position + (Vector3)vectorLeft, Color.blue, Time.fixedDeltaTime);

                    var force = movingLeft ? vectorLeft.normalized : vectorRight.normalized;

                    fakeBall.Rigidbody2D.AddForce(force * 2f);
                }

                if (ballVelocity.magnitude > MAX_VELOCITY)
                {
                    ballVelocity = ballVelocity.normalized * MAX_VELOCITY;
                }
            });
        }

        public void Awake()
        {
            SetupSwing();
        }
    }

}