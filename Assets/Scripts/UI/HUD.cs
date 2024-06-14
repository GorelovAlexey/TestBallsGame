using Assets.Scripts.Game;
using Assets.Scripts.Util;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text textScore;
        [SerializeField] private RectTransform panelScore;
        [SerializeField] public Image pressControllPanel;

        [Space]
        [SerializeField] private Image floatPrefab;
        [SerializeField] private Transform floatLayer;
        [SerializeField] private float floatAnimTime;
        [Space]
        [SerializeField] private Transform floatEndControl;
        [SerializeField] private Transform floatStartControl;


        public void ShowGameBallFloat(GameBallObject o)
        {
            var cam = GameManager.Instance.Camera;
            var spawnPos = cam.WorldToScreenPoint(o.transform.position);

            var addScore = GameManager.Instance.scoreDictionary[o.Color];
            var sprite = o.GetSprite();
            var itemFloat = Instantiate(floatPrefab, floatLayer);
            itemFloat.transform.position = o.transform.position;
            itemFloat.gameObject.SetActive(true);
            itemFloat.transform.position = spawnPos;
            itemFloat.sprite = sprite;
            itemFloat.transform.DOScale(.1f, floatAnimTime);
            itemFloat.transform.BezierTween(spawnPos, floatStartControl.position,
                 panelScore.position, floatEndControl.position, floatAnimTime)
                .OnComplete(() => {
                    GameManager.Instance.PlayerScore.Value += addScore;
                    Destroy(itemFloat.gameObject);
                })
                .SetLink(gameObject).SetLink(itemFloat.gameObject);
        }

        public void Start()
        {
            GameManager.Instance.PlayerScore.Subscribe(x => textScore.text = x.ToString()).AddTo(gameObject);
            GameManager.Instance.PlayerScore.Pairwise().Subscribe((pair) => {
                if (pair.Current > pair.Previous)
                    ShakeScoreAnimation(true);
                else if (pair.Current < pair.Previous)
                    ShakeScoreAnimation(false);

            }).AddTo(gameObject);
        }

        Tween scoreAnim;
        private void ShakeScoreAnimation(bool scaleUp)
        {
            scoreAnim?.Rewind();
            var endScale = new Vector3( .6f, .5f, 1f);

            scoreAnim = panelScore.transform.DOPunchScale(scaleUp ? endScale : -endScale, .2f)
                .SetLink(panelScore.gameObject);
        }

        public void SetIngame(bool ingame)
        {
            canvasGroup.alpha = ingame ? 1 : 0;
            canvasGroup.interactable = ingame ? true : false;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                ShakeScoreAnimation(true);
            if (Input.GetKey(KeyCode.RightArrow))
                ShakeScoreAnimation(false);
        }
    }
}