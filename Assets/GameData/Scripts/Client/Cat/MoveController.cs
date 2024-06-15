using System;
using DG.Tweening;
using UnityEngine;

namespace PJTC.CatScripts
{
    public class MoveController : PJTC.Controllers.MoveController
    {
        protected void Start()
        {
            base.Start();
            speed = 5;
        }
    }
}
