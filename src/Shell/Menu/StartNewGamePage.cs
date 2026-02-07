using System;
using BrineAndCoin.Core;

namespace BrineAndCoin.Menu;

public class StartNewGamePage : MenuPage
{
    public override string? Title => "New Game";
    protected override bool RenderTitle { get; } = false;

    protected override void Render()
    {
        Console.Clear();

        //await writer.PlayAsync("Assets/Texts/intro.txt", "Assets/Voice/intro.mp3");

        var game = BrineAndCoinGame.NewGame();
        game.Execute();
    }
}
