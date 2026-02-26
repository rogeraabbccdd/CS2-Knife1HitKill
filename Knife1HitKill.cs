using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Knife1HitKill;

public class Knife1HitKill : BasePlugin
{
    public override string ModuleName => "Knife 1 Hit Kill";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Kento";
    public override string ModuleDescription => "Knife 1 Hit Kill";
    private ConVar? mp_teammates_are_enemies = ConVar.Find("mp_teammates_are_enemies");

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnEntityTakeDamagePre>(OnTakeDamage);
    }
    private HookResult OnTakeDamage(CEntityInstance entity, CTakeDamageInfo info)
    {
        if (
            entity is null || !entity.IsValid || !info.Attacker.IsValid || info.Attacker.Value is null || info.Ability.Value is null ||
            entity.DesignerName != "player" || info.Attacker.Value.DesignerName != "player" ||
            !info.Ability.Value.DesignerName.Contains("knife")
        ) return HookResult.Continue;
        
        CCSPlayerPawn pawn = entity.As<CCSPlayerPawn>();

        if (mp_teammates_are_enemies != null)
        {
            bool ffa = mp_teammates_are_enemies.GetPrimitiveValue<bool>();
            if (!ffa && pawn.TeamNum == info.Attacker.Value.TeamNum)
            {
                return HookResult.Continue;
            }
        }

        info.Damage = pawn.Health + pawn.ArmorValue;

        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnPlayerDeathPre(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event.Weapon.Contains("knife") || @event.Weapon.Contains("bayonet"))
        {
            @event.Headshot = true;
        }

        return HookResult.Continue;
    }
}