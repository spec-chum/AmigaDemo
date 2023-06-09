using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AmigaDemo;

public struct CopperBar
{
    public int Position;
    public int UpperLimit;
    public int LowerLimit;
    public int Direction;

    public CopperBar(int position, int upperLimit, int lowerLimit, int direction = 1)
    {
        Position = position;
        UpperLimit = upperLimit;
        LowerLimit = lowerLimit;
        Direction = direction;
    }

    public void Update()
    {
        Position += Direction;
        if (Position > UpperLimit || Position < LowerLimit)
        {
            Direction *= -1;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, float layerDepth)
    {
        spriteBatch.Draw(texture, new Rectangle(0, Position, texture.Width, texture.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
    }
}