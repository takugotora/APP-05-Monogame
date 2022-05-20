using App05MonoGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace App05MonoGame.Controllers
{
    /// <summary>
    /// This class allows the player to be moved using
    /// the arrow keys, setting appropriate input keys.
    /// </summary>
    /// <author>
    /// Derek Peacock & Andrei Cruceru
    /// Modified by Jason Huggins (29/04/2021)
    /// </author>
    public class MovementController
    {
        public InputKeys InputKeys { get; set; }
        
        public MovementController()
        {
            InputKeys = new InputKeys()
            {
                // For directions

                Up = Keys.Up,
                Down = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right,

                // Rotate and Move

                TurnLeft = Keys.A,
                TurnRight = Keys.D,
                Forward = Keys.Space
            };
        }

        public Vector2 ChangeDirection(KeyboardState keyState)
        {
            Vector2 Direction = Vector2.Zero;

            if (keyState.IsKeyDown(InputKeys.Right))
            {
                Direction = new Vector2(1, 0);
            }

            if (keyState.IsKeyDown(InputKeys.Left))
            {
                Direction = new Vector2(-1, 0);
            }

            if (keyState.IsKeyDown(InputKeys.Up))
            {
                Direction = new Vector2(0, -1);
            }

            if (keyState.IsKeyDown(InputKeys.Down))
            {
                Direction = new Vector2(0, 1);
            }

            return Direction;
        }

    }
}
