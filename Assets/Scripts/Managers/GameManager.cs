using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

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
        
        public static GameStats CurrentGameStats = new GameStats();

        public class GameStats
        {
            public int SurvivorsAccepted, SurvivorsRejected, 
                VampiresRejected, VampiresAccepted,
                ImpostersAccepted, ImpostersRejected;

            public int TimeSurvived
            {
                get
                {
                    var dlm = FindAnyObjectByType<DaylightManager>();
                    return Mathf.RoundToInt(dlm.CurrentGameTime);
                }
            }
        }
        
        #endregion
    }
}
