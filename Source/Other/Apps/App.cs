using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
namespace Celeste.Mod.KoseiHelper.Other.Apps;

[CustomEntity("KoseiHelper/App")]
[Tracked]
public abstract class App : Entity
{
    protected static Color Black => Calc.HexToColor(0x0c0c0c);
    protected static Color White => Calc.HexToColor(0xfbfbf7);
    protected static Color DarkGray => Calc.HexToColor(0x87888f);
    protected static Color LightGray => Calc.HexToColor(0xc0c7c8);
    protected static Color DarkCream => Calc.HexToColor(0xd5cda3);
    protected static Color Cream => Calc.HexToColor(0xe8e3ce);
    protected static Color DarkRed => Calc.HexToColor(0xab0e03);
    protected static Color Red => Calc.HexToColor(0xf63527);
    protected static Color DarkOrange => Calc.HexToColor(0xa96814);
    protected static Color Orange => Calc.HexToColor(0xf99c23);
    protected static Color DarkYellow => Calc.HexToColor(0xaaa759);
    protected static Color Yellow => Calc.HexToColor(0xf1ec58);
    protected static Color DarkGreen => Calc.HexToColor(0x0ab20a);
    protected static Color Green => Calc.HexToColor(0x2bf72b);
    protected static Color DarkCyan => Calc.HexToColor(0x5aa6ab);
    protected static Color Cyan => Calc.HexToColor(0x27effb);
    protected static Color DarkBlue => Calc.HexToColor(0x0b188f);
    protected static Color Blue => Calc.HexToColor(0x1f34f6);
    protected static Color DarkMagenta => Calc.HexToColor(0xa958a2);
    protected static Color Magenta => Calc.HexToColor(0xf940ea);
    protected static Color DarkBrown => Calc.HexToColor(0x896642);
    protected static Color Brown => Calc.HexToColor(0xe8ae73);
    protected static Color Transparent => Calc.HexToColor(0x00000000);


    protected Button buttonClose;
    protected Button buttonMinSize, buttonMaxSize;

    protected const int buttonOffset = 5; // margin between buttons
    protected const int buttonWidth = 65;
    protected const int buttonHeight = 20;
    protected const int smallButtonSize = 16;

    protected int minWidth = 600;
    protected int minHeight = 400;
    protected Rectangle window = new Rectangle(50, 50, 600, 400);

    protected bool maximized;

    public App(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        buttonClose = new Button(new Rectangle(0, 0, 16, 16), "", () => RemoveSelf(), Transparent, Transparent, Transparent,
            texture: GFX.Gui["x"], false, 0.5f, Red, true);
        buttonMinSize = new Button(new Rectangle(0, 0, 16, 16), "", () => Minimize(), Transparent, Transparent, Transparent,
            texture: GFX.Gui["KoseiHelper/apps/button_minimize"], false, 0.5f, White, true);
        buttonMaxSize = new Button(new Rectangle(0, 0, 16, 16), "", () => Maximize(), Transparent, Transparent, Transparent,
            texture: GFX.Gui["KoseiHelper/apps/button_maximize"], false, 0.5f, White, true);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        // Ensure the window spawns inside the bounds of the screen
        if ((int)MInput.Mouse.Position.X <= 0f)
            window.X = (int)(0f);
        if ((int)MInput.Mouse.Position.Y <= 0f)
            window.Y = (int)(0f);
        if ((int)MInput.Mouse.Position.X >= 1920f - window.Width)
            window.X = (int)(1921f - window.Width);
        if ((int)MInput.Mouse.Position.Y >= 1080f - window.Height)
        {
            window.Y = (int)(1081f - window.Height);
        }
    }

    public override void Render()
    {
        base.Render();
    }

    public virtual void Minimize()
    {
        window.Width = minWidth;
        window.Height = minHeight;
        maximized = false;
    }

    public virtual void Maximize()
    {
        window = new Rectangle(0, 0, 1921, 1080);
        window.X = 0;
        window.Y = 0;
        maximized = true;
    }
}