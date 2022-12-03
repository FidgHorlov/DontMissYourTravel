using System.Collections;
using DontMissTravel.Data;
using DontMissTravel.Ui;
using UnityEngine;
using Direction = DontMissTravel.Data.Direction;
using GameState = DontMissTravel.Data.GameState;

namespace DontMissTravel.Persons
{
    public class Player : PersonController
    {
        private const int DragIncreaseDecreaseSpeed = 2;
        private const float ObstacleLightMass = 5f;
        
        [SerializeField] private float _speed = 10f;

        private Hud _hud;
        private GameController _gameController;
        private KeepDataManager _keepDataManager;
        
        private Rigidbody2D _objectForPull;
        private FixedJoint2D _springJoint;
        private bool _powerPulling;
        private float _previousObstacleMass;
        private Vector3 _defaultPosition;

        protected bool IsAlreadyMet;
        
        public bool IsPulling { get; private set; }

#region Monobehaviour

        protected override void Start()
        {
            base.Start();
            _hud = Singleton<Hud>.Instance;
            _gameController = Singleton<GameController>.Instance;
            _keepDataManager = Singleton<KeepDataManager>.Instance;
            
            _springJoint = GetComponent<FixedJoint2D>();
            _defaultPosition = transform.localPosition;
        }

        private void FixedUpdate()
        {
            if (_keepDataManager.GameState != GameState.Play)
            {
                return;
            }
            
            if (!_keepDataManager.IsPhone)
            {
                KeyBoardController();
            }

            CheckCollision();
            if (Input.GetKey(KeyCode.Space) && _objectForPull != null)
            {
                if (_powerPulling)
                {
                    return;
                }
                EnablePowerPull();
            }
            else
            {
                if (_powerPulling)
                {
                    DisablePowerPull();
                }
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            string otherTag = other.gameObject.tag;
            switch (otherTag)
            {
                case Constants.Tags.Gate:
                {
                    SwitchSprites(PersonAction.Stay);
                    break;
                }
                case Constants.Tags.Enemy:
                {
                    if (IsAlreadyMet)
                    {
                        return;
                    }
                    
                    EnemyName enemyName = other.gameObject.GetComponent<Enemy>().EnemyName;
                    SwitchSprites(PersonAction.Stay);
                    _gameController.OpenHideGame(true, enemyName);
                    Destroy(other.gameObject, 0.1f);
                    IsAlreadyMet = true;
                    break;
                }
            }
        }
#endregion

        public void SetDefaultPosition()
        {
            transform.localPosition = _defaultPosition;
            transform.localScale = Vector3.one;
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

        public void OnMeetFinish()
        {
            IsAlreadyMet = false;
        }

        public void ForceStop()
        {
            Move(Direction.Idle, 0f);
        }
        
        private void CheckCollision()
        {
            RaycastHit2D cast = (Physics2D.CircleCast(transform.position, 50f, transform.up, distance: 30f));
            if (!cast.collider.CompareTag(Constants.Tags.Obstacle))
            {
                if (_objectForPull != null)
                {
                    DisableObjectPull();
                }

                return;
            }

            if (_objectForPull != null)
            {
                return;
            }

            EnableObjectPull(cast.rigidbody);
        }

        // Control for PC
        private void KeyBoardController()
        {
            float directionX = Input.GetAxis("Horizontal");
            float directionY = Input.GetAxis("Vertical");
            Direction direction = Direction.Idle;

            switch (directionX)
            {
                case > 0:
                    direction = Direction.Right;
                    break;
                case < 0:
                    direction = Direction.Left;
                    break;
            }

            switch (directionY)
            {
                case > 0:
                    direction = Direction.Up;
                    break;
                case < 0:
                    direction = Direction.Down;
                    break;
            }

            if (directionX == 0 && directionY == 0)
            {
                direction = Direction.Idle;
            }

            base.Move(direction, _speed);
        }

        private IEnumerator MakePlayerVisible()
        {
            yield return new WaitForSeconds(5f);
            _collider2D.isTrigger = false;
        }

        private void EnablePowerPull()
        {
            _powerPulling = true;
            _currentPersonAction = PersonAction.Grab;
            SwitchSprites(PersonAction.Grab);
            if (_objectForPull != null)
            {
                _previousObstacleMass = _objectForPull.mass;
                _objectForPull.mass = ObstacleLightMass;   
            }
            _springJoint.enabled = true;
            _springJoint.connectedBody = _objectForPull;
        }

        private void DisablePowerPull()
        {
            _powerPulling = false;
            _currentPersonAction = PersonAction.None;
            if (_objectForPull != null && _previousObstacleMass != 0)
            {
                _objectForPull.mass = _previousObstacleMass;   
            }
            _springJoint.connectedBody = null;
            _springJoint.enabled = false;
        }

        private void EnableObjectPull(Rigidbody2D other)
        {
            _speed /= DragIncreaseDecreaseSpeed;
            if (_keepDataManager.IsPhone) _hud.TogglePullButton(true);
            _objectForPull = other;
            IsPulling = true;
        }

        private void DisableObjectPull()
        {
            _speed *= DragIncreaseDecreaseSpeed;
            if (_keepDataManager.IsPhone) _hud.TogglePullButton(false);
            _objectForPull = null;
            IsPulling = false;
        }
    }
}