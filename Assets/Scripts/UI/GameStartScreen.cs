using Assets.Scripts.Util;
using DG.Tweening;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameStartScreen : SuperSimpleWindowBase
    {
        [Space]
        [SerializeField] private Button btnStart;
        [SerializeField] private Button btnClose;

        [Space]
        [SerializeField] Transform pointA;
        [SerializeField] Transform pointAControl;

        [SerializeField] Transform pointB;
        [SerializeField] Transform pointBControl;

        [SerializeField] Transform pointC;
        [SerializeField] Transform pointCControl;

        [Space]
        [SerializeField] Transform ball1;
        [SerializeField] Transform ball2;
        [SerializeField] Transform ball3;

        private Sequence animationSeq;

        public override void OnShow()
        {
            animationSeq?.Kill();
            animationSeq = CreateAnimSeq().SetLink(gameObject);
            animationSeq.Play();

            base.OnShow();
        }

        private Sequence CreateAnimSeq()
        {
            var seq = DOTween.Sequence();

            var a = pointA.transform.position;
            var ac = pointAControl.transform.position;
            var b = pointB.transform.position;
            var bc = pointBControl.transform.position;
            var c = pointC.transform.position;
            var cc = pointCControl.transform.position;

            var minTime = .8f;
            var time1 = Random.Range(minTime, minTime * 3f);
            var time2 = Random.Range(minTime, minTime * 2f);
            var time3 = Random.Range(minTime, minTime * 3.5f);

            var startTime1 = 0;
            var startTime2 = startTime1 + time1;
            var startTime3 = startTime2 + time2;
            
            seq.Insert(startTime1, ball1.BezierTween(a, ac, b, bc, time1));
            seq.Insert(startTime2, ball1.BezierTween(b, bc, c, cc, time2));
            seq.Insert(startTime3, ball1.BezierTween(c, cc, a, ac, time3));

            seq.Insert(startTime1, ball2.BezierTween(b, bc, c, cc, time1) );
            seq.Insert(startTime2 , ball2.BezierTween(c, cc, a, ac, time2));
            seq.Insert(startTime3, ball2.BezierTween(a, ac, b, bc, time3));

            seq.Insert(startTime1, ball3.BezierTween(c, cc, a, ac, time1));
            seq.Insert(startTime2, ball3.BezierTween(a, ac, b, bc, time2));
            seq.Insert(startTime3, ball3.BezierTween(b, bc, c, cc, time3));

            /*
            seq.Insert(startTime1, ball1.DOMove(b, time1).ChangeStartValue(a));
            seq.Insert(startTime2, ball1.DOMove(c, time2));
            seq.Insert(startTime3, ball1.DOMove(a, time3)); 

            seq.Insert(startTime1, ball2.DOMove(c, time1).ChangeStartValue(b));
            seq.Insert(startTime2, ball2.DOMove(a, time2));
            seq.Insert(startTime3, ball2.DOMove(b, time3)); 

            seq.Insert(startTime1, ball3.DOMove(a, time1).ChangeStartValue(c));
            seq.Insert(startTime2, ball3.DOMove(b, time2));
            seq.Insert(startTime3, ball3.DOMove(c, time3));
            */

            seq.OnComplete(() => seq.Restart());
            seq.SetAutoKill(false);

            return seq.SetLink(gameObject);
        }

        public override void OnHide()
        {
            animationSeq?.Pause();
        }

        public override void OnAwake()
        {
            base.OnAwake();

            btnClose.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            btnStart.onClick.AddListener(() =>
            {
                if (IsHiding)
                    return;

                Hide();
                DOVirtual.DelayedCall(HideShowTime / 2, 
                    () => GameManager.Instance.StartGame()).SetLink(gameObject);
            });
        }
    }
}