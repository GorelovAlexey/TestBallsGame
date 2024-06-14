using System.Collections;
using System.Drawing;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game
{
    public class GameBallObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject particlesPrefab;

        private Rigidbody2D rigidbody2D;
        public Rigidbody2D Rigidbody2D => rigidbody2D ??= GetComponent<Rigidbody2D>();

        public Sprite GetSprite() => spriteRenderer.sprite;
        
        public BallColor Color { get; private set; }

        public void Setup(BallColor color, Sprite s)
        {
            Color = color;
            spriteRenderer.sprite = s;
        }

        public void Setup(GameBallObject other)
        {
            Color = other.Color;
            spriteRenderer.sprite = other.spriteRenderer.sprite;
        }

        public void SetTransparent(bool transparent)
        {
            spriteRenderer.enabled = !transparent;
            GetComponent<CircleCollider2D>().enabled = !transparent;
        }

        private void OnDestroy()
        {
            if (!particlesPrefab)
                return;

            var particlesInst = Instantiate(particlesPrefab);
            var ps = particlesInst.GetComponent<ParticleSystem>();
            ps.transform.position = transform.position;
            var main = ps.main;

            if (Color == BallColor.Blue)
                main.startColor = new ParticleSystem.MinMaxGradient(UnityEngine.Color.blue);
            else if (Color == BallColor.Red)
                main.startColor = new ParticleSystem.MinMaxGradient(UnityEngine.Color.red);
            else
                main.startColor = new ParticleSystem.MinMaxGradient(UnityEngine.Color.green);

        }
    }
}