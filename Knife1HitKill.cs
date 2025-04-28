using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace Knife1HitKill;

public class Knife1HitKill : BasePlugin
{
    public override string ModuleName => "Knife 1 Hit Kill";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Kento";
    public override string ModuleDescription => "Knife 1 Hit Kill";

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(OnMapStartHandler);
        RegisterListener<Listeners.OnMapEnd>(OnMapEndHandler);
        
    }

    public override void Unload(bool hotReload)
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage, HookMode.Pre);
    }

    private void OnMapEndHandler()
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage, HookMode.Pre);
    }

    private void OnMapStartHandler(string mapName)
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage, HookMode.Pre);
    }

    private HookResult OnTakeDamage(DynamicHook hook)
    {
        CEntityInstance entity = hook.GetParam<CEntityInstance>(0);
        CTakeDamageInfo info = hook.GetParam<CTakeDamageInfo>(1);

        if (
            entity is null || !entity.IsValid || !info.Attacker.IsValid || info.Attacker.Value is null || info.Ability.Value is null ||
            entity.DesignerName != "player" || info.Attacker.Value.DesignerName != "player" ||
            !info.Ability.Value.DesignerName.Contains("knife")
        ) return HookResult.Continue;
        
        CCSPlayerPawn pawn = entity.As<CCSPlayerPawn>();

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