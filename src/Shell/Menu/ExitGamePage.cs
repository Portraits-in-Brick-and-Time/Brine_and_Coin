using System;
using BrineAndCoin.Core;

namespace BrineAndCoin.Menu;

public class ExitGamePage : MenuPage
{
    public override string? Title => "Exit";

    protected override void Render()
    {
        Environment.Exit(0);
    }
}
