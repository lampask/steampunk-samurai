﻿using System;
using System.Collections;
using System.Linq;
using Definitions;
using Gameplay;
using Gameplay.Input;
using Management;
using Models;
using UI;
using UnityEngine;
using UnityEngine.Events;

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
        public bool dead;
        
        private bool _reversed;
        public bool reversed
        {
            get => _reversed;
            set
            {
                if (value != _reversed)
                {
                    _reversed = value;
                    Reverse();
                }
            }
        }

        private bool _invincible;

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

        public float attackRange = 2f;
        public int attackDamage = 10;
        
        private bool _playerIndexSet;
        private Rigidbody2D _rb;
        private Animator _anim;

        public Tuple<PlayerModel, PlayerDefinition> playerReference;

        public UnityEvent finishedDissolving;

        public float deathZone = -20f;
        
        // Use this for initialization
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponentInChildren<Animator>();
            finishedDissolving = new UnityEvent();
            finishedDissolving.AddListener(() => gameObject.SetActive(false));
        }

        private void Start()
        {
            if (controlledBy != null)
                _prevState = controlledBy.GenerateFrameStateClone();
        }

        private void FixedUpdate()
        {
            // vibration testing DEPRECATED
            //GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
        }

        public SpriteRenderer head;
        public Transform sword;

        private Control.ControlSnapshot _prevState;
        
        // Parameters
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Jump1 = Animator.StringToHash("Jump");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        private void Update()
        {
            _anim.SetFloat(Horizontal, Math.Abs(_rb.velocity.x));
            _anim.SetFloat(Vertical, Math.Abs(_rb.velocity.y));
            
            Move(new Vector2(controlledBy.globalAxis.x, controlledBy.globalAxis.y));
            
            if (!_prevState.a && controlledBy.a)
            {
                if (onGround) Jump();
                else if (playerReference.Item1.energy >= 100)
                {
                    playerReference.Item1.energy = 0;
                    Jump();
                }
            }

            if (!_prevState.b && controlledBy.b)
            {
                Attack();
            }

            if(_rb.velocity.y < 0) _rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            else if(_rb.velocity.y > 0 && !controlledBy.a) _rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            
            var position = transform.position;
            onGround = Physics2D.OverlapCircle((Vector2)position + bottomOffset, collisionRadius, groundLayer);
            _anim.SetBool(Grounded, onGround);
            onWall = Physics2D.OverlapCircle((Vector2)position + rightOffset, collisionRadius, groundLayer) 
                     || Physics2D.OverlapCircle((Vector2)position + leftOffset, collisionRadius, groundLayer);

            onRightWall = Physics2D.OverlapCapsule((Vector2) position + rightOffset, new Vector2(collisionRadius, collisionHeight), CapsuleDirection2D.Vertical,
                collisionRadius, groundLayer);
            onLeftWall = Physics2D.OverlapCapsule((Vector2) position + leftOffset, new Vector2(collisionRadius, collisionHeight), CapsuleDirection2D.Vertical,
                collisionRadius, groundLayer);

            _prevState = controlledBy.GenerateFrameStateClone();

            if (playerReference.Item1.energy < playerReference.Item2.maxEnergy)
            {
                playerReference.Item1.energy += 1;
            }
            
            if (transform.position.y < deathZone)
                Die();
            
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

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)sword.position, attackRange);
        }

        private void Move(Vector2 dir)
        {
            if (!canMove)
                return;
            dir = NormalizeInput(dir);
            if (dir.x > 0.1f)
            {
                reversed = false;
            } else if (dir.x < -0.1f)
            {
                reversed = true;
            }
            var velocity = _rb.velocity;
            velocity = new Vector2((dir.x * speed), velocity.y);
            _rb.velocity = velocity;
        }

        private void Jump() {
            _anim.SetTrigger(Jump1);
           
            var velocity = _rb.velocity;
            velocity = new Vector2(velocity.x, 0);
            velocity += Vector2.up * jumpForce;
            _rb.velocity = velocity;
        }

        private void Attack()
        {
            _anim.SetTrigger(Attack1);
            Physics2D.OverlapCircleAll(sword.position, 0,1 << 9).ToList().ForEach(c =>
            {
                var vectorToCollider = (c.transform.position - transform.position).normalized;
                // 180 degree arc, change 0 to 0.5 for a 90 degree "pie"
                if( Vector3.Dot(vectorToCollider, transform.forward) > 0.6f)
                {
                    if (c.TryGetComponent(out PlayerBehaviour pb))
                    {
                        pb.TakeDamage(this);
                    }
                }
            });
        }

        private void Reverse()
        {
            var rotation = transform.eulerAngles;
            var rot = Quaternion.Euler(new Vector3(rotation.x, (rotation.y + 180) % 360, rotation.z));
            transform.rotation = rot;
        }

        public void TakeDamage(PlayerBehaviour attacker)
        {
            playerReference.Item1.health -= attacker.attackDamage;
        }

        public void Die()
        {
            if (dead)
                return;
            dead = true;
            canMove = false;
            playerReference.Item1.health = 0;
            playerReference.Item1.energy = 0;
            GetComponentsInChildren<SpriteDissolver>().ToList().ForEach(sd => sd.isDissolving = true);
        }
        private Vector2 NormalizeInput(Vector2 dir) => new Vector2(dir.x < 2f && dir.x > -2f ? 0 : dir.x / 30, dir.y < 2f && dir.y > -2f ? 0 : dir.y / 30);
        
    }
}