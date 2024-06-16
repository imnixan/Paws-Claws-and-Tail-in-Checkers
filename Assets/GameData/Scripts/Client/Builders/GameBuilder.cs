using System.Collections.Generic;
using PJTC.Builders;
using PJTC.CatScripts;
using PJTC.Controllers;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Handlers;
using PJTC.Managers;
using PJTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJTC.Managers
{
    public class GameBuilder : MonoBehaviour
    {
        [SerializeField]
        private Cat catPrefab;

        [SerializeField]
        public VisualModel normalModel,
            chonkyModel;

        [SerializeField]
        private Material orangeMat,
            blackMat;

        [SerializeField]
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
                        VisualModel modelPrefab =
                            catData[x, y].type == Enums.CatsType.Type.Normal
                                ? normalModel
                                : chonkyModel;
                        Material mat =
                            catData[x, y].team == Enums.CatsType.Team.Orange ? orangeMat : blackMat;
                        VisualModel newModel = Instantiate(modelPrefab, newCat.transform);
                        newModel.Init(mat);
                        newCat.Init(catData[x, y]);
                        catsList.Add(newCat);
                    }
                }
            }
            return catsList;
        }
    }
}
