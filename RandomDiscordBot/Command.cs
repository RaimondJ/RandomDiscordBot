using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomDiscordBot
{
    public class Command
    {
        public int varLength { get; set; }
        public string usage { get; set; }
        public Command(int varLength, string usage) 
        {
            this.varLength = varLength;
            this.usage = usage;
        }
    }
}
