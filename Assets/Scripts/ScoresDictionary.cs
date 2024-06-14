using Assets.Scripts.Game;
using RotaryHeart.Lib.SerializableDictionary;
using System;

namespace Assets.Scripts
{
    [Serializable]
    public class ScoresDictionary : SerializableDictionaryBase<BallColor, int> { }
}