using Thor;

namespace UnderMineControl.API
{
    public interface IPlayer
    {
        HealthExt Health { get; }
        DamageExt Damage { get; }

        int MaxHP { get; set; }
        int CurrentHP { get; set; }
        bool Invulnerable { get; set; }

        int Bombs { get; set; }
        int Keys { get; set; }
        int Gold { get; set; }
        int Thorium { get; set; }

        void SetValue<T>(string member, float value, Modifier.Operator op = null);
        void SetValue<T>(string member, bool value, Modifier.Operator op = null);

        void AddRandomBlessing();
        void AddRandomCurse(HealthExt.CurseType type = HealthExt.CurseType.Minor);
        void RemoveRandomCurse(out Entity curse, HealthExt.CurseType type = HealthExt.CurseType.Minor);
        void ChangeHP(int amount, HealthExt.DamageType type = HealthExt.DamageType.Physical, bool ignoreGrace = true);
    }
}
