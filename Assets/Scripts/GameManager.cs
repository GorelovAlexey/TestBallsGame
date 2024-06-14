using Assets.Scripts.Game;
using Assets.Scripts.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= FindAnyObjectByType<GameManager>();

        private Camera camera;
        public Camera Camera => camera ??= Camera.main;

        [SerializeField] public ScoresDictionary scoreDictionary;
        public ReactiveProperty<int> PlayerScore { get; private set; } = new ReactiveProperty<int>();

        [SerializeField] private HUD hud;
        [SerializeField] private Transform screensHolder;
        [SerializeField] private LevelController level;

        public T ShowWindow<T>() where T : SuperSimpleWindowBase
        {
            var window = GetWindow<T>();
            window.Show();
            return window;
        }

        public T GetWindow<T>() where T : SuperSimpleWindowBase
        {
            var window = screensHolder.GetComponentInChildren<T>(true);
            return window;
        }

        const float CAMERA_UP_POS = 12;
        const float CAMERA_MOVE_DUR = 1.2f;

        // GAME ENTRY POINT
        public void Awake()
        {
            _instance = this;
            var camPos = Camera.transform.position;
            camPos.y = CAMERA_UP_POS;
            Camera.transform.position = camPos;
            SetStateMenu();
        }

        public void StartGame()
        {
            PlayerScore.Value = 0;
            level.Restart();
            SetStateInGame(true);
        }

        public void SetStateMenu()
        {
            SetStateInGame(false);
            ShowWindow<GameStartScreen>();
        }

        public void SetStateInGame(bool inGame)
        {
            if (inGame)
            {
                Camera.transform.DOMoveY(0, CAMERA_MOVE_DUR);
                hud.SetIngame(true);
                HideWindows();
                return;
            }

            Camera.transform.DOMoveY(CAMERA_UP_POS, CAMERA_MOVE_DUR);
            hud.SetIngame(false);
        }

        public void FinishGame()
        {
            ShowWindow<GameEndScreen>();
            SetStateInGame(false);
        }

        public void RestartGame()
        {
            StartGame();
        }

        private void HideWindows()
        {
            foreach (var item in screensHolder.GetComponentsInChildren<SuperSimpleWindowBase>())
                item.Hide();
        }

        IDisposable disposable;
        public void SetOnScrenClick(Action<PointerEventData> a)
        {
            disposable?.Dispose();
            disposable = hud.pressControllPanel.OnPointerClickAsObservable().Subscribe(e => a.Invoke(e));
        }

        public void LauchGameBallAnimation(GameBallObject ball)
        {
            hud.ShowGameBallFloat(ball);
        }
    }
}