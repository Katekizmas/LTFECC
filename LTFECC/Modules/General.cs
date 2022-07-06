namespace LTFECC.Modules
{
    using System.Linq;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using LTFECC.Common;

    // Komandos : Context ( gauti informaciją apie vartotoją, procesą, žinutę)
    /// <summary>
    /// Modulis skirtas bot'o komandų informacijai gauti
    /// </summary>
    public class General : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Komanda skirta nuostabiausiam Katekizmui pagerbti.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("katekizmas")]
        [Alias("k")] // komandos sutrumpinimas
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task PingAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();
            await this.Context.Channel.SendMessageAsync("Katekizmas geriausias! :)");
        }

        /// <summary>
        /// Komanda skirta vartotojo duomenims gauti apie vartotoją.
        /// </summary>
        /// <param name="socketGuildUser">Pasirinkti ar gauti informaciją apie tam tikrą vartotoją, ar tiesiog savo nenurodant vartotojo.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("info")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task InfoAsync(SocketGuildUser socketGuildUser = null)
        {
            if (socketGuildUser == null)
            {
                socketGuildUser = this.Context.User as SocketGuildUser; // Jeigu komandoje nenurodytas vartotojas @, grąžinama informacija apie tą kuris iškvietė komandą.
            }

            var embed = new LTFECCEmbedBuilder()
                .WithTitle($"{socketGuildUser.Username}#{socketGuildUser.Discriminator}")
                .AddField("ID", socketGuildUser.Id, true)
                .AddField("Slapyvardis", $"{socketGuildUser.Username}#{socketGuildUser.Discriminator}\n", true)
                .AddField("Sukurtas", socketGuildUser.CreatedAt.ToString("yyyy-MM-dd HH:mm"), true)
                .WithThumbnailUrl(socketGuildUser.GetAvatarUrl() ?? socketGuildUser.GetDefaultAvatarUrl())
                .WithCurrentTimestamp()
                .Build();

            await this.ReplyAsync(embed: embed);
        }

        [Command("pagalba")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task PagalbaAsync()
        {
            var imageName = "https://i.ibb.co/VC6Kjp8/iron.png"; // iron.png
            var embed = new LTFECCEmbedBuilder()
                .WithTitle("LTFECC roboto komandų sąrašas")
                .AddField("Pagalbinės roboto komandų sarašas", $"> ****istrinti (skaičius)*** - ištrinti žinutę\n")
                .AddField("Mokinių eilės **COX** komandų sarašas", $"> ****qcw*** - parodyti sąrašą\n > ****qca @narys*** - pridėti mokinį\n > ****qcc @narys*** - pridėti prie išmokusių sąrašo\n > ****qcr @narys*** - ištrinti mokinį\n")
                .AddField("Nacionalinio fondo komandų sarašas", $"Jeigu norite gauti visą rimtų kentų sąrašą, kurie paaukojo į bendrą fondą įveskite komandą: ****fondas***\n")
                .WithThumbnailUrl($"{imageName}")
                .WithCurrentTimestamp()
                .Build();

            //await this.Context.Channel.SendFileAsync(imageName, embed: embed);
            await this.ReplyAsync(embed: embed);
        }

        [Command("istrinti")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task IstrintiAsync(int amount = 1)
        {
            var messages = await this.Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (this.Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await this.Context.Channel.SendMessageAsync($"{messages.Count() - 1} žinutės buvo ištrintos!");
            await Task.Delay(3000);
            await message.DeleteAsync();
        }
    }
}
