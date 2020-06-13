using Definitions;
using Management;
using Models;
using UI;
using UnityEngine;
using XInputDotNetPure;

namespace Behaviours
{
    public class PlayerBehaviour : Behaviour
    {
        private int _id;
        public int id
        {
            get => _id;
            set
            {
                if (_id == 0)
                    _id = value;
            }
        }
        
        [Header("Layers")]
        public LayerMask groundLayer;

        public PlayerModel model;
        public PlayerDefinition definition;
        public Bar healtBar;
        
        [Space]
        public bool onGround;
        public bool onWall;
        public bool onRightWall;
        public bool onLeftWall;
        public int wallSide;

        [Space]

        [Header("Collision")]

        public float collisionRadius = 0.25f;
        public Vector2 bottomOffset, rightOffset, leftOffset;

        public bool canMove = true;

        public float jumpForce = 6f;
        public float speed = 4f;
        public float fallMultiplier = 3f;
        public float lowJumpMultiplier = 2.5f;
        public float movementFactor = 0.2f;
        
        private bool playerIndexSet;
        private PlayerIndex playerIndex;
        private GamePadState state;
        private GamePadState prevState;
        private Rigidbody2D rb;
        public float a; 
        
        
        // Use this for initialization
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerIndex = LocalGameManager.definitions.players[id].controls;
        }

        private void FixedUpdate()
        {
            // vibration testing
            GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
        }
        
        private void Update()
        {
            
            prevState = state;
            state = GamePad.GetState(playerIndex);

            Move(new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y));
            
            if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
            {
                if (onGround) Jump();
            }

            if(rb.velocity.y < 0) rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            else if(rb.velocity.y > 0 && state.Buttons.A != ButtonState.Pressed) rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            
            var position = transform.position;
            onGround = Physics2D.OverlapCircle((Vector2)position + bottomOffset, collisionRadius, groundLayer);
            onWall = Physics2D.OverlapCircle((Vector2)position + rightOffset, collisionRadius, groundLayer) 
                     || Physics2D.OverlapCircle((Vector2)position + leftOffset, collisionRadius, groundLayer);

            onRightWall = Physics2D.OverlapCircle((Vector2)position + rightOffset, collisionRadius, groundLayer);
            onLeftWall = Physics2D.OverlapCircle((Vector2)position + leftOffset, collisionRadius, groundLayer);

            wallSide = onRightWall ? -1 : 1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var position = transform.position;
            Gizmos.DrawWireSphere((Vector2)position  + bottomOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)position + rightOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)position + leftOffset, collisionRadius);
        }

        private void Move(Vector2 dir) {
            if (!canMove)
                return;
            var velocity = rb.velocity;
            velocity = new Vector2((dir.x * speed), velocity.y);
            rb.velocity = velocity;
        }

        private void Jump() {
            var velocity = rb.velocity;
            velocity = new Vector2(velocity.x, 0);
            velocity += Vector2.up * jumpForce;
            rb.velocity = velocity;
        }
    }
}