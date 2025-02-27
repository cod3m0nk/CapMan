namespace CapMan.Raylib;
using Raylib_cs;

public class BoardRenderer
{
    public const int CellSize = 16;
    private static Dictionary<Element, Texture2D>? s_textures;
    public static IReadOnlyDictionary<Element, Texture2D> Textures => LoadTextures().AsReadOnly();

    public void Render(Board board, int left, int top)
    {
        foreach ((Tile pos, Element el) in board.Elements)
        {
            Raylib.DrawTexture(Textures[el], left + pos.X * CellSize, top + pos.Y * CellSize, Color.White);
        }
    }

    public static Dictionary<Element, Texture2D> LoadTextures()
    {
        if (s_textures == null)
        {
            s_textures = new();
            foreach (Element element in Enum.GetValues<Element>().Where(el => el is not Element.Corner))
            {
                Image image = Raylib.LoadImage(AssetPath(element));
                Texture2D texture = Raylib.LoadTextureFromImage(image);
                s_textures[element] = texture;
                Raylib.UnloadImage(image);
            }
        }
        return s_textures;
    }

    private static string AssetPath(Element element)
    {
        return Path.Combine("assets", "sprites", element switch
        {
            Element.Horizontal => "horizontal.png",
            Element.Vertical => "vertical.png",
            Element.TopLeft => "top-left.png",
            Element.TopRight => "top-right.png",
            Element.BottomLeft => "bottom-left.png",
            Element.BottomRight => "bottom-right.png",
            Element.Dot => "dot.png",
            Element.PowerPill => "powerpill.png",
            Element.Door => "door.png",
            _ => throw new ArgumentException($"No sprite found for board element: {element}"),
        });
    }
}