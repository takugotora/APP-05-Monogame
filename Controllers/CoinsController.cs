using App05MonoGame.Helpers;
using App05MonoGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace App05MonoGame.Controllers
{
    public enum CoinColours
    {
        Copper = 100,
        Silver = 200,
        Gold = 500
    }

    /// <summary>
    /// This class creates a list of coins which
    /// can be updated and drawn and checked for
    /// collisions with the player sprite
    /// </summary>
    /// <authors>
    /// Derek Peacock
    /// Modified by Takudzwa Gotora 20/05/2022
    /// </authors>
    public class CoinsController
    {
        private SoundEffect coinEffect;
        private int coinValue;
        private double maxTime;
        private double timer;
        private AnimatedSprite spriteCoinTemplate;

        private readonly List<AnimatedSprite> Coins;        

        /// <summary>
        /// Constructor; initialises a new list of coins which
        /// will spawn every 3 seconds and the timer. 
        /// </summary>
        public CoinsController()
        {
            Coins = new List<AnimatedSprite>();
            maxTime = 3.0;
            timer = maxTime;
        }

        /// <summary>
        /// Create an animated sprite of a copper coin
        /// which could be collected by the player for a score
        /// </summary>
        public void CreateCoin(GraphicsDevice graphics, Texture2D coinSheet)
        {
            coinEffect = SoundController.GetSoundEffect("Coin");
            coinValue = (int)CoinColours.Copper;
            Animation animation = new Animation("coin", coinSheet, 8);

            AnimatedSprite coin = new AnimatedSprite()
            {
                Animation = animation,
                Image = animation.SetMainFrame(graphics),
                Scale = 2.0f,
                Position = new Vector2(600, 100),
                Speed = 0,
            };

            spriteCoinTemplate = coin;

            Coins.Add(coin);
        }

        /// <summary>
        /// Checks if a coin has collided with a player (i.e. the 
        /// player walks over it to pick it up) and if it has,
        /// a sound effect will play and the coin will disappear
        /// from the screen.
        /// </summary>
        /// <returns>
        /// The value of the coin to add onto the player's current
        /// score.
        /// </returns>
        public int HasCollided(AnimatedPlayer player)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                if (coin.HasCollided(player) && coin.IsAlive)
                {
                    coinEffect.Play();

                    coin.IsActive = false;
                    coin.IsAlive = false;
                    coin.IsVisible = false;

                    return coinValue;
                }
            }

            return 0;
        }

        /// <summary>
        /// Spawns coins into the game at random positions every 3
        /// seconds. 
        /// </summary>
        public void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0)
            {
                int x = RandomNumber.Generator.Next(1000) + 100;
                int y = RandomNumber.Generator.Next(500) + 100;

                AnimatedSprite coin = new AnimatedSprite()
                {
                    Animation = spriteCoinTemplate.Animation,
                    Image = spriteCoinTemplate.Image,
                    Scale = spriteCoinTemplate.Scale,
                    Position = new Vector2(x, y),
                    Speed = 0,
                };

                Coins.Add(coin);
                timer = maxTime;
            }

            foreach(AnimatedSprite coin in Coins)
            {
                coin.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                coin.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Clears the list of coins, used when the game is
        /// restarted.
        /// </summary>
        public void Clear()
        {
            Coins.Clear();
        }
    }
}
