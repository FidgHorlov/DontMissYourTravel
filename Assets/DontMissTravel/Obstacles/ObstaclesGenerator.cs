using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DontMissTravel.Obstacles
{
    public class ObstaclesGenerator : MonoBehaviour
    {
#region Serialized Values

        [SerializeField] private RectTransform _player;

        [Header("Obstacles")] [SerializeField] private Transform _obstacleFolder;
        [SerializeField] private List<GameObject> _obstacles;

        [SerializeField] private GameObject _obstacle;
        [SerializeField] private GameObject[] _boxes;
        [SerializeField] private int _maxObstacles = 7;

#endregion

        private GameController _gameController;
        private List<Vector2> _environmentPositionList;
        private List<int> _generatedValues;

        private float _cellSize;

        private void Start()
        {
            _generatedValues = new List<int>();
            _gameController = GameController.Instance;
            
            for (int i = 0; i < _maxObstacles; i++)
            {
                GenerateObstacle();
            }

            FillAvailablePositions();
        }

        private void FillAvailablePositions()
        {
            Debug.Log($"Generated values: {_generatedValues.Count}");
            for (int index = 0; index < _obstacles.Count; index++)
            {
                if (_generatedValues.Any(value => value.Equals(index)))
                {
                    continue;
                }

                _gameController.AddAvailablePositions(_obstacles[index].transform.position);
            }
        }

        private void GenerateObstacle()
        {
            int randomValue = Random.Range(0, _obstacles.Count);
            foreach (int generatedValue in _generatedValues)
            {
                while (randomValue.Equals(generatedValue))
                {
                    randomValue = Random.Range(0, _obstacles.Count);
                }
            }
            
            _generatedValues.Add(randomValue);
            _obstacles[randomValue].SetActive(true);
        }

#if UNITY_EDITOR
        [ContextMenu("Create obstacles")]
        private void CreateObstaclesTemp()
        {
            _cellSize = _boxes[0].GetComponent<RectTransform>().sizeDelta.x;
            _obstacles = new List<GameObject>();
            _environmentPositionList = new List<Vector2>();


            FillData();
            foreach (Vector2 position in _environmentPositionList)
            {
                GameObject newObstacle = PrefabUtility.InstantiatePrefab(_obstacle, _obstacleFolder) as GameObject;
                if (newObstacle == null)
                {
                    continue;
                }

                newObstacle.transform.position = position;
                _obstacles.Add(newObstacle);
                newObstacle.SetActive(false);
            }
        }

        [ContextMenu("Delete all obstacles")]
        private void DeleteAllObstacles()
        {
            if (_obstacles == null || _obstacles.Count == 0)
            {
                foreach (GameObject obj in _obstacleFolder.GetComponentsInParent<GameObject>())
                {
                    DestroyImmediate(obj);
                }
            }
            else
            {
                foreach (GameObject obstacle in _obstacles)
                {
                    DestroyImmediate(obstacle);
                }
            }

            _obstacles = new List<GameObject>();
            _environmentPositionList = new List<Vector2>();
        }

        private void FillData()
        {
            _environmentPositionList = new List<Vector2>();
            foreach (GameObject environment in _boxes)
            {
                Vector2 newPosition = environment.transform.position;
                AddValue(newPosition, true);
                AddValue(newPosition, false);
            }

            Vector2 firstPosition = _boxes[0].transform.position;
            firstPosition.x -= _cellSize;
            firstPosition.y += _cellSize;
            AddNewPosition(firstPosition);

            Vector2 lastPosition = _boxes[_boxes.Length - 1].transform.position;
            lastPosition.x += _cellSize;
            lastPosition.y -= _cellSize;
            AddNewPosition(lastPosition);

            Vector2 playerPosition = _player.position;
            foreach (Vector2 position in _environmentPositionList)
            {
                if (Vector2.Distance(playerPosition, position) < _cellSize)
                {
                    Debug.Log($"position: {position}");
                    _environmentPositionList.Remove(position);
                    return;
                }
            }


            Debug.LogError($"Created -> {_environmentPositionList.Count} vectors");
        }

        private void AddValue(Vector2 position, bool isAddition)
        {
            Vector2 defaultPosition = position;
            if (isAddition)
            {
                position.x += _cellSize;
                AddNewPosition(position);

                position = defaultPosition;
                position.x += _cellSize;
                position.y += _cellSize;
                AddNewPosition(position);

                position = defaultPosition;
                position.y += _cellSize;
                AddNewPosition(position);
            }
            else
            {
                position.x -= _cellSize;
                AddNewPosition(position);

                position = defaultPosition;
                position.x -= _cellSize;
                position.y -= _cellSize;
                AddNewPosition(position);

                position = defaultPosition;
                position.y -= _cellSize;
                AddNewPosition(position);
            }
        }

        private void AddNewPosition(Vector2 offset)
        {
            foreach (Vector2 position in _environmentPositionList)
            {
                if (position.Equals(offset))
                {
                    return;
                }
            }

            _environmentPositionList.Add(offset);
            Debug.Log($"New position added -> {offset}");
        }
#endif
    }
}