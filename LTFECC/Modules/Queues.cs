using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LTFECC.Common;
using LTFECC.Models;
using LTFECC.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTFECC.Modules
{
    public class Queues : ModuleBase<SocketCommandContext>
    {
        private readonly ulong roleVadovas = 873507066134822912;
        private readonly ulong channelCox = 893089158426394694;
        private readonly ulong channelTob = 893089267797098496;
        private readonly ulong channelNm = 893089304715362344;
        private readonly string imageNameCox = "https://i.ibb.co/2KGPR6S/cox.png";
        private readonly string imageNameTob = "https://i.ibb.co/pzS5rKB/tob.png";
        private readonly string imageNameNm = "https://i.ibb.co/QnbXXgD/nm.png";

        private readonly IQueueDB _queueDB;
        public Queues(IQueueDB queueDB){ _queueDB = queueDB; }
        public string FormatOutputString(List<QueueContendor> contendors, string type)
        {
            string formatedQueue = "";
            if (contendors.Count == 0)
            {
                formatedQueue += "> **šiuo metu sąrašas yra tuščias.**\n";
            }
            else
            {
                for (int i = 0; i < contendors.Count; i++)
                        formatedQueue += $"> **{i + 1}.** <@{contendors[i].IdUser}>\n";
            }
            return formatedQueue;
        }

        public async Task UpdateChannelList(ulong channelId, string raidType, string imageName)
        {

            var Channel = Context.Guild.GetChannel(channelId) as SocketTextChannel;

            if (!(Channel is ISocketMessageChannel msgChannel)) return;

            var msg = (await msgChannel.GetMessagesAsync(1).FlattenAsync())?.FirstOrDefault();

            if (msg != null) 
            { 
                await Channel.DeleteMessageAsync(msg);
                await QueueSendMessageToTheChannel(Channel, raidType, imageName);
            }


        }
        public async Task QueueSendMessageToTheChannel(SocketTextChannel Channel, string raidType, string imageName)
        {

            List<QueueContendor> contendorsWaiting = await _queueDB.GetQueueByRaidNotCompletedAsync(raidType);
            string formatedQueueWaitingList = FormatOutputString(contendorsWaiting, raidType);

            List<QueueContendor> contendorsCompleted = await _queueDB.GetQueueByRaidCompletedAsync(raidType);
            var contendorsCompletedDisctinct = contendorsCompleted.GroupBy(x => x.IdUser).Select(y => y.First()).ToList();
            string formatedQueueCompletedList = FormatOutputString(contendorsCompletedDisctinct, raidType);


            var embed = new LTFECCEmbedBuilder()
                //.WithTitle($"{raidType} mokinių sąrašas")
                .AddField($"Žaidėjai laukiantys išmokti **{raidType}** pagal eiliškumą", $"\n{formatedQueueWaitingList}")
                .AddField($"Žaidėjai sėkmingai baigę **{raidType}** pamokas", $"\n{formatedQueueCompletedList}")
                .AddField("__", $"Norite užsiregistruoti į eilę? Privalote užsiregistruoti, parašę betkuriam iš  <@&{roleVadovas}>  narių, nusiuntę jiems savo gear setup nuotrauką, kurioje matytusi jūsų RSN.")
                //.WithDescription($"{formatedQueue}")
                .WithThumbnailUrl($"{imageName}")
                .Build();

            await Channel.SendMessageAsync(embed: embed);
        }

        [Command("qcoxwaiting")]
        [Alias("qcoxw", "qcw")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CoxQueueDisplay()
        {
            string raidType = "COX";

            List<QueueContendor> contendors = await _queueDB.GetQueueByRaidNotCompletedAsync(raidType);
             
            string formatedQueue = FormatOutputString(contendors, raidType);

            await QueueSendMessageToTheChannel((SocketTextChannel)this.Context.Channel, raidType, imageNameCox);

        }

        [Command("qcoxadd")]
        [Alias("qcoxa", "qca")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CoxQueueAdd(SocketGuildUser socketGuildUser)
        {
            string raidType = "COX";

            QueueContendor addcontendor = new QueueContendor()
            {
                IdUser = socketGuildUser.Id.ToString(),
                Username = socketGuildUser.Username.ToString(),
                Discriminator = socketGuildUser.Discriminator.ToString(),
                Raid = raidType,
                TimeAdded = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Completed = false,
            };

            var answer = await _queueDB.AddQueueAsync(addcontendor);

            if (answer)
            {
                await UpdateChannelList(channelCox, raidType, imageNameCox);
                await this.Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await this.Context.Channel.SendMessageAsync("Žaidėjas <@" + socketGuildUser.Id + $"> buvo pridėtas prie **{raidType}** mokinių eilės.");
            }
            else
            {
                await this.Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await this.Context.Channel.SendMessageAsync("Žaidėjas <@" + socketGuildUser.Id + $"> jau yra **{raidType}** mokinių eilėje.");
            }
        }

        

        [Command("qcoxcomplete")]
        [Alias("qcoxc", "qcc")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CoxQueueComplete(SocketGuildUser socketGuildUser)
        {
            string raidType = "COX";

            QueueContendor completedContendor = await _queueDB.GetByIdNotCompletedAsync(socketGuildUser.Id.ToString(), raidType);

            if (completedContendor != null)
            {
                completedContendor.TimeAdded = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                completedContendor.Completed = true;

                var answer = await _queueDB.MarkAsCompletedAsync(completedContendor, raidType);

                await UpdateChannelList(channelCox, raidType, imageNameCox);
                await this.Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await this.Context.Channel.SendMessageAsync("Žaidėjas <@" + socketGuildUser.Id + $"> sėkmingai išmoko **{raidType}** reidą.");
            }
            else
            {
                await this.Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await this.Context.Channel.SendMessageAsync("Žaidėjas <@" + socketGuildUser.Id + $"> nebuvo rastas **{raidType}** mokinių eilėje.");
            }
        }
        
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("qcoxremove")]
        [Alias("qcoxr", "qcr")]
        public async Task CoxQueueRemove(SocketGuildUser socketGuildUser)
        {
            string raidType = "COX";
            QueueContendor removecontendor = new QueueContendor()
            {
                IdUser = socketGuildUser.Id.ToString(),
                Username = socketGuildUser.Username.ToString(),
                Discriminator = socketGuildUser.Discriminator.ToString(),
                Raid = raidType,
            };

            var answer = await _queueDB.RemoveQueueAsync(removecontendor);
            if(answer)
            {
                await UpdateChannelList(channelCox, raidType, imageNameCox);
                await this.Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await this.Context.Channel.SendMessageAsync("Žaidėjas <@" + socketGuildUser.Id + "> buvo pašalintas **COX** iš mokinių eilės.");
            }
            else
            {
                await this.Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await this.Context.Channel.SendMessageAsync("Žaidėjas <@" + socketGuildUser.Id + "> neegzistuoja **COX** mokinių eilėje.");
            }
        }
    }
}
