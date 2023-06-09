using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AmigaDemo;

public class BitmapFont
{
    public readonly Texture2D texture;
    public readonly int CellWidth;
    public readonly int CellHeight;
    public Vector2 Origin;
    public bool WasSmallChar;

    private List<Rectangle> letterRects;

    public BitmapFont(Texture2D image, int cellWidth, int cellHeight, int numCells = 0)
    {
        texture = image;
        CellWidth = cellWidth;
        CellHeight = cellHeight;
        WasSmallChar = false;

        Origin = new(cellWidth / 2, cellHeight / 2);
        if (numCells == 0)
        {
            numCells = image.Width / cellWidth * image.Height / cellHeight;
        }
        letterRects = new List<Rectangle>(numCells);
        GenerateRects(numCells);
    }

    public Rectangle this[char letter] => letterRects[GetLetterIndex(letter)];

    public Rectangle GetLetterRect(char letter) => letterRects[GetLetterIndex(letter)];

    public int GetLetterIndex(char letter)
    {
        letter = char.ToUpper(letter);
        WasSmallChar = false;

        int letterIndex;
        if (char.IsDigit(letter))
        {
            letterIndex = letter - '0' + 30;
        }
        else
        {
            switch (letter)
            {
                case 'I':
                    letterIndex = 8;
                    WasSmallChar = true;
                    break;

                case '!':
                    letterIndex = 26;
                    WasSmallChar = true;
                    break;

                case '\'':
                    letterIndex = 46;
                    WasSmallChar = true;
                    break;

                case ' ':
                    letterIndex = 47;
                    //WasSmallChar = true;
                    break;

                case '?':
                    letterIndex = 27;
                    break;

                case ':':
                    letterIndex = 28;
                    break;

                case ';':
                    letterIndex = 29;
                    break;

                case '"':
                    letterIndex = 40;
                    break;

                case '(':
                    letterIndex = 41;
                    break;

                case ')':
                    letterIndex = 42;
                    break;

                case ',':
                    letterIndex = 43;
                    break;

                case '-':
                    letterIndex = 44;
                    break;

                case '.':
                    letterIndex = 45;
                    break;

                default:
                    letterIndex = letter - 'A';
                    break;
            }
        }

        return letterIndex;
    }

    public void Dispose() => texture.Dispose();

    private void GenerateRects(int numCells)
    {
        int columns = texture.Width / CellWidth;
        for (int i = 0; i < numCells; i++)
        {
            int x = (i % columns) * CellWidth;
            int y = (i / columns) * CellHeight;
            letterRects.Add(new Rectangle(x, y, CellWidth, CellHeight));
        }
    }
}