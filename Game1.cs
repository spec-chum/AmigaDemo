using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AmigaDemo;

public sealed class Game1 : Game
{
    private const float PI_OVER_2 = MathF.PI / 2f;
    private const float TO_RADIANS = MathF.PI / 180f;
    private const int SCALE = 6;
    private const int SCREEN_WIDTH = 320;
    private const int SCREEN_HEIGHT = 200;
    private const int CENTRE_X = SCREEN_WIDTH / 2;
    private const int CENTRE_Y = SCREEN_HEIGHT / 2;
    private const int WINDOW_WIDTH = SCREEN_WIDTH * SCALE;
    private const int WINDOW_HEIGHT = SCREEN_HEIGHT * SCALE;

    private GraphicsDeviceManager gdm;
    private SpriteBatch spriteBatch;
    private RenderTarget2D framebuffer;
    private Rectangle framebufferDstRect = new(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
    private BitmapFont font;
    private CopperBar copperBar1;
    private CopperBar copperBar2;
    private Texture2D copperUp;
    private Texture2D copperDown;
    private Texture2D chequered;
    private Texture2D gradient;
    private float theta = 0;
    private int scrollOffset = SCREEN_WIDTH + 32;
    private const string ScrollerText = "Amiga Style Scroller with FNA";

    public Game1()
    {
        gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = WINDOW_WIDTH;
        gdm.PreferredBackBufferHeight = WINDOW_HEIGHT;

        IsFixedTimeStep = true;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        framebuffer = new(GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
        font = new(Content.Load<Texture2D>("font.png"), 32, 25, 48);

        InitialiseTextures();
        InitialiseCopperBars(5);

        base.Initialize();
    }

    private void InitialiseTextures()
    {
        chequered = CreateCheckerboardTexture(Color.Black, Color.DarkGreen, SCREEN_WIDTH * 2, SCREEN_HEIGHT, 16, 16);
        gradient = CreateGradientTexture(Color.Black, Color.DarkGreen, SCREEN_WIDTH, SCREEN_HEIGHT - 35);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        GetInput();
        UpdateCopperBars();

        font.WasSmallChar = false;
        theta += 0.08f;

        scrollOffset--;
        if (scrollOffset < (font.CellWidth * -ScrollerText.Length))
        {
            scrollOffset = SCREEN_WIDTH + 32;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(framebuffer);
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        DrawBackground(theta);
        DrawFloor();
        DrawScroller(ScrollerText, scrollOffset, theta);
        copperBar1.Draw(spriteBatch, copperUp, 1);
        copperBar2.Draw(spriteBatch, copperDown, 0);
        spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
        spriteBatch.Draw(framebuffer, framebufferDstRect, Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    private void GetInput()
    {
        var kb = Keyboard.GetState();
        if (kb.IsKeyDown(Keys.Escape))
        {
            Exit();
        }
    }

    private Texture2D CreateCheckerboardTexture(Color color1, Color color2, int width, int height, int checkSizeX, int checkSizeY)
    {
        var texture = new Texture2D(GraphicsDevice, width, height);
        var colorData = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            int checkY = y / checkSizeY;
            for (int x = 0; x < width; x++)
            {
                int checkX = x / checkSizeX;
                colorData[y * width + x] = ((checkX + checkY) % 2 == 0) ? color1 : color2;
            }
        }

        texture.SetData(colorData);
        return texture;
    }

    private Texture2D CreateGradientTexture(Color topColor, Color bottomColor, int width, int height)
    {
        var texture = new Texture2D(GraphicsDevice, width, height);
        var colorData = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            var color = Color.Lerp(topColor, bottomColor, t);

            for (int x = 0; x < width; x++)
            {
                colorData[y * width + x] = color;
            }
        }

        texture.SetData(colorData);
        return texture;
    }

    public Texture2D CreateDoubleGradientTexture(Color topColor, Color bottomColor, int width, int height)
    {
        var texture = new Texture2D(GraphicsDevice, width, height);
        var colorData = new Color[width * height];

        Color color;
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            color = y < height / 2 ? Color.Lerp(topColor, bottomColor, t) : Color.Lerp(bottomColor, topColor, t);

            for (int x = 0; x < width; x++)
            {
                colorData[y * width + x] = color;
            }
        }

        texture.SetData(colorData);
        return texture;
    }

    private void InitialiseCopperBars(int height)
    {
        copperBar1 = new(CENTRE_Y - 40, CENTRE_Y + 32, CENTRE_Y - 40, 1);
        copperBar2 = new(CENTRE_Y + 32, CENTRE_Y + 32, CENTRE_Y - 40, -1);
        copperUp = CreateDoubleGradientTexture(new Color(64, 0, 0, 255), Color.Red, SCREEN_WIDTH, height);
        copperDown = CreateDoubleGradientTexture(new Color(32, 0, 0, 255), Color.DarkRed, SCREEN_WIDTH, height);
    }

    private void UpdateCopperBars()
    {
        copperBar1.Update();
        copperBar2.Update();
    }

    private void DrawScroller(in string text, int scrollOffset, float theta)
    {
        int smallCharOffset = 0;

        for (int i = 0; i < text.Length; i++)
        {
            smallCharOffset += font.WasSmallChar ? 10 : 0;

            float sine = 20 * MathF.Sin(theta + i);
            float rotation = 20 * MathF.Sin(PI_OVER_2 + theta + i) * TO_RADIANS;

            Rectangle srcRect = font[text[i]];
            smallCharOffset += font.WasSmallChar ? 10 : 0;

            // Shadow
            Rectangle dstRect = new(scrollOffset - 8 + (32 * i) - smallCharOffset, (int)(CENTRE_Y  + sine - 8), font.CellWidth, font.CellHeight);
            spriteBatch.Draw(font.texture, dstRect, srcRect, Color.Black * 0.5f, rotation, font.Origin, SpriteEffects.None, 0.1f);

            // Scroller
            dstRect = new(scrollOffset + (32 * i) - smallCharOffset, (int)(CENTRE_Y + sine), font.CellWidth, font.CellHeight);
            spriteBatch.Draw(font.texture, dstRect, srcRect, Color.White, rotation, font.Origin, SpriteEffects.None, 0.5f);

            // Reflection
            dstRect.Y = (int)(70 + CENTRE_Y - sine);
            spriteBatch.Draw(font.texture, dstRect, srcRect, Color.White * 0.1f, -rotation, font.Origin, SpriteEffects.FlipVertically, 0.1f);
        }
    }

    private void DrawBackground(float theta)
    {
        int bgx = (int)(32 * MathF.Cos(theta * 0.6f) - 32);
        int bgy = (int)(32 * MathF.Sin(theta * 0.6f) - 32);
        spriteBatch.Draw(chequered, new Vector2(bgx, bgy), Color.White);
    }

    private void DrawFloor()
    {
        spriteBatch.Draw(gradient, new Vector2(0, (SCREEN_HEIGHT / 2) + 35), Color.White);
    }

    protected override void UnloadContent()
    {
        font.Dispose();
        framebuffer.Dispose();

        base.UnloadContent();
    }
}