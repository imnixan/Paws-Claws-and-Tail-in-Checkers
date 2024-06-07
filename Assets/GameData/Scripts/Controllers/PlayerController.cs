using System;
using System.Collections.Generic;
using NUnit.Framework;
using PCTC.CatScripts;
using PCTC.Enums;
using PCTC.Handlers;
using PCTC.Managers;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        private List<Cat> cats = new List<Cat>();
        private GameController gameController;
        CatsType.Team currentTeam;

        public void InitController(List<Cat> gameField, int playerId, GameController gameController)
        {
            this.cats = gameField;
            this.gameController = gameController;
            currentTeam = (CatsType.Team)playerId + 1;
            SubOnCats();
        }

        private void OnDestroy()
        {
            UnsubFromCats();
        }

        private void SubOnCats()
        {
            foreach (var cat in cats)
            {
                cat.catTouched += OnCatClick;
            }
        }

        private void UnsubFromCats()
        {
            foreach (var cat in cats)
            {
                cat.catTouched -= OnCatClick;
            }
        }

        private void OnCatClick(Cat cat)
        {
            if (cat.catData.team == currentTeam)
            {
                CatData catData = cat.catData;
                gameController.OnCatClick(catData);
            }
        }

        public Cat GetCat(CatData catData)
        {
            foreach (var cat in cats)
            {
                if (cat.catData.id == catData.id)
                {
                    Cat theCat = cat;
                    return theCat;
                }
            }
            return null;
        }

        public void RemoveCat(CatData catData)
        {
            Cat theCat = new Cat();
            foreach (var cat in cats)
            {
                if (cat.catData.id == catData.id)
                {
                    theCat = cat;
                }
            }
            theCat.RemoveCat();
            cats.Remove(theCat);
        }

        public void UpgradeCat(CatData catData)
        {
            Cat theCat = new Cat();
            foreach (var cat in cats)
            {
                if (cat.catData == catData)
                {
                    theCat = cat;
                }
            }
            theCat.UpgradeCat();
        }
    }
}
