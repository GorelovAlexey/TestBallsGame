using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.Scripts.Util
{
    public static class TweenUtil
    {
        public static Tween BezierTween(this Transform transform, Vector3 start, Vector3 startControl, Vector3 end, Vector3 endControl, float time)
        {
            var currTime = 0f;
            DOGetter<float> timeGetter = () => currTime;
            DOSetter<float> timeSetter = t =>
            {
                currTime = t;
                var currPosition = BezierUtility.BezierPoint(startControl, start, end, endControl, currTime);
                transform.position = currPosition;
            };

            return DOTween.To(timeGetter, timeSetter, 1f, time).SetLink(transform.gameObject);
        }
    }
}