using App05MonoGame.Controllers;
using App05MonoGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace App05MonoGame
{
    /// <summary>
    /// Different states for the game.
    /// </summary>
    public enum GameState
    {
        STARTING,
        PLAYING,
        WON,
        LOST
    }

    /// <summary>
    /// This simple game has a similar style to Pac-Man
    /// where the player moves around collecting
    /// randomly spawned coins, while trying 
    /// to avoid enemies intending to harm them
    /// by not going into their field of vision.
    /// The player can also fire projectiles
    /// at the enemies to defeat them.
    /// </summary>
    /// <authors>
    /// Derek Peacock & Andrei Cruceru
    /// Modified by Takudzwa Gotora (20/05/2022)
    /// </authors>
    public class App05Game : Game
    {
        #region Constants

        public const int HD_Height = 720;
        public const int HD_Width = 1280;
        public const int MAX_SCORE = 1000;
        public const int MAX_HEALTH = 100;
        public const int NO_HEALTH = 0;
        public const int NO_SCORE = 0;

        #endregion

        #region Attribute

        private readonly GraphicsDeviceManager graphicsManager;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        private SpriteFont arialFont;
        private SpriteFont calibriFont;// change to verdana on Mac

        private Texture2D backgroundImage;
        private SoundEffect flameEffect;

        private readonly CoinsController coinsController;
        private readonly PlayerController playerController;
        private readonly EnemyController enemyController;
        private BulletController bulletController;

        private AnimatedPlayer playerSprite;
        private AnimatedSprite enemySprite;

        private Button restartButton;
        private Button quitButton;

        private GameState gameState;

        #endregion

        /// <summary>
        /// Constructor for the game. Initialises the graphics manager,
        /// makes the mouse cursor visible and also initialises the
        /// relevant controllers.
        /// </summary>
        public App05Game()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            coinsController = new CoinsController();
            playerController = new PlayerController();
            enemyController = new EnemyController();
        }

        /// <summary>
        /// Setup the game window size to 720P 1280 x 720 pixels
        /// Simple fixed playing area with no camera or scrolling
        /// </summary>
        protected override void Initialize()
        {
            graphicsManager.PreferredBackBufferWidth = HD_Width;
            graphicsManager.PreferredBackBufferHeight = HD_Height;

            graphicsManager.ApplyChanges();

            graphicsDevice = graphicsManager.GraphicsDevice;

            gameState = GameState.STARTING;

            base.Initialize();
        }

        /// <summary>
        /// Use Content to load your game images, fonts,
        /// music and sound effects
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundImage = Content.Load<Texture2D>(
                "backgrounds/green_background720p");

            // Load Music and SoundEffects

            SoundController.LoadContent(Content);
            SoundController.PlaySong("Adventure");

            flameEffect = SoundController.GetSoundEffect("Flame");

            // Load Fonts

            arialFont = Content.Load<SpriteFont>("fonts/arial");
            calibriFont = Content.Load<SpriteFont>("fonts/calibri");

            // Load buttons, player and enemy sprites

            SetupButtons();
            SetupAnimatedPlayer();
            SetupEnemy();

            Texture2D coinSheet = Content.Load<Texture2D>("Actors/coin_copper");
            coinsController.CreateCoin(graphicsDevice, coinSheet);
        }

        /// <summary>
        /// Sets up the restart and quit buttons on the game's
        /// user interface.
        /// </summary>
        private void SetupButtons()
        {
            restartButton = new Button(arialFont,
                Content.Load<Texture2D>("Controls/button-icon-png-200"))
            {
                Position = new Vector2(900, 600),
                Text = "Restart",
                Scale = 0.6f
            };

            restartButton.click += RestartButton_click;

            quitButton = new Button(arialFont,
                Content.Load<Texture2D>("Controls/button-icon-png-200"))
            {
                Position = new Vector2(1100, 600),
                Text = "Quit",
                Scale = 0.6f
            };

            quitButton.click += QuitButton_click;
        }

        /// <summary>
        /// Restarts the game when the Restart button in the game
        /// is clicked. The player's score and health is reset to
        /// its initial values, as well as the player and enemy's
        /// positions. The coins are also cleared from the screen.
        /// </summary>
        private void RestartButton_click(object sender, System.EventArgs e)
        {
            playerSprite.Score = NO_SCORE;
            playerSprite.Health = MAX_HEALTH;

            playerController.StartPlayer();
            enemyController.StartEnemy();
            coinsController.Clear();
        }

        /// <summary>
        /// Exits the game when the Quit button in the game
        /// is clicked.
        /// </summary>
        private void QuitButton_click(object sender, System.EventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// This is a Sprite with four animations for the four
        /// directions, up, down, left and right
        /// </summary>
        private void SetupAnimatedPlayer()
        {
            Texture2D playerSheet = Content.Load<Texture2D>
                ("Actors/rsc-sprite-sheet1");

            playerSprite = playerController.CreatePlayer
                (graphicsDevice, playerSheet);

            Texture2D bulletTexture = Content.Load<Texture2D>("Actors/bullet");
            bulletController = new BulletController(bulletTexture);
            bulletController.killEffect = flameEffect;

            playerSprite.BulletController = bulletController;

            playerSprite.Boundary = new Rectangle(0, 0, HD_Width, HD_Height); 
        }

        /// <summary>
        /// This is an enemy Sprite with four animations for the four
        /// directions, up, down, left and right. Walks around in
        /// random directions and will chase the player if they
        /// enter their field of view.
        /// </summary>
        private void SetupEnemy()
        {
            Texture2D enemySheet = Content.Load<Texture2D>
                ("Actors/rsc-sprite-sheet3");

            enemySprite = enemyController.CreateEnemy
                (graphicsDevice, enemySheet);

            enemyController.Player = playerSprite;

            enemySprite.Boundary = new Rectangle(0, 0, HD_Width, HD_Height);
        }

        /// <summary>
        /// Called 60 frames/per second and updates the positions
        /// of all the drawable objects
        /// </summary>
        /// <param name="gameTime">
        /// Can work out the elapsed time since last call if
        /// you want to compensate for different frame rates
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            gameState = GameState.PLAYING;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            restartButton.Update(gameTime);
            quitButton.Update(gameTime);

            playerSprite.Update(gameTime);
            enemyController.Update(gameTime);
            enemyController.HasCollided(playerSprite);

            bulletController.UpdateBullets(gameTime);
            bulletController.HasCollided(enemySprite);

            coinsController.Update(gameTime);
            playerSprite.Score += coinsController.HasCollided(playerSprite);

            // Player wins the game if their score is over 1000.
            if (playerSprite.Score >= MAX_SCORE)
            {
                gameState = GameState.WON;
                enemyController.RemoveEnemy();
            }
            // Player loses the game if their health drops to 0.
            else if (playerSprite.Health == NO_HEALTH)
            {
                gameState = GameState.LOST;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Called 60 frames/per second and Draw all the 
        /// sprites and other drawable images here
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LawnGreen);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);

            restartButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);

            playerSprite.Draw(spriteBatch);
            coinsController.Draw(spriteBatch);
            bulletController.DrawBullets(spriteBatch);
            enemySprite.Draw(spriteBatch);

            DrawGameStatus(spriteBatch);
            DrawGameFooter(spriteBatch);

            if (gameState == GameState.WON)
            {
                DrawGameWinMessage(spriteBatch);
            }
            else if (gameState == GameState.LOST)
            {
                DrawGameLoseMessage(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Display the name fo the game and the current score
        /// and health of the player at the top of the screen
        /// </summary>
        public void DrawGameStatus(SpriteBatch spriteBatch)
        {
            Vector2 topLeft = new Vector2(4, 4);
            string status = $"Score = {playerSprite.Score:##0}";

            spriteBatch.DrawString(arialFont, status, topLeft, Color.White);

            string game = "Coin Chase";
            Vector2 gameSize = arialFont.MeasureString(game);
            Vector2 topCentre = new Vector2((HD_Width / 2 - gameSize.X / 2), 4);
            spriteBatch.DrawString(arialFont, game, topCentre, Color.White);

            string healthText = $"Health = {playerSprite.Health}%";
            Vector2 healthSize = arialFont.MeasureString(healthText);
            Vector2 topRight = new Vector2(HD_Width - (healthSize.X + 4), 4);
            spriteBatch.DrawString(arialFont, healthText, topRight, Color.White);
        }

        /// <summary>
        /// Display the Module, the authors and the application name
        /// at the bottom of the screen
        /// </summary>
        public void DrawGameFooter(SpriteBatch spriteBatch)
        {
            int margin = 20;

            string names = "Derek & Andrei; modified by Jason";
            string app = "App05: MonoGame";
            string module = "BNU CO453-2020";

            Vector2 namesSize = calibriFont.MeasureString(names);
            Vector2 appSize = calibriFont.MeasureString(app);

            Vector2 bottomCentre = new Vector2((HD_Width - namesSize.X) / 2, HD_Height - margin);
            Vector2 bottomLeft = new Vector2(margin, HD_Height - margin);
            Vector2 bottomRight = new Vector2(HD_Width - appSize.X - margin, HD_Height - margin);

            spriteBatch.DrawString(calibriFont, names, bottomCentre, Color.Yellow);
            spriteBatch.DrawString(calibriFont, module, bottomLeft, Color.Yellow);
            spriteBatch.DrawString(calibriFont, app, bottomRight, Color.Yellow);
        }

        /// <summary>
        /// Display a congratulatory message to the player saying
        /// that they've won the game (score is over 1000) and
        /// end.
        /// </summary>
        public void DrawGameWinMessage(SpriteBatch spriteBatch)
        {
            string winMsg = "Congratulations! You have won the game!";
            string exit = "Press the ESC key to exit the game.";

            Vector2 winMsgSize = arialFont.MeasureString(winMsg);
            Vector2 exitSize = arialFont.MeasureString(exit);

            Vector2 centre = new Vector2((HD_Width - winMsgSize.X) / 2, 
                (HD_Height - winMsgSize.Y) / 3);
            Vector2 lowerCentre = new Vector2((HD_Width - exitSize.X) / 2,
                (HD_Height - exitSize.Y) / 2);

            spriteBatch.DrawString(arialFont, winMsg, centre, Color.White);
            spriteBatch.DrawString(arialFont, exit, lowerCentre, Color.White);
        }

        /// <summary>
        /// Display a message to the player saying that they've lost
        /// the game (health has dropped to 0) and end.
        /// </summary>
        public void DrawGameLoseMessage(SpriteBatch spriteBatch)
        {
            string loseMsg = "You have lost the game. Better luck next time!";
            string exit = "Press the ESC key to exit the game.";

            Vector2 loseMsgSize = arialFont.MeasureString(loseMsg);
            Vector2 exitSize = arialFont.MeasureString(exit);

            Vector2 centre = new Vector2((HD_Width - loseMsgSize.X) / 2,
                (HD_Height - loseMsgSize.Y) / 3);
            Vector2 lowerCentre = new Vector2((HD_Width - exitSize.X) / 2,
                (HD_Height - exitSize.Y) / 2);

            spriteBatch.DrawString(arialFont, loseMsg, centre, Color.White);
            spriteBatch.DrawString(arialFont, exit, lowerCentre, Color.White);
        }
    }
}
