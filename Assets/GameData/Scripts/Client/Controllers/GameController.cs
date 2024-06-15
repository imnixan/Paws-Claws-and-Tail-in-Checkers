using System;
using System.Collections.Generic;
using DG.Tweening;
using GameData.Managers;
using PJTC.Builders;
using PJTC.CatScripts;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Handlers;
using PJTC.Managers;
using PJTC.Scripts;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.Controllers
{
    public class GameController : MonoBehaviour
    {
        public static event UnityAction<CatData, CatsType.Attack> AttackChanged;

        private GameField gameField;

        [SerializeField]
        private CarpetController carpetController;

        [SerializeField]
        private PlayerController playerController;

        public bool playerOrder;

        private CatData choosedCat = new CatData();
        private ClientGameManager gameManager;
        private Enums.GameData.GameState gameState;
        public static CatsType.Team playerTeam;

        public void Init(
            CatData[,] catData,
            int playerID,
            List<Cat> cats,
            ClickInputHandler[,] cellHandlers,
            ClientGameManager gameManager
        )
        {
            this.gameField = new GameField(catData);
            playerController.InitController(cats, playerID, this);
            this.carpetController.Init(cellHandlers, this);
            this.gameManager = gameManager;
            playerTeam = (CatsType.Team)playerID;
        }

        public void ShowPossibleMoves(Moves moves)
        {
            carpetController.ActiveCells(moves.possibleMoves);
        }

        public void OnCatClick(CatData cat)
        {
            switch (gameState)
            {
                case Enums.GameData.GameState.Game:
                    ChooseCat(cat);
                    break;
                case Enums.GameData.GameState.PlayerInit:
                    ChangeAttackType(cat);
                    break;
            }
        }

        private void ChangeAttackType(CatData catData)
        {
            CatsType.Attack oldAttack = catData.attackType;
            Cat cat = playerController.GetCat(catData.id);
            int currentAttackType = (int)oldAttack;
            int typesTotal = Enum.GetValues(typeof(CatsType.Attack)).Length;
            currentAttackType++;
            if (currentAttackType >= typesTotal)
            {
                currentAttackType = 0;
            }
            CatsType.Attack newAttack = (CatsType.Attack)currentAttackType;
            cat.UpdateAttackType(newAttack);
            gameField.SetElement(cat.catData);
            AttackChanged?.Invoke(cat.catData, oldAttack);
        }

        private void OnRandomChoose(AttacksPool attacks)
        {
            CatsType.Attack[] attackTypes = GetAttacksShuffleArray(attacks);
            attackTypes = ArrayTransformer.Shuffle(attackTypes);
            List<Cat> cats = playerController.ownCats;
            CatsType.Attack currentAttack = CatsType.Attack.None;
            for (int i = 0; i < attackTypes.Length; i++)
            {
                Cat cat = cats[i];
                currentAttack = cat.catData.attackType;
                cats[i].UpdateAttackType(attackTypes[i]);
                gameField.SetElement(cat.catData);
                AttackChanged?.Invoke(cat.catData, currentAttack);
            }
        }

        private CatsType.Attack[] GetAttacksShuffleArray(AttacksPool attacks)
        {
            CatsType.Attack[] attackTypes = new CatsType.Attack[playerController.ownCats.Count];
            for (int i = 0; i < attackTypes.Length; i++)
            {
                if (i >= attacks.maxPaws + attacks.maxJaws)
                {
                    attackTypes[i] = CatsType.Attack.Tail;
                }
                else if (i >= attacks.maxPaws)
                {
                    attackTypes[i] = CatsType.Attack.Jaws;
                }
                else
                {
                    attackTypes[i] = CatsType.Attack.Paws;
                }
            }

            return attackTypes;
        }

        private void ChooseCat(CatData cat)
        {
            if (playerOrder)
            {
                choosedCat = cat;
                gameManager.RequestPossibleMoves(cat);
                carpetController.ActiveCells(new Vector2Int[0]);
            }
        }

        public void OnCarpetClick(Vector2Int position)
        {
            if (choosedCat.id > 1)
            {
                MoveData moveData = new MoveData(choosedCat, position);
                gameManager.MakeMove(moveData);
                carpetController.DeactivateCells();
                choosedCat.id = 0;
            }
        }

        public void OnPlayerMove(MoveResult moveResult)
        {
            Sequence move = DOTween.Sequence();

            foreach (var moveData in moveResult.moves)
            {
                Cat cat = playerController.GetCat(moveData.catData.id);
                if (cat != null)
                {
                    move.Append(cat.MoveTo(moveData.moveEnd));
                }
            }
            move.AppendCallback(() =>
                {
                    gameField.UpdateField(moveResult);
                    RemoveCats(moveResult.catsForRemove);
                    UpgradeCats(moveResult.catsForUpgrade);
                    gameManager.OnReady(gameField.mapHash);
                })
                .Restart();
        }

        private void RemoveCats(CatData[] catsForRemove)
        {
            foreach (var catData in catsForRemove)
            {
                playerController.RemoveCat(catData);
            }
        }

        private void UpgradeCats(CatData[] catsForUpgrade)
        {
            foreach (var catData in catsForUpgrade)
            {
                playerController.UpgradeCat(catData);
            }
        }

        public void UpdateCatAttackType(CatData catData)
        {
            gameField.SetElement(catData);
        }

        private void OnGameStateChanged(Enums.GameData.GameState state)
        {
            this.gameState = state;
        }

        private void OnPlayerFinishChooseAttack()
        {
            List<AttackTypeData> attackTypes = new List<AttackTypeData>();
            foreach (var cat in gameField.matrix)
            {
                if (cat.team == playerTeam)
                {
                    attackTypes.Add(new AttackTypeData(cat.id, (int)cat.attackType));
                }
            }
            gameManager.SendPlayerAttackTypes(new PlayerAttackTypesData(attackTypes.ToArray()));
        }

        private void SubOnEvents()
        {
            ServerDataHandler.Move += OnPlayerMove;
            ServerDataHandler.GotPossibleMoves += ShowPossibleMoves;
            ClientGameManager.GameStateChanged += OnGameStateChanged;
            AttackChooseManager.PlayerFinishChoosingAttacks += OnPlayerFinishChooseAttack;
            AttackChooseManager.RandomChoose += OnRandomChoose;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.Move += OnPlayerMove;
            ServerDataHandler.GotPossibleMoves -= ShowPossibleMoves;
            ClientGameManager.GameStateChanged -= OnGameStateChanged;
            AttackChooseManager.PlayerFinishChoosingAttacks -= OnPlayerFinishChooseAttack;
            AttackChooseManager.RandomChoose -= OnRandomChoose;
        }

        private void OnEnable()
        {
            SubOnEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }
    }
}
