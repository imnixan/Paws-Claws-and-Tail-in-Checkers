using System;
using System.Collections.Generic;
using DG.Tweening;
using PJTC.CatScripts;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Handlers;
using PJTC.Managers;
using PJTC.Managers;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.Controllers
{
    public class GameController : MonoBehaviour
    {
        public static event UnityAction<CatData, CatsType.Attack> AttackChanged;
        public static CatsType.Team playerTeam;

        [SerializeField]
        private CarpetController carpetController;

        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private Material[] attackMats;

        private ClientGameManager gameManager;
        private GameField gameField;
        private Enums.GameData.GameState gameState;
        private CatData choosedCat = new CatData();
        public bool playerOrder;

        public void Init(
            CatData[,] catData,
            int playerID,
            List<Cat> cats,
            ClickInputHandler[,] cellHandlers,
            ClientGameManager gameManager
        )
        {
            playerTeam = (CatsType.Team)playerID;
            this.gameField = new GameField(catData);
            playerController.InitController(cats, playerID, this);
            this.carpetController.Init(cellHandlers, this);
            this.gameManager = gameManager;
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
            List<CatData> catsForRemove = new List<CatData>();
            Sequence move = DOTween.Sequence();

            foreach (var completedMove in moveResult.moves)
            {
                Cat movedCat = playerController.GetCat(completedMove.moveData.catData.id);

                if (movedCat != null)
                {
                    Cat enemyCat = playerController.GetCat(completedMove.enemy.id);
                    if (completedMove.moveWithBattle)
                    {
                        move.Append(movedCat.MoveTo(completedMove.enemy.position));
                        if (completedMove.battleWin)
                        {
                            move.AppendCallback(() =>
                            {
                                movedCat.OnBattle(
                                    true,
                                    true,
                                    attackMats[(int)movedCat.catData.attackType - 1]
                                );
                                enemyCat.OnBattle(false, false);
                            });
                        }
                        else
                        {
                            move.AppendCallback(() =>
                            {
                                movedCat.OnBattle(
                                    true,
                                    false,
                                    attackMats[(int)movedCat.catData.attackType - 1]
                                );
                                enemyCat.transform.forward = movedCat.transform.forward * -1;
                                enemyCat.OnBattle(false, true);
                            });
                        }
                    }
                    move.Append(movedCat.MoveTo(completedMove.moveData.moveEnd));
                    move.AppendCallback(() =>
                    {
                        if (completedMove.moveWithBattle)
                        {
                            movedCat.UpdateAttackType(completedMove.moveData.catData);

                            if (completedMove.battleWin)
                            {
                                catsForRemove.Add(completedMove.enemy);
                            }
                            else
                            {
                                Cat enemyCat = playerController.GetCat(completedMove.enemy.id);
                                enemyCat.UpdateAttackType(completedMove.enemy);
                            }
                        }

                        if (completedMove.moveWithUpgrade)
                        {
                            playerController.MakeCatChonky(movedCat);
                        }
                    });
                }
            }
            move.AppendCallback(() =>
                {
                    foreach (var catForRemove in catsForRemove)
                    {
                        playerController.RemoveCat(catForRemove);
                    }
                    catsForRemove.Clear();
                    gameField.UpdateField(moveResult);
                    gameManager.OnReady(gameField.mapHash);
                })
                .Restart();
        }

        private void ChangeAttackType(CatData catData)
        {
            CatsType.Attack oldAttack = catData.attackType;
            Cat cat = playerController.GetCat(catData.id);

            int currentAttackType = (int)oldAttack;
            currentAttackType++;

            int typesTotal = Enum.GetValues(typeof(CatsType.Attack)).Length;
            if (currentAttackType >= typesTotal)
            {
                currentAttackType = 0;
            }

            CatsType.Attack newAttack = (CatsType.Attack)currentAttackType;
            catData.attackType = newAttack;
            cat.UpdateAttackType(catData);
            gameField.SetElement(cat.catData);

            AttackChanged?.Invoke(cat.catData, oldAttack);
        }

        private void OnRandomChoose(AttacksPool attacks)
        {
            CatsType.Attack[] attackTypes = GetAttacksShuffleArray(attacks);
            List<Cat> cats = playerController.ownCats;
            CatsType.Attack currentAttack = CatsType.Attack.None;

            for (int i = 0; i < attackTypes.Length; i++)
            {
                Cat cat = cats[i];
                currentAttack = cat.catData.attackType;
                cat.UpdateAttackType(
                    new CatData(
                        cat.catData.id,
                        cat.catData.position,
                        cat.catData.type,
                        cat.catData.team,
                        attackTypes[i]
                    )
                );

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

            return ArrayTransformer.Shuffle(attackTypes);
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
