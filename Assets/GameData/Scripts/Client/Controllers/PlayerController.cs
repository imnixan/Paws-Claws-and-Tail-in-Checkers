using System;
using System.Collections.Generic;
using PJTC.CatScripts;
using PJTC.Enums;
using PJTC.Handlers;
using PJTC.Managers;
using PJTC.Managers;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameBuilder gameBuilder;
        public List<Cat> cats;
        public List<Cat> ownCats { get; private set; }
        private GameController gameController;

        public void InitController(List<Cat> gameField, int playerID, GameController gameController)
        {
            if (cats != null)
            {
                UnsubFromCats();
            }
            this.cats = gameField;
            this.gameController = gameController;

            FillOwnCats();
            SubOnCats();
        }

        private void FillOwnCats()
        {
            ownCats = new List<Cat>();
            foreach (Cat cat in cats)
            {
                if (cat.catData.team == GameController.playerTeam)
                {
                    ownCats.Add(cat);
                }
            }
        }

        private void OnDestroy()
        {
            UnsubFromCats();
        }

        private void SubOnCats()
        {
            if (cats == null)
                return;
            foreach (var cat in cats)
            {
                cat.catTouched += OnCatClick;
            }
        }

        private void UnsubFromCats()
        {
            if (cats == null)
                return;

            foreach (var cat in cats)
            {
                if (cat != null)
                {
                    cat.catTouched -= OnCatClick;
                }
            }
        }

        private void OnCatClick(Cat cat)
        {
            if (cat.catData.team == GameController.playerTeam)
            {
                CatData catData = cat.catData;
                gameController.OnCatClick(catData);
            }
        }

        public Cat GetCat(int id)
        {
            foreach (var cat in cats)
            {
                if (cat.catData.id == id)
                {
                    Cat theCat = cat;
                    return theCat;
                }
            }
            return null;
        }

        public void RemoveCat(CatData catData)
        {
            Cat theCat = null;
            foreach (var cat in cats)
            {
                if (cat.catData.id == catData.id)
                {
                    theCat = cat;
                }
            }
            theCat?.RemoveCat();
            cats.Remove(theCat);
        }

        public void MakeCatChonky(Cat cat)
        {
            Material mat = cat.mat;
            Destroy(cat.visualModel.gameObject);
            VisualModel newModel = Instantiate(gameBuilder.chonkyModel, cat.transform);
            newModel.Init(mat);
            cat.visualModel = newModel;
            cat.catData.type = CatsType.Type.Chonky;
        }
    }
}
