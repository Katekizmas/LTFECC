namespace LTFECC.Common
{
    using Discord;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Custom embed builder su temomis.
    /// </summary>
    internal class LTFECCEmbedBuilder : EmbedBuilder
    {
        public LTFECCEmbedBuilder()
        {
            this.WithColor(new Color(252, 186, 3));
        }
    }
}
