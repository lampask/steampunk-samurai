using System;
using Gameplay.Input;
using Management;
using Models;
using UI;
using UnityEngine;

namespace Behaviours
{
    public class PlayerBehaviour : Behaviour
    {
        private int _id;
        public int id { get; set; }
        public Control controlledBy;
        
        
        [Header("Layers")]
        public LayerMask groundLayer;

        [Space]
        public bool onGround;
        public bool onWall;
        public bool onRightWall;
        public bool onLeftWall;
        [Space]

        [Header("Collision")]

        public float collisionRadius = 0.25f;
        public float collisionHeight = 2f;
        public Vector2 bottomOffset, rightOffset, leftOffset;

        public bool canMove = true;

        public float jumpForce = 18f;
        public float speed = 35f;
        public float fallMultiplier = 6f;
        public float lowJumpMultiplier = 5f;
        public float movementFactor = 0.6f;
        
        private bool _playerIndexSet;
        private Rigidbody2D _rb;


        // Use this for initialization
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (controlledBy != null)
                _prevState = controlledBy.GenerateFrameStateClone();
        }

        private void FixedUpdate()
        {
            // vibration testing
            //GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
        }


        private Control.ControlSnapshot _prevState;
        private void Update()
        {
            
            Move(new Vector2(controlledBy.globalAxis.x, controlledBy.globalAxis.y));
            
            if (!_prevState.a && controlledBy.a)
            {
                if (onGround) Jump();
            }

            if(_rb.velocity.y < 0) _rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            else if(_rb.velocity.y > 0 && !controlledBy.a) _rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            
            var position = transform.position;
            onGround = Physics2D.OverlapCircle((Vector2)position + bottomOffset, collisionRadius, groundLayer);
            onWall = Physics2D.OverlapCircle((Vector2)position + rightOffset, collisionRadius, groundLayer) 
                     || Physics2D.OverlapCircle((Vector2)position + leftOffset, collisionRadius, groundLayer);

            onRightWall = Physics2D.OverlapCapsule((Vector2) position + rightOffset, new Vector2(collisionRadius, collisionHeight), CapsuleDirection2D.Vertical,
                collisionRadius, groundLayer);
            onLeftWall = Physics2D.OverlapCapsule((Vector2) position + leftOffset, new Vector2(collisionRadius, collisionHeight), CapsuleDirection2D.Vertical,
                collisionRadius, groundLayer);

            _prevState = controlledBy.GenerateFrameStateClone();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var position = transform.position;
            Gizmos.DrawWireSphere((Vector2)position  + bottomOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)position + rightOffset, collisionRadius);
            Gizmos.DrawLine((Vector2)position + rightOffset - new Vector2(0, collisionHeight/2), (Vector2)position + rightOffset + new Vector2(0, collisionHeight/2));
            Gizmos.DrawWireSphere((Vector2)position + leftOffset, collisionRadius);
            Gizmos.DrawLine((Vector2)position + leftOffset - new Vector2(0, collisionHeight/2), (Vector2)position + leftOffset + new Vector2(0, collisionHeight/2));
        }

        private void Move(Vector2 dir)
        {
            dir = NormalizeInput(dir);
            if (!canMove)
                return;
            var velocity = _rb.velocity;
            velocity = new Vector2((dir.x * speed), velocity.y);
            _rb.velocity = velocity;
        }

        private void Jump() {
            var velocity = _rb.velocity;
            velocity = new Vector2(velocity.x, 0);
            velocity += Vector2.up * jumpForce;
            _rb.velocity = velocity;
        }

        private Vector2 NormalizeInput(Vector2 dir) => new Vector2(dir.x < 2f && dir.x > -2f ? 0 : dir.x / 30, dir.y < 2f && dir.y > -2f ? 0 : dir.y / 30);
        
    }
}