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
             if (!ctx.Member.Permissions.HasFlag(DSharpPlus.Permissions.ManageRoles))
            {
                await ctx.RespondAsync("Você não tem permissão para utilizar esse bot.");
                return;
            }
            await ctx.Channel.SendMessageAsync($"Olá {ctx.User.Username}, como poso lhe ajudar? ");
        }

        [Command("ban")]
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
                await member.SendMessageAsync($"Devido às suas ações ou por ter quebrado as regras, tivemos que lhe banir. Razão: {reason}");
                await member.BanAsync(reason: reason);
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} foi banido com sucesso. Razão: {reason}");
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync($"Ocorreu um erro ao tentar banir {member.DisplayName}: {ex.Message}");
            }
        }


        //!kick @UsuárioOfensivo [razão opcional]
        [Command("kick")]
        public async Task KickUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = "Sem razão especificada")
        {
            if (!ctx.Member.Permissions.HasFlag(DSharpPlus.Permissions.KickMembers))
            {
                await ctx.RespondAsync("Você não tem permissão para expulsar membros.");
                return;
            }
            if (member == ctx.Member)
            {
                await ctx.RespondAsync("Você não pode se expulsar.");
                return;
            }
            if (member == ctx.Guild.Owner)
            {
                await ctx.RespondAsync("Você não pode expulsar o dono do servidor.");
                return;
            }

            try
            {
                await member.SendMessageAsync($"Você foi kickado do servidor. Razão: {reason} Esperamos que você possa refletir sobre suas ações e decida voltar ao servidor em um futuro próximo.");
                await member.RemoveAsync(reason: reason);           
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} foi expulso com sucesso. Razão: {reason}");
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync($"Ocorreu um erro ao tentar expulsar o {member.DisplayName}: {ex.Message}");
            }


        }

        [Command("mute")]


        //!mute @UsuárioOfensivo 00:10 Razão opcional
        public async Task MuterUser(CommandContext ctx, DiscordMember member, TimeSpan duration, [RemainingText] string reason = "Sem razão especificada")
        {
            if (!ctx.Member.Permissions.HasFlag(DSharpPlus.Permissions.ManageRoles))
            {
                await ctx.RespondAsync("Você não tem permissão para mutar membros.");
                return;
            }

            if (member == ctx.Member)
            {
                await ctx.RespondAsync("Você não pode se mutar.");
                return;
            }


            if (member == ctx.Guild.Owner)
            {
                await ctx.RespondAsync("Você não pode mutar o dono do servidor.");
                return;
            }

            // Encontra o cargo "Muted"
            var muteRole = ctx.Guild.Roles.Values.FirstOrDefault(role => role.Name.ToLower() == "muted");

            if (muteRole == null)
            {
                await ctx.RespondAsync("Cargo de Mutado não encontrado. Por favor, crie um cargo chamado 'Muted' e configure-o corretamente.");
                return;
            }

            try
            {
                await member.GrantRoleAsync(muteRole);
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} foi mutado por {duration.TotalMinutes} minutos. Razão: {reason}");

                await Task.Delay(duration);
                await member.RevokeRoleAsync(muteRole);
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} foi desmutado após {duration.TotalMinutes} minutos.");
                
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync($"Ocorreu um erro ao tentar mutar {member.DisplayName}: {ex.Message}");
            }
        }

        [Command("unmute")]
        public async Task UnmuteUser(CommandContext ctx, DiscordMember member)
        {

            if (!ctx.Member.Permissions.HasFlag(DSharpPlus.Permissions.ManageRoles))
            {
                await ctx.RespondAsync("Você não tem permissão para desmutar membros.");
                return;
            }

            // Encontra o cargo "Muted"
            var muteRole = ctx.Guild.Roles.Values.FirstOrDefault(role => role.Name.ToLower() == "muted");


            if (muteRole == null)
            {
                await ctx.RespondAsync("Cargo de Mutado não encontrado. Por favor, crie um cargo chamado 'Muted' e configure-o corretamente.");
                return;
            }

            try
            {
                await member.RevokeRoleAsync(muteRole);
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} foi desmutado");
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync($"Ocorreu um erro ao tentar desmutar {member.DisplayName}: {ex.Message}");
            }

        }
        [Command("warn")]
        public async Task WarnUser(CommandContext ctx, DiscordMember member, [RemainingText] string reason = "Sem razão especificada")
        {
            if (!ctx.Member.Permissions.HasFlag(DSharpPlus.Permissions.ManageRoles))
            {
                await ctx.RespondAsync("Você não tem permissão para avisar outros membros.");
                return;
            }

            if (member == null)
            {
                await ctx.RespondAsync("Usuário não encontrado ou inexistente");
                return;
            }

            await member.SendMessageAsync($"Você recebeu um aviso no servidor **{ctx.Guild.Name}**. Motivo: {reason}");


            // Registra o aviso no chat onde o comando foi usado
            var embed = new DiscordEmbedBuilder
            {
                Title = "Usuário avisado",
                Description = $"{member.Username}, recebeu um aviso pelo motivo de: {reason} ",
                Color = DiscordColor.Orange
            };
            await ctx.Channel.SendMessageAsync(embed);
        }
    }
}
