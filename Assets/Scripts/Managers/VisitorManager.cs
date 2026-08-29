using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using EditorAttributes;
using Player;
using Triggerable;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Visitors.In_House;
using Random = UnityEngine.Random;

namespace Managers
{
    public class VisitorManager : MonoBehaviour
    {
        public static VisitorManager Instance;

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

        [Header("Character")]
        [SerializeField, ReadOnly] private List<Character> collectedCharacters = new();
        [SerializeField] private Transform[] roomPoints;

        [SerializeField, ReadOnly] private List<Transform> _availableRoomPoints = new();

        private void Awake()
        {
            Instance = this;

            entrances = FindObjectsByType<Entrance>();
            _availableEntrances = entrances.ToList();

            // Initially every room point is available.
            _availableRoomPoints = roomPoints.ToList();

            Debug.Log($"Entrances Count = {_availableEntrances.Count}");
            Debug.Log($"Room Points Count = {_availableRoomPoints.Count}");
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
            _targetTime = Random.Range(spawnRate.x, spawnRate.y);
        }

        private void DoSpawning()
        {
            _currentTime += Time.fixedDeltaTime;

            if (_currentTime < _targetTime) return;

            UpdateTargetTime();
            SpawnVisitorAtEntrance();

            _currentTime = 0f;
        }

        private void SpawnVisitorAtEntrance()
        {
            if (_availableEntrances.Count <= 0) return;

            var entrance = _availableEntrances[Random.Range(0, _availableEntrances.Count)];

            _availableEntrances.Remove(entrance);

            entrance.DeliverVisitor(GetRandomVisitor());
        }
        
        public void RemoveCapturedCharacter(Character character)
        {
            if (!character) return;

            var survivor = character.GetComponent<Character>();

            if (survivor)
            {
                RemoveSurvivor(survivor);
                return;
            }

            var player = character.GetComponent<PlayerController>();

            if (player) RemovePlayer();
        }

        #endregion

        #region Visitor Decisions

        public void DecideVisitorChoice(VisitorData visitor, RejectionChoice choice, Entrance entrance = null)
        {
            switch (visitor.visitorType)
            {
                case VisitorType.Vampire:

                    if (choice == RejectionChoice.Yes)
                    {
                        GameManager.CurrentGameStats.VampiresAccepted++;

                        SpawnVampire(vampireData);
                    }
                    else
                    {
                        GameManager.CurrentGameStats.VampiresRejected++;
                    }

                    break;

                case VisitorType.Survivor:

                    if (choice == RejectionChoice.Yes)
                    {
                        GameManager.CurrentGameStats.SurvivorsAccepted++;

                        // Spawn the survivor into the house.
                        if (SpawnSurvivor(visitor))
                        {
                            GameManager.Instance.AddSurvivor();
                        }
                        else
                        {
                            Debug.LogWarning("Survivor accepted, but there are no available room points.");
                        }
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

                        SpawnVampire(imposterData);
                    }
                    else
                    {
                        GameManager.CurrentGameStats.ImpostersRejected++;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(visitor), visitor, null);
            }
        }

        #endregion

        #region Survivor Spawning

        private bool SpawnSurvivor(VisitorData data)
        {
            if (_availableRoomPoints.Count == 0)
            {
                GameManager.Instance.AddSurvivor();
                return false;
            }

            // Pick a random free room.
            var index = Random.Range(0, _availableRoomPoints.Count);
            var roomPoint = _availableRoomPoints[index];

            // Reserve the point immediately.
            _availableRoomPoints.RemoveAt(index);

            // Spawn survivor.
            var survivorObject = Instantiate(survivorData.characterPrefab, roomPoint.position, roomPoint.rotation);

            var character = survivorObject.GetComponent<Character>();

            if (!character)
            {
                Debug.LogError("Survivor prefab does not contain a Character component.");

                // Give the room point back if spawning failed.
                _availableRoomPoints.Add(roomPoint);
                Destroy(survivorObject);

                return false;
            }

            // Tell the survivor which room point it occupies.
            character.Initialize(data, roomPoint);

            collectedCharacters.Add(character);

            return true;
        }

        #endregion

        #region Vampire Spawning

        private void SpawnVampire(VisitorData enemyData)
        {
            if (entrances.Length == 0)
                return;

            var entrance = entrances[Random.Range(0, entrances.Length)];

            var enemyObject = Instantiate(enemyData.characterPrefab,
                entrance.EntrancePoint.position, quaternion.identity);

            var target = GetRandomTarget();
            
            var vampire = enemyObject.GetComponent<VampireController>();

            if (vampire) vampire.OnSpawned(target);
        }
        
        private Character GetRandomTarget()
        {
            // Pick a random survivor if any are available.
            if (collectedCharacters.Count > 0)
            {
                int index = Random.Range(0, collectedCharacters.Count);

                return collectedCharacters[index].GetComponent<Character>();
            }

            // Otherwise, target the player.
            PlayerController player = FindAnyObjectByType<PlayerController>();

            return player ? player.GetComponent<Character>() : null;
        }

        private void RemoveRandomOccupant()
        {
            // Prefer survivors.
            if (collectedCharacters.Count > 0)
            {
                var index = Random.Range(0, collectedCharacters.Count);
                var survivor = collectedCharacters[index];

                RemoveSurvivor(survivor);
                return;
            }

            // No survivors, so target the player.
            RemovePlayer();
        }

        private void RemoveSurvivor(Character survivor)
        {
            if (!survivor) return;

            collectedCharacters.Remove(survivor);

            // Give their room back.
            Transform roomPoint = survivor.RoomPoint;

            if (roomPoint && !_availableRoomPoints.Contains(roomPoint))
            {
                _availableRoomPoints.Add(roomPoint);
            }

            // TODO:
            // Play "lifted out" animation instead of immediately destroying.
            Destroy(survivor.gameObject);

            GameManager.Instance.RemoveGuard();

            Debug.Log("A survivor was lifted out by an enemy.");
        }

        private void RemovePlayer()
        {
            Debug.Log("No survivors available. Enemy targets the player.");

            // TODO:
            // Replace this with whatever should happen to the player.
            // Example:
            // GameManager.Instance.PlayerCaptured();
        }

        #endregion

        #region Entrances

        public void ReturnEntrance(Entrance entrance)
        {
            if (!_availableEntrances.Contains(entrance)) _availableEntrances.Add(entrance);
        }

        #endregion

        #region Visitor Selection

        private VisitorData GetRandomVisitor()
        {
            var pick = Random.value;

            if (pick < vampireData.appearanceWeight)
                return vampireData;

            if (pick < vampireData.appearanceWeight + survivorData.appearanceWeight)
                return survivorData;

            if (pick < vampireData.appearanceWeight + survivorData.appearanceWeight + imposterData.appearanceWeight)
                return imposterData;

            return null;
        }

        #endregion
    }
}