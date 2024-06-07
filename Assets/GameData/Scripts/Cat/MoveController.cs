using System;
using DG.Tweening;
using UnityEngine;

namespace PCTC.CatScripts
{
    public class MoveController : PCTC.Controllers.MoveController
    {
        protected void Start()
        {
            base.Start();
            speed = 5;
        }
    }
}
