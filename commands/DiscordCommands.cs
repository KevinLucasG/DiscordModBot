using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.commands
{
    public class DiscordCommands : BaseCommandModule
    {
        [Command("hi")]
        public async Task HelloCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Olá {ctx.User.Username}, como poso lhe ajudar? ");
        }

        [Command("!ban")]
        public async Task BanUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = "Sem razão especificada")
        {
            // Verifica se o usuário que está executando o comando tem permissão para banir
            if (!ctx.Member.Permissions.HasFlag(DSharpPlus.Permissions.BanMembers))
            {
                await ctx.RespondAsync("Você não tem permissão para banir membros.");
                return;
            }

            // Impede que o bot se auto-bane
            if (member == ctx.Member)
            {
                await ctx.RespondAsync("Você não pode se banir.");
                return;
            }
            // Verifica se o membro a ser banido é o dono do servidor
            if (member == ctx.Guild.Owner)
            {
                await ctx.RespondAsync("Você não pode banir o dono do servidor.");
                return;
            }

            // Tenta banir o usuário
            try
            {
                await member.BanAsync(reason: reason);
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} foi banido com sucesso. Razão: {reason}");
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync($"Ocorreu um erro ao tentar banir {member.DisplayName}: {ex.Message}");
            }
        }
    }
}
