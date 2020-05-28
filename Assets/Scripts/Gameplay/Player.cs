using System;
using UnityEngine;
using XInputDotNetPure;

namespace Gameplay
{
    public class Player : MonoBehaviour
    {

        [Header("Layers")]
        public LayerMask groundLayer;

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
        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            // SetVibration should be sent in a slower rate.
            // Set vibration according to triggers
            GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
        }

        // Update is called once per frame
        private void Update()
        {
            a = Mathf.Clamp(1f / rb.velocity.y, 0, 1);
            // Find a PlayerIndex, for a single player game
            // Will find the first controller that is connected ans use it
            if (!playerIndexSet || !prevState.IsConnected)
            {
                for (var i = 0; i < 4; ++i)
                {
                    var testPlayerIndex = (PlayerIndex)i;
                    var testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex = testPlayerIndex;
                        playerIndexSet = true;
                    }
                }
            }

            prevState = state;
            state = GamePad.GetState(playerIndex);

            Move(new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y));
        
            // Detect if a button was pressed this frame
            if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
            {
                if (onGround)
                    Jump();
                //GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
            }

            if(rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            } else if(rb.velocity.y > 0 && state.Buttons.A != ButtonState.Pressed)
            {
                rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            }
    
            // Detect if a button was released this frame
            /* if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released)
        {
            GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        } */

            // Make the current object turn
            //transform.localRotation *= Quaternion.Euler(0.0f, state.ThumbSticks.Left.X * 25.0f * Time.deltaTime, 0.0f);
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
            //if (rb.velocity.y < 0) {
            var velocity = rb.velocity;
            velocity = new Vector2(velocity.x, 0);
            velocity += Vector2.up * jumpForce;
            rb.velocity = velocity;
            //}
        }
    }
}
