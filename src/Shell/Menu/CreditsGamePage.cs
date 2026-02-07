using System;
using BrineAndCoin.Core;

namespace BrineAndCoin.Menu;

public class CreditsGamePage : MenuPage
{
    public override string? Title => "Credits";

    protected override void Render()
    {
        Console.WriteLine("The Game is written by Chris Anders");
    }
}
