using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [Header("Gameplay Stats")]
        public int currentGuards;
        public int maxGuards = 5;
        /// <summary> The survivors currently in Inventory </summary>
        public int survivorsInInventory;

        private void Start()
        {
            //TODO: TEMP UNTIL GAME FLOW HAS BEEN ESTABLISHED
            StartGame();
        }

        #region Game Flow

        public static Action OnGameStarted, OnGameOver, OnGameEnded;

        public static void StartGame()
        {
            OnGameStarted?.Invoke();
            
            // Reset game stats
            CurrentGameStats = new GameStats();
        }

        public static void EndGame()
        {
            OnGameEnded?.Invoke();
        }
        
        public static void TriggerGameOver()
        {
            OnGameOver?.Invoke();
        }
        #endregion
        
        #region Gameplay

        public static Action OnSurvivorsUpdated;

        public void AddSurvivor()
        {
            if (currentGuards >= maxGuards)
            {
                survivorsInInventory++;
            }
            else
            {
                currentGuards++;
            }
            OnSurvivorsUpdated?.Invoke();
        }

        public void RemoveGuard()
        {
            if (currentGuards <= 0)
            {
                TriggerGameOver();
                //TODO: Might want to change this so that the game over is triggered when the player is hit by a vampire
            }
            else
            {
                currentGuards--;
                OnSurvivorsUpdated?.Invoke();
            }
        }
        
        public static GameStats CurrentGameStats = new GameStats();

        public class GameStats
        {
            // Results
            public int SurvivorsAccepted, SurvivorsRejected, 
                VampiresRejected, VampiresAccepted,
                ImpostersAccepted, ImpostersRejected;

            public int timeSurvived
            {
                get
                {
                    var dlm = FindAnyObjectByType<DaylightManager>();
                    if (!dlm)
                    {
                        Debug.LogWarning("Missing Daylight Manager in Scene");
                        return 0;
                    }
                    return Mathf.RoundToInt(dlm.CurrentGameTime);
                }
            }
        }
        
        #endregion
    }
}
