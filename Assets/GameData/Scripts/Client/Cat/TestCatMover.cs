using System;
using DG.Tweening;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Structs;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PJTC.CatScripts
{
    public class TestCatMover : MonoBehaviour
    {
        [SerializeField]
        private Cat catPrefab;

        [SerializeField]
        private Vector2Int[] movePoses;

        [SerializeField]
        private Vector2Int startPos;

        [SerializeField]
        private CatsType.Type startType;

        [SerializeField]
        private CatsType.Attack startAttack;

        [SerializeField]
        private CatsType.Team startTeam;

        [Tooltip("3D visual model prefab")]
        [SerializeField]
        public VisualModel normalModel,
            chonkyModel;

        [Tooltip("Color matererials for teams")]
        [SerializeField]
        private Material orangeMat,
            blackMat;

        [SerializeField]
        private Material[] attackMats;

        private Cat cat;

        private void Start()
        {
            cat = Instantiate(
                catPrefab,
                new Vector3(startPos.x, 0, startPos.y),
                new Quaternion(),
                transform
            );

            VisualModel modelPrefab =
                startType == Enums.CatsType.Type.Normal ? normalModel : chonkyModel;
            Material mat = startTeam == Enums.CatsType.Team.Orange ? orangeMat : blackMat;

            VisualModel newModel = Instantiate(modelPrefab, cat.transform);

            newModel.Init(mat);

            cat.Init(new Structs.CatData(0, startPos, startType, startTeam, startAttack));
            cat.visualModel = newModel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                Material mat = cat.mat;
                Destroy(cat.visualModel.gameObject);
                VisualModel newModel = Instantiate(chonkyModel, cat.transform);
                newModel.Init(mat);
                cat.visualModel = newModel;
                cat.catData.type = CatsType.Type.Chonky;
                cat.OnCatUpgrade();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                Move();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                cat.OnBattle(false, true);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                cat.OnBattle(true, true, attackMats[(int)cat.catData.attackType - 1]);
            }
        }

        private void Move()
        {
            Sequence moveSeq = DOTween.Sequence();
            foreach (var move in movePoses)
            {
                moveSeq.Append(cat.MoveTo(move));
            }
            moveSeq.Restart();
        }
    }
}
