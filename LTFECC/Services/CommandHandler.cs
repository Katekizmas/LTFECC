namespace LTFECC.Services
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Klasė skirta apdoroti įvarias komandas, įvykius ir klaidas.
    /// </summary>
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider provider;
        private readonly DiscordSocketClient client;
        private readonly CommandService service;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="provider">Tai <see cref="IServiceProvider"/> kuris turi būti inicijuojamas.</param>
        /// <param name="client">Tai <see cref="DiscordSocketClient"/> kuris turi būti inicijuojamas.</param>
        /// <param name="service">Tai <see cref="CommandService"/> kuris turi būti inicijuojamas.</param>
        /// <param name="configuration">Tai <see cref="IConfiguration"/> kuris turi būti inicijuojamas.</param>
        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration)
        {
            this.provider = provider;
            this.client = client;
            this.service = service;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            this.client.MessageReceived += this.OnUserMessageReceived;
            this.service.CommandExecuted += this.OnCommandExcecuted;
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task OnCommandExcecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result) // Komanda skirta klaidų atvaizdavimui
        {
            if (result.IsSuccess) return;

            await commandContext.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task OnUserMessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(this.client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(this.client, message);
            await this.service.ExecuteAsync(context, argPos, this.provider);
        }
    }
}
