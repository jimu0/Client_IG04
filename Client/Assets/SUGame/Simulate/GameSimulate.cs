
public class GameSimulate
{
    public void Step(GameState state,GameInput input,float dt)
    {
        foreach (GameMode gameMode in state.gameModes)
        {
            gameMode.Step(input, dt);
        }
    }
}
