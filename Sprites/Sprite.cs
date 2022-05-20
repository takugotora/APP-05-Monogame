using App05MonoGame.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace App05MonoGame.Models
{
    /// <summary>
    /// This is a basic sprite which has a single image which
    /// can be scaled and rotated around an origin.  The Bounding
    /// Box is the Rectangle the image occupies, and the Bounday
    /// if it exists is the area inside outside which the Sprite can
    /// not move.  Direction is a Vector such as (0, 1) which indicate
    /// the down direction, and Speed is the rate of movement.  A
    /// Speed of 60 is one pixel per second.  The Sprite can only
    /// move if it is Active and Alive.
    /// </summary>
    /// <author>
    /// Modified by Takudzwa Gotora (20/05/2022)
    /// </author>
    public class Sprite: ICloneable
    {
        #region Properties

        // Single Image
        public Texture2D Image { get; set; }

        public Vector2 Position { get; set; }

        // A rectangle limiting where the sprite can move
        public Rectangle Boundary { get; set; }

        // Speed = 60 is 1 Pixel/second
        public float Speed { get; set; }

        public Vector2 Origin { get; set; }

        public float Rotation { get; set; }

        public float RotationSpeed { get; set; }

        public Vector2 Direction { get; set; }

        public float Scale { get; set; }

        public SpriteFont TextFont { get; set; }

        public bool IsVisible { get; set; }

        public bool IsAlive { get; set; }

        public bool IsActive { get; set; }

        public int Width
        {
            get { return Image.Width; }
        }

        public int Height
        {
            get { return Image.Height; }
        }

        // The rectangle occupied by the unscaled image
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle
                (
                    (int)Position.X,
                    (int)Position.Y,
                    (int)(Width * Scale), (int)(Height * Scale)
                );
            }
        }
        
        public KeyboardState CurrentKey { get; set; }

        public KeyboardState PreviousKey { get; set; }

        public float RotationVelocity { get; set; }

        public float LinearVelocity { get; set; }

        public Sprite Parent { get; set; }

        public float LifeSpan { get; set; }

        #endregion

        #region Attributes

        protected float deltaTime;

        protected bool debug = false;

        #endregion

        /// <summary>
        /// Create a sprite that is active, alive and
        /// visible with no speed, rotation or scale
        /// facing east (to the right)
        /// </summary>
        public Sprite()
        {
            if (Image != null)
                Origin = new Vector2(Width / 2, Height / 2);
            else Origin = Vector2.Zero;

            Direction = new Vector2(1, 0);
            Speed = 0;

            IsVisible = true;
            IsAlive = true;
            IsActive = true;

            Scale = 1;
            Rotation = 0;
            RotationSpeed = 0;            
        }

        /// <summary>
        /// Constructor sets the main image and starting position of
        /// the Sprite as a Vector2
        /// </summary>
        public Sprite(Texture2D image, int x, int y) : this()
        {
            Image = image;
            Position = new Vector2(x, y);
        }

        public bool HasCollided(Sprite other)
        {
            if(BoundingBox.Intersects(other.BoundingBox))
            {
                int margin = 8 * (int)Scale;
                Rectangle overlap = Rectangle.Intersect(BoundingBox, other.BoundingBox);
                if(overlap.Width > margin)
                    return true;
            }

            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive && IsAlive)
            {
                Rotation += MathHelper.ToRadians(RotationSpeed);
                Vector2 newPosition = Position + ((Direction * Speed) * deltaTime);

                if (Boundary.Width == 0 || Boundary.Height == 0)
                {
                    Position = newPosition;
                }
                else if (newPosition.X >= Boundary.X &&
                    newPosition.Y >= Boundary.Y &&
                    newPosition.X + Width < Boundary.X + Boundary.Width &&
                    newPosition.Y + Height < Boundary.Y + Boundary.Height)
                {
                    Position = newPosition;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (debug)
            {
                TextHelper.DrawString(
                    $"({Position.X:0},{Position.Y:0})", Position);
            }

            if (Origin == Vector2.Zero)
                Origin = new Vector2(Width / 2, Height / 2);

            if(IsVisible)
            {
                spriteBatch.Draw
                    (Image,
                     Position,
                     null,
                     Color.White, Rotation, Origin,
                     Scale, SpriteEffects.None, 1);
            }
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
