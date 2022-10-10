using System;
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
        private Rigidbody2D _objectForPull;
        private FixedJoint2D _springJoint;
        private bool _isAlreadyMet;
        private bool _powerPulling;
        private float _previousObstacleMass;
        
        public bool IsPulling { get; private set; }

#region Monobehaviour
        
        private void Start()
        {
            _hud = Hud.Instance;
            _springJoint = GetComponent<FixedJoint2D>();
        }

        private void FixedUpdate()
        {
            if (GameController.Instance.GameState != GameState.Play && GameController.Instance.GameState != GameState.Tutorial)
            {
                return;
            }
            
            if (!GameController.Instance.IsPhone)
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
                    if (_isAlreadyMet)
                    {
                        return;
                    }
                    
                    EnemyName enemyName = other.gameObject.GetComponent<Enemy>().EnemyName;
                    SwitchSprites(PersonAction.Stay);
                    GameController.Instance.OpenHideGame(true, enemyName);
                    Destroy(other.gameObject, 1f);
                    _isAlreadyMet = true;
                    break;
                }
            }
        }
#endregion
        
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
            _isAlreadyMet = false;
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
            Debug.Log($"Enable Power pull");
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
            Debug.Log($"Disable Power pull");
        }

        private void EnableObjectPull(Rigidbody2D other)
        { 
            Debug.Log($"Enable pull");
            _speed /= DragIncreaseDecreaseSpeed;
            if (GameController.Instance.IsPhone) _hud.PullButton.SetActive(true);
            _objectForPull = other;
            IsPulling = true;
        }

        private void DisableObjectPull()
        {
            Debug.Log($"Disable pull");
            _speed *= DragIncreaseDecreaseSpeed;
            if (GameController.Instance.IsPhone) _hud.PullButton.SetActive(false);
            _objectForPull = null;
            IsPulling = false;
        }
    }
}