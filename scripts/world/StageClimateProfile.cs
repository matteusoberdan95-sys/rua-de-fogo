namespace SangueNoAsfalto.World;

using SangueNoAsfalto.Core;
/// Clima e horario por ato da Vila Esperanca (docs/STAGE_01_VILA_ESPERANCA.md).
/// </summary>
public static class StageClimateProfile
{
    public enum Act
    {
        Entrada,
        Martins,
        Central,
        Viela,
        Portao,
    }

    public readonly record struct ActClimate(
        WeatherController.WeatherState Weather,
        TimeOfDayController.TimeOfDayState Time,
        float FogBoost,
        float WindStrength,
        string Hint);

    public readonly record struct BossClimate(
        WeatherController.WeatherState Weather,
        TimeOfDayController.TimeOfDayState Time,
        float FogBoost,
        float WindStrength,
        bool PulseBlackoutOnStart,
        string Status);

    public static Act ResolveAct(float playerX)
    {
        return playerX switch
        {
            < 480f => Act.Entrada,
            < 1180f => Act.Martins,
            < 2050f => Act.Central,
            < 2780f => Act.Viela,
            _ => Act.Portao,
        };
    }

    public static ActClimate GetActClimate(Act act)
    {
        return act switch
        {
            Act.Entrada => new ActClimate(
                WeatherController.WeatherState.Drizzle,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.08f,
                WindStrength: 0.35f,
                Hint: "Garoa fina — piso escorregadio."),
            Act.Martins => new ActClimate(
                WeatherController.WeatherState.HeavyRain,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.12f,
                WindStrength: 0.45f,
                Hint: "Chuva forte perto do altar."),
            Act.Central => new ActClimate(
                WeatherController.WeatherState.HeavyRain,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.10f,
                WindStrength: 0.55f,
                Hint: "Noite na rua central — cuidado com pocas."),
            Act.Viela => new ActClimate(
                WeatherController.WeatherState.Thunderstorm,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.18f,
                WindStrength: 0.85f,
                Hint: "Temporal na viela — raio e lama."),
            Act.Portao => new ActClimate(
                WeatherController.WeatherState.Thunderstorm,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.28f,
                WindStrength: 0.65f,
                Hint: "Neblina no portao — chefes da chuva."),
            _ => GetActClimate(Act.Entrada),
        };
    }

    public static BossClimate? GetBossClimate(StageScrollSpawns.SpawnKind kind, bool bossAlive)
    {
        if (!bossAlive)
        {
            return null;
        }

        return kind switch
        {
            StageScrollSpawns.SpawnKind.MiniBoss => new BossClimate(
                WeatherController.WeatherState.HeavyRain,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.22f,
                WindStrength: 0.70f,
                PulseBlackoutOnStart: false,
                Status: "O chefe traz a chuva consigo."),
            StageScrollSpawns.SpawnKind.RainBoss => new BossClimate(
                WeatherController.WeatherState.Thunderstorm,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.32f,
                WindStrength: 0.90f,
                PulseBlackoutOnStart: true,
                Status: "Infectado da chuva — raios mais frequentes!"),
            StageScrollSpawns.SpawnKind.AlphaBoss => new BossClimate(
                WeatherController.WeatherState.Thunderstorm,
                TimeOfDayController.TimeOfDayState.Night,
                FogBoost: 0.26f,
                WindStrength: 1.0f,
                PulseBlackoutOnStart: true,
                Status: "Alfa da rua — apagao e trovao!"),
            _ => null,
        };
    }
}
