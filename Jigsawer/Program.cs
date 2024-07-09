namespace Jigsawer;

public static class Program {
    public static void Main() {
        using var game = new Game(1600, 900, nameof(Jigsawer));

        game.Run();
    }
}
