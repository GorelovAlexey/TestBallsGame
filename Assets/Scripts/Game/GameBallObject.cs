using DG.Tweening;
using System;
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
            var col = GetComponent<CircleCollider2D>();
            if (col)
                col.enabled = !transparent;
        }
    }
}