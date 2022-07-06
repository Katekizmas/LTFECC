namespace LTFECC.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using LTFECC.Common;
    using LTFECC.Models;

    public class Bankas : ModuleBase<SocketCommandContext>
    {
        [Command("fondas")]
        [Alias("f")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task PagalbaAsync()
        {
            long fullAmount = 0; string formatedFullAmount = "";
            string outputFormated = "";
            List<Donators> donatorsList = new List<Donators>()
            {
                new Donators() { Id_server = "45641231231", Id_user = "341531321", Username = "Katekizmas", Amount = "1853153618" },
                new Donators() { Id_server = "45641231231", Id_user = "8979456", Username = "Kitten Fist", Amount = "32138813" },
                new Donators() { Id_server = "45641231231", Id_user = "321312", Username = "Diclo Force", Amount = "645648648" },
                new Donators() { Id_server = "45641231231", Id_user = "6936123", Username = "Nami", Amount = "465452444" },
                new Donators() { Id_server = "45641231231", Id_user = "45646486", Username = "Zxql", Amount = "45645312" },
                new Donators() { Id_server = "45641231231", Id_user = "1212344", Username = "Karami", Amount = "3475231" },
                new Donators() { Id_server = "45641231231", Id_user = "3010185", Username = "EdgaRyto", Amount = "35434536" },
            };
            donatorsList.Sort((x, y) => x.Amount.CompareTo(y.Amount)); // is db gaut sorted list

            for (int i = 0; i < donatorsList.Count; i++)
            {
                fullAmount += long.Parse(donatorsList[i].Amount);
                char[] charLenght = donatorsList[i].Amount.ToCharArray();
                string formatedAmount = ""; int counter = 0;

                for (int j = charLenght.Length - 1; j >= 0; j--)
                {
                    if (++counter % 3 == 0 && counter != charLenght.Length)
                        formatedAmount = $",{ charLenght[j] }" + formatedAmount;
                    else
                        formatedAmount = charLenght[j] + formatedAmount;
                }

                donatorsList[i].Amount = formatedAmount;
            }

            {
                char[] charLenght = fullAmount.ToString().ToCharArray();
                int counter = 0;
                for (int j = charLenght.Length - 1; j >= 0; j--)
                {
                    if (++counter % 3 == 0 && counter != charLenght.Length)
                        formatedFullAmount = $",{ charLenght[j] }" + formatedFullAmount;
                    else
                        formatedFullAmount = charLenght[j] + formatedFullAmount;
                }

            }


            for (int i = 0; i < donatorsList.Count; i++)
            {
                if (i == 0)
                    outputFormated += $":first_place: ***{donatorsList[i].Username} ⇒*** :money_with_wings: {donatorsList[i].Amount} *GP*.\n";
                else if (i == 1)
                    outputFormated += $":second_place: ***{donatorsList[i].Username} ⇒*** :money_with_wings: {donatorsList[i].Amount} *GP*.\n";
                else if (i == 2)
                    outputFormated += $":third_place: ***{donatorsList[i].Username} ⇒*** :money_with_wings: {donatorsList[i].Amount} *GP*.\n";
                else
                    outputFormated += $":heart: ***{donatorsList[i].Username} ⇒*** :money_with_wings: {donatorsList[i].Amount} *GP*.\n";
            }
            outputFormated += $"\n\n:moneybag: Iš viso sukaupta ⇒ :coin: {formatedFullAmount} *GP*.";
            var imageName = "https://i.ibb.co/t3tR2SP/fondaslogo.png"; //fondaslogo.png
            var embed = new LTFECCEmbedBuilder()
                .WithTitle("LTFECC dėkoja esantiems remėjams!")
                .WithDescription("Kadangi mūsų gretos sparčiai auga, sukaupti GP bus naudojami event'ų finansavimui, bei bendruomenės veiklai remti. Norint prisidėti, galite susisiekti su bet kuriuo administratoriumi.\n\n" + outputFormated)
                .WithThumbnailUrl($"{imageName}")
                .Build();

            await this.ReplyAsync(embed: embed);
        }
    }
}
