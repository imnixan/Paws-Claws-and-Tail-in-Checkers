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
        [SerializeField]
        private GameBuilder gameBuilder;
        private List<Cat> cats;
        private GameController gameController;
        CatsType.Team currentTeam;

        public void InitController(List<Cat> gameField, int playerID, GameController gameController)
        {
            if (cats != null)
            {
                UnsubFromCats();
            }
            this.cats = gameField;
            this.gameController = gameController;
            currentTeam = (CatsType.Team)playerID + 1;
            SubOnCats();
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
            if (cat.catData.team == currentTeam)
            {
                Debug.Log("CAT CLICKED");
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

        public void UpgradeCat(CatData catData)
        {
            Cat theCat = new Cat();
            foreach (var cat in cats)
            {
                if (cat.catData.id == catData.id)
                {
                    theCat = cat;
                }
            }
            if (theCat == null)
            {
                return;
            }
            theCat.catData.type = CatsType.Type.Chonky;
            Cat upgradedCat = Instantiate(gameBuilder.chonkyCatPrefab);
            upgradedCat.transform.position = theCat.transform.position;
            upgradedCat.transform.transform.rotation = theCat.transform.rotation;
            upgradedCat.transform.parent = theCat.transform.parent;
            upgradedCat.Init(theCat.catData, theCat.mat);
            if (theCat.catData.team == currentTeam)
            {
                theCat.catTouched -= OnCatClick;
                upgradedCat.catTouched += OnCatClick;
            }
            Debug.Log("rebuildCat");
            cats.Remove(theCat);
            cats.Add(upgradedCat);
            Destroy(theCat.gameObject);
        }
    }
}
