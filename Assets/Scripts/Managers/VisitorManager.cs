using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using Triggerable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class VisitorManager : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField, ReadOnly] private Entrance[] entrances;
        private List<Entrance> _availableEntrances;
        [SerializeField] private AnimationCurve difficultyCurve;
        [SerializeField] private VisitorData vampireData, survivorData, imposterData;

        [Header("Spawning")]
        [SerializeField] private Vector2 spawnRate = new Vector2(12f, 15f);
        [SerializeField] private int maximumVisitors = 5;
        
        [SerializeField, ReadOnly] private float _currentTime, _targetTime;
        private bool _doTimer;
        
        private void Awake()
        {
            entrances = FindObjectsByType<Entrance>();

            _availableEntrances = entrances.ToList();
            Debug.Log($"Entrances Count = {_availableEntrances.Count}");
        }
        
        private void OnEnable()
        {
            GameManager.OnGameStarted += StartSpawning;
            GameManager.OnGameEnded += StopSpawning;
        }
        
        private void OnDisable()
        {
            GameManager.OnGameStarted -= StartSpawning;
            GameManager.OnGameEnded -= StopSpawning;
        }

        private void FixedUpdate()
        {
            if (_doTimer) DoSpawning();
        }

        #region Spawning

        private void StartSpawning()
        {
            UpdateTargetTime();

            _doTimer = true;
        }

        private void StopSpawning()
        {
            _targetTime = 0f;
            _doTimer = false;
        }

        private void UpdateTargetTime()
        {
            var targetTime = Random.Range(spawnRate.x, spawnRate.y);
            _targetTime = targetTime;
        }

        private void DoSpawning()
        {
            _currentTime += Time.fixedDeltaTime;

            if (_currentTime < _targetTime) return;
            
            UpdateTargetTime();
            SpawnVisitor();

            _currentTime = 0f;
        }

        private void SpawnVisitor()
        {
            // Pick Entrance
            if (_availableEntrances.Count <= 0) return;
            Entrance entrance = _availableEntrances[Random.Range(0, _availableEntrances.Count)];
            
            _availableEntrances.Remove(entrance);
            
            entrance.DeliverVisitor(GetRandomVisitor());
        }
        
        public void ReturnEntrance(Entrance entrance)
        {
            _availableEntrances.Add(entrance);
        }
        
        private VisitorData GetRandomVisitor()
        {
            var pick = Random.value;

            if (pick < vampireData.appearanceWeight)
            {
                return vampireData;
            }
            if (pick < vampireData.appearanceWeight + survivorData.appearanceWeight)
            {
                return survivorData;
            }
            if (pick < vampireData.appearanceWeight + survivorData.appearanceWeight + imposterData.appearanceWeight)
            {
                return imposterData;
            }
            
            return null;

        }
        
        #endregion

        public static void DecideVisitorChoice(VisitorType visitor, RejectionChoice choice)
        {
            switch (visitor)
            {
                case VisitorType.Vampire:
                    if (choice == RejectionChoice.Yes)
                    {
                        GameManager.CurrentGameStats.VampiresAccepted++;
                        GameManager.Instance.RemoveGuard();
                        return; //TODO: Make the Vampire eliminate 2 guards
                    }
                    else
                    {
                        GameManager.CurrentGameStats.VampiresRejected++;
                        return;
                    }
                    break;
                case VisitorType.Survivor:
                    if (choice == RejectionChoice.Yes)
                    {
                        GameManager.Instance.AddSurvivor();
                        GameManager.CurrentGameStats.SurvivorsAccepted++;
                    }
                    else
                    {
                        GameManager.CurrentGameStats.SurvivorsRejected++;
                    }
                    break;
                case VisitorType.Imposter:
                    if (choice == RejectionChoice.Yes)
                    {
                        GameManager.CurrentGameStats.ImpostersAccepted++;
                        GameManager.Instance.RemoveGuard();
                    }
                    else
                    {
                        GameManager.CurrentGameStats.ImpostersRejected++;
                    }
                    break;
            }
        }
    }
}