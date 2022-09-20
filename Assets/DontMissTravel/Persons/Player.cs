using System.Collections;
using DontMissTravel.Data;
using DontMissTravel.Ui;
using UnityEngine;
using Action = DontMissTravel.Data.Action;
using Direction = DontMissTravel.Data.Direction;
using GameState = DontMissTravel.Data.GameState;

namespace DontMissTravel.Persons
{
    public class Player : PersonController
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] protected Sprite[] _grabSprites;
        [SerializeField] protected Sprite[] _meetSprites;

        private Hud _hud;
        private Rigidbody2D _objectForPull;
        private FixedJoint2D _springJoint;
        
        private void Start()
        {
            _hud = Hud.Instance;
            _springJoint = GetComponent<FixedJoint2D>();
        }

        private void Update()
        {
            if (GameController.Instance.GameState != GameState.Play)
            {
                return;
            }
            
            if (!GameController.Instance.IsPhone)
            {
                KeyBoardController();
            }
        }

        public void OnMeetFinish()
        {
            _isAlreadyMeet = false;
        }
        
        //Control for PC
        private void KeyBoardController()
        {
            var directionX = Input.GetAxis("Horizontal");
            var directionY = Input.GetAxis("Vertical");
            Direction direction = Direction.Idle;

            if (directionX > 0)
            {
                direction = Direction.Right;
            }
            else if (directionX < 0)
            {
                direction = Direction.Left;
            }

            if (directionY > 0)
            {
                direction = Direction.Up;
            }
            else if (directionY < 0)
            {
                direction = Direction.Down;
            }

            if (directionX == 0 && directionY == 0)
            {
                direction = Direction.Idle;
            }

            base.Move(direction, _speed);
            
            if (Input.GetKeyDown(KeyCode.Space) && _objectForPull != null)
            {
                PullObject();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                DisablePulling();
            }
        }

        private bool _isAlreadyMeet;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            string otherTag = other.gameObject.tag;
            switch (otherTag)
            {
                case Constants.Tags.Gate:
                {
                    SwitchSprites(Action.Stay);
                    break;
                }
                case Constants.Tags.Enemy:
                {
                    if (_isAlreadyMeet)
                    {
                        return;
                    }
                    
                    EnemyName enemyName = other.gameObject.GetComponent<Enemy>().EnemyName;
                    SwitchSprites(Action.Stay);
                    GameController.Instance.OpenHideGame(true, enemyName);
                    Destroy(other.gameObject, 1f);
                    _isAlreadyMeet = true;
                    break;
                }

                case Constants.Tags.Obstacle:
                {
                    SwitchSprites(Action.Grab);
                    _speed /= 2;
                    if (GameController.Instance.IsPhone) _hud.PullButton.SetActive(true);
                    _objectForPull = other.rigidbody;
                    break;
                }

                // case "Gate":
                // {
                //     other.collider.isTrigger = true;
                //     _hud.ShowWidow(_hud.WinWindow);
                //     _gameController.State = GameState.Pause;
                //     LevelGenerator.Instance.LevelUp();
                //     //Finish Game
                //     break;
                // }
            }
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            var otherTag = other.gameObject.tag;

            switch (otherTag)
            {
                case Constants.Tags.Enemy:
                {
                    break;
                }
                case Constants.Tags.Obstacle:
                {
                    _speed *= 2;
                    if (GameController.Instance.IsPhone) _hud.PullButton.SetActive(false);
                    _objectForPull = null;
                    break;
                }
                case Constants.Tags.Gate:
                {
                    //Finish Game
                    break;
                }
            }
        }

        public void UpgradeSpeed()
        {
            _speed *= Constants.SpeedParameters.SpeedIncrease;
            _hud.SetBoost();
        }

        public void MakeInvisible()
        {
            _collider2D.isTrigger = true;
            _hud.SetGreenTicket();
            StartCoroutine(nameof(MakePlayerVisible));
        }

        private IEnumerator MakePlayerVisible()
        {
            yield return new WaitForSeconds(5f);
            _collider2D.isTrigger = false;
        }

        protected override void SwitchSprites(Action action)
        {
            switch (action)
            {
                case Action.Grab:
                    CurrentSprites = _grabSprites;
                    break;
                case Action.Meet:
                    CurrentSprites = _meetSprites;
                    break;
                default:
                    base.SwitchSprites(action);
                    break;
            }
        }

        private void PullObject()
        {
            _targetImage.sprite = _grabSprites[0];
            _springJoint.enabled = true;
            _springJoint.connectedBody = _objectForPull;
        }

        private void DisablePulling()
        {
            _springJoint.connectedBody = null;
            _springJoint.enabled = false;
        }
    }
}