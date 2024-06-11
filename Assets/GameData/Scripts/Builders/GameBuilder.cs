using System.Collections.Generic;
using PCTC.Builders;
using PCTC.CatScripts;
using PCTC.Controllers;
using PCTC.Game;
using PCTC.Handlers;
using PCTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCTC.Managers
{
    public class GameBuilder : MonoBehaviour
    {
        [SerializeField]
        private PCTC.CatScripts.Cat catPrefab;

        private GameObject field;

        public List<Cat> PlaceCats(CatData[,] catData)
        {
            if (field != null)
            {
                Destroy(field);
                Debug.Log("REBUILD FIELD");
            }
            field = new GameObject("catsField");
            field.transform.SetParent(transform);
            List<Cat> catsList = new List<Cat>();
            int fieldSize = catData.GetLength(0);
            Vector3 catPosition = new Vector3();
            catPosition.y = 0;
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (catData[x, y].id > 1)
                    {
                        catPosition.x = x;
                        catPosition.z = y;
                        Cat newCat = Instantiate(
                            catPrefab,
                            catPosition,
                            new Quaternion(),
                            field.transform
                        );
                        newCat.Init(catData[x, y]);
                        catsList.Add(newCat);
                    }
                }
            }
            return catsList;
        }
    }
}
